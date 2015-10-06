//-----------------------------------------------------------------------
// <copyright file="PropertyChangedEventArgsExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MyToolkit.Model
{
    /// <summary>Provides extension methods to check <see cref="PropertyChangedEventArgs"/> in a type-safe manner using lambdas. </summary>
    public static class PropertyChangedEventArgsExtensions
    {
        /// <summary>Checks whether a given property has changed. </summary>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/>. </param>
        /// <param name="propertyName">The property name. </param>
        /// <returns><c>true</c> if the property names match; otherwise, <c>false</c>. </returns>
        [Obsolete("Use string compare directly. 5/20/2014")]
        public static bool IsProperty(this PropertyChangedEventArgs args, string propertyName)
        {
            return args.PropertyName == propertyName;
        }

        /// <summary>Checks whether a property given as lambda has changed. </summary>
        /// <typeparam name="TObject">The type of the class with the property. </typeparam>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/>. </param>
        /// <param name="expression">The property name as lambda. </param>
        /// <returns><c>true</c> if the property names match; otherwise, <c>false</c>. </returns>
        public static bool IsProperty<TObject>(this PropertyChangedEventArgs args, Expression<Func<TObject, object>> expression)
        {
            var memexp = expression.Body is UnaryExpression ?
                (MemberExpression)((UnaryExpression)expression.Body).Operand : ((MemberExpression)expression.Body);
            return args.PropertyName == memexp.Member.Name;
        }
    }
}