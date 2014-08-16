//-----------------------------------------------------------------------
// <copyright file="ValidationObject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Serialization;

//namespace MyToolkit.MVVM
//{
//    public class ValidationObject : NotifyPropertyChanged, INotifyDataErrorInfo
//    {
//        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

//        /// <summary>Initializes a new instance of the <see cref="ValidationObject"/> class.</summary>
//        public ValidationObject()
//        {
//            _errors = new Dictionary<string, List<string>>();
//            Validate();
//        }

//        /// <summary>
//        /// Occurs when the validation errors have changed for a property or for the entire entity. 
//        /// </summary>
//        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

//        /// <summary>Gets a value indicating whether the entity has validation errors. </summary>
//        /// <returns>True if the entity currently has validation errors; otherwise, false. </returns>
//        [XmlIgnore]
//        public bool HasErrors
//        {
//            get { return InvalidProperties.Any(); }
//        }

//        /// <summary>Gets the invalid properties.</summary>
//        [XmlIgnore]
//        public virtual IEnumerable<string> InvalidProperties
//        {
//            get
//            {
//                return _errors
//                    .Where(p => p.Value != null && p.Value.Count > 0)
//                    .Select(p => p.Key);
//            }
//        }

//        /// <summary>Gets the validation errors for a specified property or for the entire entity. </summary>
//        /// <returns>The validation errors for the property or entity. </returns>
//        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.</param>
//        public IEnumerable GetErrors(string propertyName)
//        {
//            if (propertyName == null)
//                return null;

//            List<string> errorsForName;
//            _errors.TryGetValue(propertyName, out errorsForName);
//            return errorsForName;
//        }

//        /// <summary>
//        /// Raises a property changed event.  
//        /// </summary>
//        /// <param name="propertyName">The property name.</param>
//        public override async void RaisePropertyChanged(string propertyName = null)
//        {
//            base.RaisePropertyChanged(propertyName);

//            Validator.ValidateProperty(value,
//                    new ValidationContext(this, null, null) { MemberName = "LastName" });
                
//        }

//        [OnDeserialized]
//        protected virtual void OnDeserialized(StreamingContext context)
//        {
//            _errors = new Dictionary<string, List<string>>();
//        }

//        private void RaiseErrorsChanged(string propertyName)
//        {
//            var handler = ErrorsChanged;
//            if (handler != null)
//                handler(this, new DataErrorsChangedEventArgs(propertyName));
//        }
//    }
//}
