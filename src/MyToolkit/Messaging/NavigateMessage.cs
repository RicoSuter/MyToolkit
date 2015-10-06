//-----------------------------------------------------------------------
// <copyright file="NavigateMessage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Messaging
{
    /// <summary>Message to navigate to another page. </summary>
    public class NavigateMessage
    {
        /// <summary>The view model type of the page. </summary>
        public Type ViewModelType { get; private set; }

        /// <summary>The navigation parameter. </summary>
        public object Parameter { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="NavigateMessage"/> class. </summary>
        public NavigateMessage(Type viewModelType)
            : this(viewModelType, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NavigateMessage"/> class. </summary>
        public NavigateMessage(Type viewModelType, params object[] parameters)
            : this(viewModelType, (object)parameters)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NavigateMessage"/> class. </summary>
        public NavigateMessage(Type viewModelType, object parameter)
        {
            ViewModelType = viewModelType;
            Parameter = parameter;
        }
    }
}
