//-----------------------------------------------------------------------
// <copyright file="WorkflowDataProvider.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using MyToolkit.WorkflowEngine.Activities;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>Provides data objects for a workflow instance. </summary>
    public class WorkflowDataProvider
    {
        /// <summary>Initializes a new instance of the <see cref="WorkflowDataProvider"/> class. </summary>
        public WorkflowDataProvider()
        {
            Data = new List<ActivityDataBase>();
        }

        [XmlElement("ActivityData")]
        public List<ActivityDataBase> Data { get; set; }

        /// <summary>Resolves the requested data object. </summary>
        /// <typeparam name="T">The type of the data object. </typeparam>
        /// <param name="activity">The current activity to load the used data group from. </param>
        /// <returns>The data object. </returns>
        public T Resolve<T>(WorkflowActivityBase activity) where T : ActivityDataBase, new()
        {
            return Resolve<T>(activity.Group);
        }

        /// <summary>Resolves the requested data object. </summary>
        /// <typeparam name="T">The type of the data object. </typeparam>
        /// <param name="group">The data group. Only use activity properties as parameter (no hard-coded strings). </param>
        /// <returns>The data object. </returns>
        public T Resolve<T>(string group) where T : ActivityDataBase, new()
        {
            var data = Data.SingleOrDefault(d => d.Group == group && d.GetType() == typeof(T));
            if (data == null)
            {
                data = new T();
                data.Group = group;

                Data.Add(data);
            }

            return (T)data; 
        }
    }
}