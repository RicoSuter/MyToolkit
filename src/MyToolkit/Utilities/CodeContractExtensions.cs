//-----------------------------------------------------------------------
// <copyright file="CodeContractExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MyToolkit.Utilities
{
    /// <summary>Provides extension methods for Code Contracts and method parameter validation. </summary>
    public static class CodeContractExtensions
    {
        /// <summary>Throws an <see cref="ArgumentNullException" /> if the value is <c>null</c> or <c>string.Empty</c>.</summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The value cannot be null or empty.</exception>
        /// <remarks>The method throws a <see cref="ArgumentNullException" /> and also defines Contract.Requires statements for static analysis.</remarks>
        [DebuggerStepThrough]
#if !LEGACY
        [ContractAbbreviator]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void CheckNotNullOrEmpty([ValidatedNotNullAttribute] this string value, string parameterName)
        {
#if !LEGACY
            Contract.Requires<ArgumentNullException>(value != null, "The value '" + parameterName + "' cannot be null. ");
            Contract.Requires<ArgumentNullException>(value != string.Empty, "The string '" + parameterName + "' cannot be empty. ");
#endif

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>Throws an <see cref="ArgumentNullException" /> if the value is <c>null</c>.</summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The value cannot be null.</exception>
        /// <remarks>The method throws a <see cref="ArgumentNullException" /> and also defines a Contract.Requires statement for static analysis.</remarks>
        [DebuggerStepThrough]
#if !LEGACY
        [ContractAbbreviator]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void CheckNotNull([ValidatedNotNullAttribute] this object value, string parameterName)
        {
#if !LEGACY
            Contract.Requires<ArgumentNullException>(value != null, "The value '" + parameterName + "' cannot be null. ");
#endif

            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>Throws an <see cref="ArgumentNullException" /> if the value is <c>null</c>.</summary>
        /// <typeparam name="T">The type of the value. </typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The value cannot be null.</exception>
        /// <remarks>The method throws a <see cref="ArgumentNullException" /> and also defines a Contract.Requires statement for static analysis.</remarks>
        [DebuggerStepThrough]
#if !LEGACY
        [ContractAbbreviator]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void CheckNotNull<T>([ValidatedNotNullAttribute] this T? value, string parameterName)
            where T : struct
        {
            // This method is additionally needed so that nullable objects are not boxed to object
#if !LEGACY
            Contract.Requires<ArgumentNullException>(value != null, "The value '" + parameterName + "' cannot be null. ");
#endif

            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>Attribute is used to avoid (FxCop/VS) CA1062 errors.</summary>
        /// <remarks>See http://geekswithblogs.net/terje/archive/2010/10/14.aspx. </remarks>
        [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
        public sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
