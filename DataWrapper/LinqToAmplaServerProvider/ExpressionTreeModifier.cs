using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SE.MESCC.DAL.DataWrapper.LinqToAmplaServerProvider
{
    internal class ExpressionTreeModifier<TData> : ExpressionVisitor
    {
        private IQueryable<BaseRecords.BaseRecord> queryablePlaces;

        internal ExpressionTreeModifier(IQueryable<BaseRecords.BaseRecord> places)
        {
            this.queryablePlaces = places;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            // Replace the constant QueryableTerraServerData arg with the queryable Place collection. 
            if (c.Type == typeof(QueryableAmplaServerData<TData>))
                return Expression.Constant(this.queryablePlaces);
            else
                return c;
        }
    }
}
