//-----------------------------------------------------------------------
// <copyright file="LoadingMessage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Messaging
{
    /// <summary>A message to show or hide a loading progress.</summary>
    public class LoadingMessage
    {
        /// <summary>Initializes a new instance of the <see cref="LoadingMessage"/> class.</summary>
        /// <param name="isLoading">Value indicating whether to show the loading progress.</param>
        public LoadingMessage(bool isLoading)
        {
            IsLoading = isLoading; 
        }

        /// <summary>Gets or sets a value indicating whether to show the loading progress.</summary>
        public bool IsLoading { get; set; }
    }
}
