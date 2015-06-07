using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Command;

namespace MyToolkit.Tests.WinRT.Command
{
    [TestClass]
    public class AsyncRelayCommandTests
    {
        [TestMethod]
        public async Task When_passing_function_with_task_then_command_cannot_be_executed_during_task_execution()
        {
            var command = new AsyncRelayCommand(async () => { await Task.Delay(1000); });
            Assert.IsTrue(command.CanExecute);

            command.TryExecute();
            Assert.IsFalse(command.CanExecute);

            await Task.Delay(1500);
            Assert.IsTrue(command.CanExecute);
        }
    }
}
