//-----------------------------------------------------------------------
// <copyright file="IEntity.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Utilities
{
    /// <summary>Interface for an entity with an ID. </summary>
    public interface IEntity<TIdentity>
    {
        /// <summary>Gets the ID of the entity. </summary>
        TIdentity Id { get; }
    }
}
