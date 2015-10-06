//-----------------------------------------------------------------------
// <copyright file="AsyncValidatedObservableObject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyToolkit.Model
{
    /// <summary>Implementation of the <see cref="INotifyPropertyChanged"/> interface with <see cref="INotifyDataErrorInfo"/> support and async validation.</summary>
    /// <remarks>Because the property access and validation may happen on different threads, 
    /// the class must be thread-safe. Use the <see cref="ValidatedObservableObject._lock"/> object for locking. </remarks>
    public class AsyncValidatedObservableObject : ValidatedObservableObject
    {
        /// <summary>Initializes a new instance of the <see cref="AsyncValidatedObservableObject"/> class.</summary>
        public AsyncValidatedObservableObject()
        {
            AsyncValidation = true;
        }

        /// <summary>Gets or sets a value indicating whether to use asynchronous validation when a property changed.</summary>
        public bool AsyncValidation { get; set; }

        /// <summary>Asynchronously validates all properties and the object invariants.</summary>
        /// <returns>The <see cref="Task"/>. </returns>
        public async Task ValidateAsync()
        {
            var tasks = new Dictionary<string, Task<ICollection<string>>>();
            foreach (var property in GetType().GetRuntimeProperties().Where(p => p.CanRead))
            {
                var propertyName = property.Name;
                tasks.Add(property.Name, ValidatePropertyAsync(propertyName));
            }

            tasks.Add(string.Empty, ValidateInvariantsAsync());
            await Task.WhenAll(tasks.Values);

            foreach (var pair in tasks)
                SetPropertyErrors(pair.Key, pair.Value.Result);
        }

        /// <summary>Called when a property needs to be validated. </summary>
        /// <param name="sender">The sender. </param>
        /// <param name="args">The arguments. </param>
        protected override async void OnValidateProperty(object sender, PropertyChangedEventArgs args)
        {
            if (AsyncValidation)
            {
                var validatePropertyTask = ValidatePropertyAsync(args.PropertyName);
                var validateInvariantsTask = ValidateInvariantsAsync();

                await Task.WhenAll(validatePropertyTask, validateInvariantsTask);

                SetPropertyErrors(args.PropertyName, validatePropertyTask.Result);
                SetPropertyErrors(string.Empty, validateInvariantsTask.Result);
            }
            else
                base.OnValidateProperty(sender, args);
        }

        /// <summary>Updates the property and raises the changed event, but only if the new value does not equal the old value. </summary>
        /// <param name="propertyName">The property name as lambda. </param><param name="oldValue">A reference to the backing field of the property. </param>
        /// <param name="newValue">The new value. </param>
        /// <remarks>In the <see cref="ValidatedObservableObject"/> override, the changing of the property is locked using the <see cref="ValidatedObservableObject._lock"/> object. </remarks>
        /// <returns>True if the property has changed. </returns>
        public override bool Set<T>(string propertyName, ref T oldValue, T newValue)
        {
            if (Equals(oldValue, newValue))
                return false;

            lock (Lock)
                oldValue = newValue;

            RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
            return true;
        }

        private Task<ICollection<string>> ValidatePropertyAsync(string propertyName)
        {
            return Task.Run(() =>
            {
                lock (Lock)
                    return ValidateProperty(propertyName);
            });
        }

        private Task<ICollection<string>> ValidateInvariantsAsync()
        {
            return Task.Run(() =>
            {
                lock (Lock)
                    return ValidateInvariants();
            });
        }
    }
}