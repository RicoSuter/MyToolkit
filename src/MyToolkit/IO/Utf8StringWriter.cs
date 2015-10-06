//-----------------------------------------------------------------------
// <copyright file="Utf8StreamWriter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.IO;
using System.Text;

namespace MyToolkit.IO
{
    /// <summary>String writer to write UTF-8. </summary>
    public class Utf8StringWriter : StringWriter
    {
        /// <summary>Gets the encoding. </summary>
        public sealed override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}