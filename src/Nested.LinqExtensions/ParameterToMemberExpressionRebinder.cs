using System;
using System.Linq.Expressions;

namespace Nested.LinqExtensions
{
    public class ParameterToMemberExpressionRebinder : ExpressionVisitor
    {
        ParameterExpression _paramExpr;
        MemberExpression _memberExpr;

        ParameterToMemberExpressionRebinder(ParameterExpression paramExpr, MemberExpression memberExpr)
        {
            _paramExpr = paramExpr;
            _memberExpr = memberExpr;
        }

        public override Expression Visit(Expression p)
        {
            return base.Visit(p == _paramExpr ? _memberExpr : p);
        }

        public static Expression<Func<T, bool>> CombinePropertySelectorWithPredicate<T, T2>(
            Expression<Func<T, T2>> propertySelector,
            Expression<Func<T2, bool>> propertyPredicate)
        {
            return NavigatePropertySelectorsToNextLambda<T, T2, bool>(propertySelector, propertyPredicate);
        }

        public static Expression<Func<TSource, TResult>> NavigatePropertySelectorsToNextLambda<TSource, TNavigate, TResult>(
            Expression<Func<TSource, TNavigate>> first, Expression<Func<TNavigate, TResult>> second)
        {
            var memberExpression = first.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("propertySelector");
            }

            var expr = Expression.Lambda<Func<TSource, TResult>>(second.Body, first.Parameters);
            var rebinder = new ParameterToMemberExpressionRebinder(second.Parameters[0], memberExpression);
            expr = (Expression<Func<TSource, TResult>>)rebinder.Visit(expr);

            return expr;
        }
    }
}