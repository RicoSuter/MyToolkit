//-----------------------------------------------------------------------
// <copyright file="ForkActivity.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Xml.Serialization;

namespace MyToolkit.WorkflowEngine.Activities
{
    /// <summary>Activity for which forking is allowed. Only subclasses of this class are allowed to have multiple outbound transitions. </summary>
    [XmlType("Fork")]
    public class ForkActivity : AutomaticWorkflowActivityBase
    {
    }
}