//-----------------------------------------------------------------------
// <copyright file="RegexViewModelToViewMapper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MyToolkit.Messaging
{
    /// <summary>Maps view model types to view types using a regular expressions. </summary>
    public class RegexViewModelToViewMapper : IViewModelToViewMapper
    {
        private readonly Regex _viewModelTypeNameRegex;
        private readonly string _viewTypeName;
        private readonly Assembly _assembly;

        /// <summary>Creates a default mapper where '{0}.ViewModels.{1}Model' is mapped to '{0}.Views.{1}'. </summary>
        /// <param name="assembly">The assembly containing the views. </param>
        /// <returns>The <see cref="RegexViewModelToViewMapper"/>. </returns>
        public static RegexViewModelToViewMapper CreateDefaultMapper(Assembly assembly)
        {
            return new RegexViewModelToViewMapper("{0}.ViewModels.{1}Model", "{0}.Views.{1}", assembly);
        }

        /// <summary>Initializes a new instance of the <see cref="RegexViewModelToViewMapper"/> class. </summary>
        /// <param name="viewModelTypeName">The match string for the view model type (e.g. '{0}.ViewModels.{1}Model'). </param>
        /// <param name="viewTypeName">The match string for the view type (e.g. '{0}.Views.{1}'). </param>
        /// <param name="assembly">The assembly containing the views. </param>
        public RegexViewModelToViewMapper(string viewModelTypeName, string viewTypeName, Assembly assembly)
        {
            _assembly = assembly;
            _viewTypeName = viewTypeName;
            _viewModelTypeNameRegex = new Regex(viewModelTypeName
                .Replace("{0}", "(.*?)")
                .Replace("{1}", "(.*?)"));
        }

        /// <summary>Maps a view model type to its view type. </summary>
        /// <param name="viewModelType">The view model type. </param>
        /// <returns>The view type. </returns>
        public Type Map(Type viewModelType)
        {
            var match = _viewModelTypeNameRegex.Match(viewModelType.FullName);
            var viewTypeName = string.Format(_viewTypeName, match.Groups[1].Value, match.Groups[2].Value);
            return _assembly.GetType(viewTypeName);
        }
    }
}