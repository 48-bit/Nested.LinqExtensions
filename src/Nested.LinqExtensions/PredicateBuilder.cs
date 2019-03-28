using System;
using System.Linq;
using System.Linq.Expressions;

namespace Nested.LinqExtensions
{
    /// <summary>
    ///     Enables the efficient, dynamic composition of query predicates.
    ///     https://petemontgomery.wordpress.com/2011/02/10/a-universal-predicatebuilder/.
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        ///     Creates a predicate that evaluates to true.
        /// </summary>
        /// <typeparam name="T">type of predicate parameter.</typeparam>
        /// <returns>predicate that evaluates to true.</returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return param => true;
        }

        /// <summary>
        ///     Creates a predicate that evaluates to false.
        /// </summary>
        /// <typeparam name="T">type of predicate parameter.</typeparam>
        /// <returns>predicate that evaluates to false.</returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return param => false;
        }

        /// <summary>
        ///     Creates a predicate expression from the specified lambda expression.
        /// </summary>
        /// <param name="predicate">source expression. </param>
        /// <typeparam name="T">type of predicate parameter.</typeparam>
        /// <returns>predicate expression. </returns>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
        {
            return predicate;
        }

        /// <summary>
        ///     Combines the first predicate with the second using the logical "and".
        /// </summary>
        /// <param name="first">first predicate. </param>
        /// <param name="second">second predicate. </param>
        /// <typeparam name="T">type of predicate parameter.</typeparam>
        /// <returns>predicate combined from first and second using the logical "and". </returns>
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        ///     Combines the first predicate with the second using the logical "or".
        /// </summary>
        /// <param name="first">first predicate. </param>
        /// <param name="second">second predicate. </param>
        /// <typeparam name="T">type of predicate parameter.</typeparam>
        /// <returns>predicate combined from first and second using the logical "or". </returns>
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        ///     Negates the predicate.
        /// </summary>
        /// <param name="expression">predicate to invert.</param>
        /// <typeparam name="T">type of predicate parameter. </typeparam>
        /// <returns>inverted predicate with logical "not". </returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        /// Combine property selector expression with predicate,
        /// passing property from selector expression as parameter.
        /// </summary>
        /// <param name="propertySelector">property selector expression.</param>
        /// <param name="propertyPredicate">predicate expression taking property as parameter. </param>
        /// <typeparam name="T1">type of property selector source. </typeparam>
        /// <typeparam name="T2">type of predicate parameter. </typeparam>
        /// <returns>combined predicate expression with property as parameter. </returns>
        public static Expression<Func<T1, bool>> Compose<T1, T2>(
            this Expression<Func<T1, T2>> propertySelector,
            Expression<Func<T2, bool>> propertyPredicate)
        {
            return ParameterToMemberExpressionRebinder.CombinePropertySelectorWithPredicate(
                propertySelector,
                propertyPredicate);
        }

        /// <summary>
        /// Combine property selector expression with lambda expression function,
        /// passing property from selector expression as parameter.
        /// </summary>
        /// <param name="propertySelector">property selector expression.</param>
        /// <param name="lambdaExpression">lambda expression taking property as parameter. </param>
        /// <typeparam name="TSource">type of property selector source. </typeparam>
        /// <typeparam name="TNavigate">type of property and lambda expression parameter. </typeparam>
        /// <typeparam name="TResult">type lambda expression return value. </typeparam>
        /// <returns>combined lambda expression with property as parameter. </returns>
        public static Expression<Func<TSource, TResult>> Navigate<TSource, TNavigate, TResult>(
            this Expression<Func<TSource, TNavigate>> propertySelector, Expression<Func<TNavigate, TResult>> lambdaExpression)
        {
            return ParameterToMemberExpressionRebinder.NavigatePropertySelectorsToNextLambda(propertySelector, lambdaExpression);
        }

        /// <summary>
        ///     Combines the first expression with the second using the specified merge function.
        /// </summary>
        private static Expression<T> Compose<T>(
            this Expression<T> first,
            Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
                .Select((f, i) => new
                {
                    f, s = second.Parameters[i],
                })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }
}