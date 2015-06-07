using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Serialization;

namespace MyToolkit.Utilities
{
    // TODO: Improve these classes

#pragma warning disable 1591

    public interface ICommandEx
    {
        Task<bool> Run();
        string RemoveIdentifier { get; }
    }

    /// <summary>
    /// Provides a command processor. 
    /// </summary>
    public class CommandProcessor
    {
        private bool _isProcessing = false;
        private List<ICommandEx> _commands = new List<ICommandEx>();

        public async Task<bool> ProcessAsync(ICommandEx command)
        {
            lock (this)
            {
                if (command.RemoveIdentifier != null)
                {
                    foreach (var cmd in _commands.Where(c => c.RemoveIdentifier == command.RemoveIdentifier))
                        _commands.Remove(cmd);
                }

                _commands.Add(command);
            }
            return await ProcessAsync();
        }

        private readonly List<TaskCompletionSource<bool>> _waitingTasks = new List<TaskCompletionSource<bool>>();
        public async Task<bool> ProcessAsync()
        {
            if (_isProcessing)
            {
                var source = new TaskCompletionSource<bool>();
                lock (this)
                    _waitingTasks.Add(source);
                await source.Task;
                return source.Task.Result;
            }

            lock (this)
                _isProcessing = true;

            ICommandEx[] commandsCopy;
            lock (this)
                commandsCopy = _commands.ToArray();

            foreach (var command in commandsCopy)
            {
                var successful = false;

                try
                {
                    successful = await command.Run();
                    lock (this)
                        _commands.Remove(command);
                }
                catch (Exception)
                {
                    //if (Debugger.IsAttached)
                    //	Debugger.Break();
                }

                if (!successful)
                {
                    lock (this)
                    {
                        _isProcessing = false;
                        foreach (var source in _waitingTasks)
                            source.SetResult(false);
                        _waitingTasks.Clear();
                    }
                    return false;
                }
            }

            var hasMoreCommands = false;
            lock (this)
            {
                hasMoreCommands = _commands.Count > 0;
                _isProcessing = false;
            }

            if (hasMoreCommands)
                await ProcessAsync();

            lock (this)
            {
                foreach (var source in _waitingTasks)
                    source.SetResult(true);
                _waitingTasks.Clear();
            }
            return true;
        }

        public void Load(string xml, Type[] commandTypes)
        {
            lock (this)
            {
                _commands = xml != null ?
                    new List<ICommandEx>(XmlSerialization.Deserialize<List<object>>(xml, commandTypes).OfType<ICommandEx>()) :
                    new List<ICommandEx>();
            }
        }

        public string Save(Type[] commandTypes)
        {
            lock (this)
                return XmlSerialization.Serialize(_commands.OfType<object>().ToList(), commandTypes);
        }
    }

    #pragma warning restore 1591
}
