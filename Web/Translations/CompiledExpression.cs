// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Linq.Expressions;

namespace MyToolkit.Translations {
	/// <summary>
    /// Provides the common boxed version of get.
    /// </summary>
    public abstract class CompiledExpression {
        internal abstract LambdaExpression BoxedGet { get; }
    }

    /// <summary>
    /// Represents an expression and its compiled function.
    /// </summary>
    /// <typeparam name="TClass">Class the expression relates to.</typeparam>
    /// <typeparam name="TProperty">Return type of the expression.</typeparam>
    public sealed class CompiledExpression<T, TResult> : CompiledExpression {
        private Expression<Func<T, TResult>> expression;
        private Func<T, TResult> function;

        public CompiledExpression() {
        }

        public CompiledExpression(Expression<Func<T, TResult>> expression) {
            this.expression = expression;
            function = expression.Compile();
        }

        public TResult Evaluate(T instance) {
            return function(instance);
        }

        internal override LambdaExpression BoxedGet {
            get { return expression; }
        }
    }
}