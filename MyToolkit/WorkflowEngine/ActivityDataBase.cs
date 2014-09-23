//-----------------------------------------------------------------------
// <copyright file="ActivityDataBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Xml.Serialization;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>Contains activity data. </summary>
    public class ActivityDataBase
    {
        /// <summary>Gets or sets the data group (only one data object per type and group allowed). </summary>
        [XmlAttribute]
        public string Group { get; set; }
    }
}