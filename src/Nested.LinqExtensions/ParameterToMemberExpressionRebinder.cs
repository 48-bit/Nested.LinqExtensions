using System;
using System.Linq.Expressions;

namespace Nested.LinqExtensions
{
    /// <summary>
    /// Rebind parameters given from parameter expression to MemberExpression.
    /// Useful to create 'navigate' expressions,
    ///     where first expression represents property selector and second is lambda predicate.
    /// </summary>
    public class ParameterToMemberExpressionRebinder : ExpressionVisitor
    {
        ParameterExpression paramExpr;
        MemberExpression memberExpr;

        private ParameterToMemberExpressionRebinder(ParameterExpression paramExpr, MemberExpression memberExpr)
        {
            this.paramExpr = paramExpr;
            this.memberExpr = memberExpr;
        }

        /// <summary>
        /// Pass property selector expression as parameter to predicate with type of given property.
        /// </summary>
        /// <param name="propertySelector">Property selector lambda expression.</param>
        /// <param name="propertyPredicate">Predicate lambda expression which uses given property. </param>
        /// <typeparam name="T">Type of source object from where property is selected. </typeparam>
        /// <typeparam name="T2">Type of property returned by property selector and used by predicate. </typeparam>
        /// <returns>Predicate lambda expression with given property as parameter. </returns>
        public static Expression<Func<T, bool>> CombinePropertySelectorWithPredicate<T, T2>(
            Expression<Func<T, T2>> propertySelector,
            Expression<Func<T2, bool>> propertyPredicate)
        {
            return NavigatePropertySelectorsToNextLambda(propertySelector, propertyPredicate);
        }

        /// <summary>
        /// Pass property selector expression as parameter to lambda expression with single parameter of selected property type.
        /// </summary>
        /// <param name="propertySelector">Property selector lambda expression.</param>
        /// <param name="lambdaExpression">Lambda expression taking given property as parameter. </param>
        /// <typeparam name="TSource">>Type of source object from where property is selected.</typeparam>
        /// <typeparam name="TNavigate">Type of property returned by property selector and used by lambda expression. </typeparam>
        /// <typeparam name="TResult">Type of return value for lambda expression. </typeparam>
        /// <returns>Lambda expression with given property as parameter.</returns>
        public static Expression<Func<TSource, TResult>> NavigatePropertySelectorsToNextLambda<TSource, TNavigate, TResult>(
            Expression<Func<TSource, TNavigate>> propertySelector, Expression<Func<TNavigate, TResult>> lambdaExpression)
        {
            var memberExpression = propertySelector.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("propertySelector");
            }

            var expr = Expression.Lambda<Func<TSource, TResult>>(lambdaExpression.Body, propertySelector.Parameters);
            var rebinder = new ParameterToMemberExpressionRebinder(lambdaExpression.Parameters[0], memberExpression);
            expr = (Expression<Func<TSource, TResult>>)rebinder.Visit(expr);

            return expr;
        }

        /// <inheritdoc />
        public override Expression Visit(Expression p)
        {
            return base.Visit(p == this.paramExpr ? this.memberExpr : p);
        }

    }
}