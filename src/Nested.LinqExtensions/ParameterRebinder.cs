using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nested.LinqExtensions
{
    /// <summary>
    /// Rebind expression parameters from one ParameterExpression to another.
    /// To use for combining expressions.
    /// </summary>
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// Rebind expression parameters from one ParameterExpression to another.
        /// </summary>
        /// <param name="map">parameter expression map to rebind source parameters to target.
        /// source expression as keys, target expressions as values. </param>
        /// <param name="exp">parent (lambda) expression to replace parameters in it. </param>
        /// <returns>Expression with replaced parameters. </returns>
        public static Expression ReplaceParameters(
            Dictionary<ParameterExpression, ParameterExpression> map,
            Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (this.map.TryGetValue(p, out var replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }
}