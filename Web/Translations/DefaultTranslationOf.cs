// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MyToolkit.Translations {
	/// <summary>
    /// Simple fluent way to access the default translation map.
    /// </summary>
    /// <typeparam name="T">Class the expression uses.</typeparam>
    public static class DefaultTranslationOf<T> {
        public static CompiledExpression<T, TResult> Property<TResult>(Expression<Func<T, TResult>> property, Expression<Func<T, TResult>> expression) {
            return TranslationMap.defaultMap.Add(property, expression);
        }

        public static IncompletePropertyTranslation<TResult> Property<TResult>(Expression<Func<T, TResult>> property) {
            return new IncompletePropertyTranslation<TResult>(property);
        }

        public static TResult Evaluate<TResult>(T instance, MethodBase method) {
            var compiledExpression = TranslationMap.defaultMap.Get<T, TResult>(method);
            return compiledExpression.Evaluate(instance);
        }

        public class IncompletePropertyTranslation<TResult> {
            private Expression<Func<T, TResult>> property;

            internal IncompletePropertyTranslation(Expression<Func<T, TResult>> property) {
                this.property = property;
            }

            public CompiledExpression<T, TResult> Is(Expression<Func<T, TResult>> expression) {
                return DefaultTranslationOf<T>.Property(property, expression);
            }
        }
    }
}