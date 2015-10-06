//-----------------------------------------------------------------------
// <copyright file="ValidationObject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MyToolkit.Model
{
    /// <summary>Implementation of the <see cref="INotifyPropertyChanged"/> interface with <see cref="INotifyDataErrorInfo"/> support.</summary>
    public class ValidatedObservableObject : ObservableObject, INotifyDataErrorInfo
    {
        private Dictionary<string, ICollection<string>> _errors;
        private readonly object _lock = new object();

        /// <summary>Initializes a new instance of the <see cref="MyToolkit.Model.ValidatedObservableObject"/> class.</summary>
        public ValidatedObservableObject()
        {
            NotValidatedProperties = new HashSet<string>
            {
                "NotValidatedProperties", 
                "HasErrors", 
                "InvalidProperties", 
                "AutoValidateProperties", 
                "Lock", 
                "InvariantErrors"
            };
            AutoValidateProperties = true;
            Initialize();
        }

        /// <summary>Occurs when the validation errors have changed for a property or for the entire entity. </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>Gets a value indicating whether the entity has validation errors. </summary>
        /// <returns>True if the entity currently has validation errors; otherwise, false. </returns>
        [XmlIgnore]
        public bool HasErrors
        {
            get
            {
                lock (Lock)
                    return _errors.Any(p => p.Value != null && p.Value.Count > 0);
            }
        }

        /// <summary>Gets the properties with validation errors.</summary>
        [XmlIgnore]
        public IEnumerable<string> InvalidProperties
        {
            get
            {
                lock (Lock)
                {
                    return _errors
                      .Where(p => p.Value != null && p.Value.Count > 0 && p.Key != string.Empty)
                      .Select(p => p.Key);
                }
            }
        }

        /// <summary>Gets the invariant object errors.</summary>
        public IEnumerable<string> InvariantErrors
        {
            get { return GetErrors(string.Empty); }
        }

        /// <summary>Gets or sets a value indicating whether the properties are automatically validated.</summary>
        /// <remarks>After enabling automatic validation you should call <see cref="Validate"/> to updated the validation errors. </remarks>
        public bool AutoValidateProperties { get; set; }

        /// <summary>Gets the validation errors for a specified property or for the entire entity. </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or entity. </returns>
        public IEnumerable<string> GetErrors(string propertyName)
        {
            if (propertyName == null)
                return null;

            lock (Lock)
            {
                ICollection<string> errorsForName;
                if (_errors.TryGetValue(propertyName, out errorsForName))
                    return errorsForName;
            }

            return null;
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return GetErrors(propertyName);
        }

        /// <summary>Validates all properties and the object invariants.</summary>
        public void Validate()
        {
            foreach (PropertyInfo property in GetType().GetRuntimeProperties().Where(p => p.CanRead && !NotValidatedProperties.Contains(p.Name)))
            {
                var errors = ValidateProperty(property.Name);
                SetPropertyErrors(property.Name, errors);
            }

            var invariantErrors = ValidateInvariants();
            SetPropertyErrors(string.Empty, invariantErrors);
        }

        /// <summary>Gets the lock object for synchronizing this object. </summary>
        protected object Lock
        {
            get { return _lock; }
        }

        /// <summary>Gets a set with properties which will not be validated. </summary>
        protected HashSet<string> NotValidatedProperties { get; private set; }

        /// <summary>Validates the given property.</summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The list of validation errors (never <c>null</c>).</returns>
        protected virtual ICollection<string> ValidateProperty(string propertyName, object value)
        {
            var validationResults = CreateValidationResults();
            var validationContext = CreateValidationContext(propertyName);

            InvokeTryValidatePropertyMethod(value, validationContext, validationResults);

            return validationResults
                .OfType<object>()
                .Select(result =>
                {
                    dynamic dynamicResult = result;
                    return (string)dynamicResult.ErrorMessage;
                })
                .ToList();
        }

        /// <summary>Validates the invariants.</summary>
        /// <returns>The list of validation errors (never <c>null</c>). </returns>
        protected virtual ICollection<string> ValidateInvariants()
        {
            return new List<string>();
        }

        /// <summary>Called when a property needs to be validated. </summary>
        /// <param name="sender">The sender. </param>
        /// <param name="args">The arguments. </param>
        protected virtual void OnValidateProperty(object sender, PropertyChangedEventArgs args)
        {
            if (AutoValidateProperties &&
                !args.IsProperty<ValidatedObservableObject>(o => o.HasErrors) &&
                !args.IsProperty<ValidatedObservableObject>(o => o.InvalidProperties) &&
                !args.IsProperty<ValidatedObservableObject>(o => o.InvariantErrors))
            {
                var propertyErrors = ValidateProperty(args.PropertyName);
                SetPropertyErrors(args.PropertyName, propertyErrors);

                var invariantErrors = ValidateInvariants();
                SetPropertyErrors(string.Empty, invariantErrors);
            }
        }

        /// <summary>Called when the object has been deserialized.</summary>
        /// <param name="context">The context.</param>
        [OnDeserialized]
        protected virtual void OnDeserialized(StreamingContext context)
        {
            Initialize();
            Validate();
        }

        /// <summary>Validates a property. </summary>
        /// <param name="propertyName">The property name. </param>
        /// <returns>The validation errors. </returns>
        protected ICollection<string> ValidateProperty(string propertyName)
        {
            var value = GetType().GetRuntimeProperty(propertyName).GetValue(this);
            return ValidateProperty(propertyName, value);
        }

        private void Initialize()
        {
            lock (Lock)
                _errors = new Dictionary<string, ICollection<string>>();

            PropertyChanged += (sender, args) =>
            {
                if (AutoValidateProperties &&
                  !args.IsProperty<ValidatedObservableObject>(o => o.HasErrors) &&
                  !args.IsProperty<ValidatedObservableObject>(o => o.InvalidProperties) &&
                  !args.IsProperty<ValidatedObservableObject>(o => o.InvariantErrors))
                {
                    OnValidateProperty(sender, args);
                }
            };
        }

        /// <summary>Sets the errors for the given property. </summary>
        /// <param name="propertyName">The property name. </param>
        /// <param name="validationErrors">The validation errors. </param>
        protected void SetPropertyErrors(string propertyName, ICollection<string> validationErrors)
        {
            lock (Lock)
            {
                if (validationErrors.Count == 0)
                {
                    if (!_errors.ContainsKey(propertyName))
                        return;
                    _errors.Remove(propertyName);
                }
                else
                    _errors[propertyName] = validationErrors;
            }

            RaiseErrorsChanged(propertyName);
            if (propertyName == string.Empty)
                RaisePropertyChanged(() => InvariantErrors);
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            if (handler != null)
                handler(this, new DataErrorsChangedEventArgs(propertyName));

            RaisePropertyChanged(() => HasErrors);
            RaisePropertyChanged(() => InvalidProperties);
        }

        #region Reflection

        private static readonly MethodInfo TryValidatePropertyMethod;
        private static readonly ConstructorInfo ValidationResultsConstructor;
        private static readonly ConstructorInfo ValidationContextConstructor;

        static ValidatedObservableObject()
        {
            try
            {
                var validationContextType = Type.GetType(
                    "System.ComponentModel.DataAnnotations.ValidationContext, " +
                    "System.ComponentModel.DataAnnotations, Version=4.0.0.0, " +
                    "Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                var validatorType = Type.GetType(
                    "System.ComponentModel.DataAnnotations.Validator, " +
                    "System.ComponentModel.DataAnnotations, Version=4.0.0.0, " +
                    "Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                var validationResultType = Type.GetType(
                    "System.ComponentModel.DataAnnotations.ValidationResult, " +
                    "System.ComponentModel.DataAnnotations, Version=4.0.0.0, " +
                    "Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                var validationResultsType = typeof(List<>).MakeGenericType(validationResultType);

                TryValidatePropertyMethod = validatorType.GetRuntimeMethods()
                    .Single(m => m.Name == "TryValidateProperty" && m.GetParameters().Length == 3);

                ValidationResultsConstructor = validationResultsType.GetTypeInfo().
                    DeclaredConstructors.Single(m => m.IsConstructor && m.GetParameters().Length == 0);
                ValidationContextConstructor = validationContextType.GetTypeInfo().
                    DeclaredConstructors.Single(m => m.IsConstructor && m.GetParameters().Length == 1);
            }
            catch (Exception exception)
            {
                throw new NotSupportedException("The ValidatedObservableObject class cannot be used with the current .NET framework. ", exception);
            }
        }

        private object CreateValidationContext(string propertyName)
        {
            dynamic validationContext = ValidationContextConstructor.Invoke(new object[] { this });
            if (propertyName != null)
                validationContext.MemberName = propertyName;
            return validationContext;
        }

        private static IList CreateValidationResults()
        {
            return (IList)ValidationResultsConstructor.Invoke(new object[] { });
        }

        private static bool InvokeTryValidatePropertyMethod(object value, dynamic validationContext, IList validationResults)
        {
            return (bool)TryValidatePropertyMethod.Invoke(null, new object[] { value, validationContext, validationResults });
        }

        #endregion
    }
}