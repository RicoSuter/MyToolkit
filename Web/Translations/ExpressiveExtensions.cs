// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MyToolkit.Translations {
	/// <summary>
    /// Extension methods over IQueryable to turn on expression translation via a
    /// specified or default TranslationMap.
    /// </summary>
    public static class ExpressiveExtensions {
		public static T[] ToArrayWithTranslations<T>(this IQueryable<T> source)
		{
			return source.Provider.CreateQuery<T>(WithTranslations(source.Expression)).ToArray();
		}

		public static IList<T> ToListWithTranslations<T>(this IQueryable<T> source)
		{
			return source.Provider.CreateQuery<T>(WithTranslations(source.Expression)).ToList();
		}

        public static IQueryable<T> WithTranslations<T>(this IQueryable<T> source) {            
            return source.Provider.CreateQuery<T>(WithTranslations(source.Expression));
        }

        public static IQueryable<T> WithTranslations<T>(this IQueryable<T> source, TranslationMap map) {
            return source.Provider.CreateQuery<T>(WithTranslations(source.Expression, map));
        }

        public static Expression WithTranslations(Expression expression) {
            return WithTranslations(expression, TranslationMap.defaultMap);
        }

        public static Expression WithTranslations(Expression expression, TranslationMap map) {
            return new TranslatingVisitor(map).Visit(expression);
        }

        private static void EnsureTypeInitialized(Type type) {
            try {
                // Ensure the static members are accessed class' ctor
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
            catch (TypeInitializationException) {

            }
        }

        /// <summary>
        /// Extends the expression visitor to translate properties to expressions
        /// according to the provided translation map.
        /// </summary>
        private class TranslatingVisitor : ExpressionVisitor {
            private readonly Stack<KeyValuePair<ParameterExpression, Expression>> bindings = new Stack<KeyValuePair<ParameterExpression, Expression>>();
            private readonly TranslationMap map;

            internal TranslatingVisitor(TranslationMap map) {
                this.map = map;
            }

            protected override Expression VisitMember(MemberExpression node) {
                EnsureTypeInitialized(node.Member.DeclaringType);

                CompiledExpression cp;
                if (map.TryGetValue(node.Member, out cp)) {
                    return VisitCompiledExpression(cp, node.Expression);
                }

                if (typeof(CompiledExpression).IsAssignableFrom(node.Member.DeclaringType)) {
                    return VisitCompiledExpression(cp, node.Expression);
                }

                return base.VisitMember(node);
            }

            private Expression VisitCompiledExpression(CompiledExpression ce, Expression expression) {
                bindings.Push(new KeyValuePair<ParameterExpression, Expression>(ce.BoxedGet.Parameters.Single(), expression));
                var body = Visit(ce.BoxedGet.Body);
                bindings.Pop();
                return body;
            }

            protected override Expression VisitParameter(ParameterExpression p) {
                var binding = bindings.Where(b => b.Key == p).FirstOrDefault();
                return (binding.Value == null) ? base.VisitParameter(p) : Visit(binding.Value);
            }
        }
    }
}
