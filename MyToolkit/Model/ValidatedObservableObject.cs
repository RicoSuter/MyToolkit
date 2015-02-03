//-----------------------------------------------------------------------
// <copyright file="ValidationObject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
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
        private static readonly MethodInfo TryValidatePropertyMethod;
        private static readonly MethodInfo TryValidateObjectMethod;

        private static readonly ConstructorInfo ValidationResultsConstructor;
        private static readonly ConstructorInfo ValidationContextConstructor;

        private Dictionary<string, ICollection<string>> _errors;
        private bool _autoValidateProperties = true;
        private bool _raiseErrorsChanged = true;

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
                TryValidateObjectMethod = validatorType.GetRuntimeMethods()
                    .Single(m => m.Name == "TryValidateObject" && m.GetParameters().Length == 4);

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

        /// <summary>Initializes a new instance of the <see cref="ValidatedObservableObject"/> class.</summary>
        public ValidatedObservableObject()
        {
            _errors = new Dictionary<string, ICollection<string>>();
        }

        /// <summary>Occurs when the validation errors have changed for a property or for the entire entity. </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>Gets a value indicating whether the entity has validation errors. </summary>
        /// <returns>True if the entity currently has validation errors; otherwise, false. </returns>
        [XmlIgnore]
        public bool HasErrors
        {
            get { return _errors.Any(p => p.Value != null && p.Value.Count > 0); }
        }

        /// <summary>Gets the properties with validation errors.</summary>
        [XmlIgnore]
        public IEnumerable<string> InvalidProperties
        {
            get
            {
                return _errors
                  .Where(p => p.Value != null && p.Value.Count > 0)
                  .Select(p => p.Key);
            }
        }

        /// <summary>Gets or sets a value indicating whether the properties are automatically validated.</summary>
        /// <remarks>When enabling the automatic validation, the whole object is validated immediately. </remarks>
        public bool AutoValidateProperties
        {
            get { return _autoValidateProperties; }
            set
            {
                if (_autoValidateProperties != value)
                {
                    _autoValidateProperties = value;
                    if (_autoValidateProperties)
                        ValidateObject();
                }
            }
        }

        /// <summary>Gets the validation errors for a specified property or for the entire entity. </summary>
        /// <returns>The validation errors for the property or entity. </returns>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.</param>
        public ICollection<string> GetErrors(string propertyName)
        {
            if (propertyName == null)
                return null;

            ICollection<string> errorsForName;
            if (_errors.TryGetValue(propertyName, out errorsForName))
                return errorsForName;

            return null;
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return GetErrors(propertyName);
        }

        /// <summary>Validates the object.</summary>
        public void ValidateObject()
        {
            _raiseErrorsChanged = false;
            try
            {
                foreach (var property in GetType().GetRuntimeProperties().Where(p => p.CanRead))
                {
                    var errors = ValidateProperty(property.Name);
                    SetPropertyErrors(property.Name, errors);
                }
            }
            finally
            {
                _raiseErrorsChanged = true;
            }
            RaiseErrorsChanged(string.Empty);
        }

        /// <summary>Validates the given property.</summary>
        /// <param name="propertyName">The name of the property.</param>
        public virtual ICollection<string> ValidateProperty(string propertyName)
        {
            var value = GetType().GetRuntimeProperty(propertyName).GetValue(this);

            var validationResults = CreateValidationResults();
            var validationContext = CreateValidationContext(propertyName);

            InvokeTryValidatePropertyMethod(value, validationContext, validationResults);

            return validationResults
                .OfType<object>()
                .Select(r =>
                {
                    dynamic dyn = r;
                    return (string)dyn.ErrorMessage;
                })
                .ToList();
        }

        /// <summary>Raises the property changed event. </summary>
        /// <param name="args">The arguments. </param>
        protected override void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            if (AutoValidateProperties)
            {
                var errors = ValidateProperty(args.PropertyName);
                SetPropertyErrors(args.PropertyName, errors);
            }

            base.RaisePropertyChanged(args);
        }

        /// <summary>Called when the object has been deserialized.</summary>
        /// <param name="context">The context.</param>
        [OnDeserialized]
        protected virtual void OnDeserialized(StreamingContext context)
        {
            _errors = new Dictionary<string, ICollection<string>>();
            ValidateObject();
        }

        private void SetPropertyErrors(string propertyName, ICollection<string> validationResults)
        {
            if (validationResults.Count == 0)
                _errors.Remove(propertyName);
            else
                _errors[propertyName] = validationResults;

            RaiseErrorsChanged(propertyName);
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            if (!_raiseErrorsChanged)
                return;

            var handler = ErrorsChanged;
            if (handler != null)
                handler(this, new DataErrorsChangedEventArgs(propertyName));
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

        private bool InvokeTryValidateObjectMethod(object validationContext, IList validationResults)
        {
            return (bool)TryValidateObjectMethod.Invoke(null, new[] { this, validationContext, validationResults, true });
        }
    }
}