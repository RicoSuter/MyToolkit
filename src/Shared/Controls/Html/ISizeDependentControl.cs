//-----------------------------------------------------------------------
// <copyright file="ISizeDependentControl.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Controls.Html
{
    /// <summary>The interface of a size independent control.</summary>
    public interface ISizeDependentControl
	{
        /// <summary>Called when the HTML view's with changed and the size dependent control should fit the new width.</summary>
        /// <param name="actualWidth">The actual width of the HTML view.</param>
        void Update(double actualWidth);
	}
}