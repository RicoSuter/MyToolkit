//-----------------------------------------------------------------------
// <copyright file="ExtendedPropertyChangedEventArgs.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace MyToolkit.Model
{
    public class ExtendedPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public ExtendedPropertyChangedEventArgs(string propertyName, object oldValue, object newValue) : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>Gets the old value of the changed property. </summary>
        public object OldValue { get; private set; }

        /// <summary>Gets the new value of the changed property. </summary>
        public object NewValue { get; private set; }
    }
}