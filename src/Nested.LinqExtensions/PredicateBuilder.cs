using System;
using System.Linq;
using System.Linq.Expressions;

namespace Nested.LinqExtensions
{
    /// <summary>
    ///     Enables the efficient, dynamic composition of query predicates.
    ///     https://petemontgomery.wordpress.com/2011/02/10/a-universal-predicatebuilder/
    /// </summary>
    public static partial class PredicateBuilder
    {
        /// <summary>
        ///     Creates a predicate that evaluates to true.
        /// </summary>
        public static Expression<Func<T, bool>> True<T>()
        {
            return param => true;
        }

        /// <summary>
        ///     Creates a predicate that evaluates to false.
        /// </summary>
        public static Expression<Func<T, bool>> False<T>()
        {
            return param => false;
        }

        /// <summary>
        ///     Creates a predicate expression from the specified lambda expression.
        /// </summary>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
        {
            return predicate;
        }

        /// <summary>
        ///     Combines the first predicate with the second using the logical "and".
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        ///     Combines the first predicate with the second using the logical "or".
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        ///     Negates the predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        ///     Combines the first expression with the second using the specified merge function.
        /// </summary>
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
                .Select((f, i) => new {f, s = second.Parameters[i]})
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T1, bool>> Compose<T1, T2>(this Expression<Func<T1, T2>> propertySelector,
            Expression<Func<T2, bool>> propertyPredicate)
        {
            return ParameterToMemberExpressionRebinder.CombinePropertySelectorWithPredicate(propertySelector,
                propertyPredicate);
        }

        public static Expression<Func<TSource, TResult>> Navigate<TSource, TNavigate, TResult>(
            this Expression<Func<TSource, TNavigate>> first, Expression<Func<TNavigate, TResult>> second)
        {
            return ParameterToMemberExpressionRebinder.NavigatePropertySelectorsToNextLambda(first, second);
        }
    }
}