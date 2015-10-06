//-----------------------------------------------------------------------
// <copyright file="GraphPropertyChangedEventArgs.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace MyToolkit.Model
{
    /// <summary>The argument of the ExtendedPropertyChanged event. </summary>
    public class GraphPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="GraphPropertyChangedEventArgs"/> class. </summary>
        public GraphPropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
            : base(propertyName)
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