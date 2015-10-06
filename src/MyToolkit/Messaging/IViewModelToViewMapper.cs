//-----------------------------------------------------------------------
// <copyright file="IViewModelToViewMapper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Messaging
{
    /// <summary>Maps view model types to view types. </summary>
    public interface IViewModelToViewMapper
    {
        /// <summary>Maps a view model type to its view type. </summary>
        /// <param name="viewModelType">The view model type. </param>
        /// <returns>The view type. </returns>
        Type Map(Type viewModelType);
    }
}