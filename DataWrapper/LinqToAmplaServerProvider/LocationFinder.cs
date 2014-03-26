using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SE.MESCC.DAL.DataWrapper.LinqToAmplaServerProvider
{
    internal class LocationFinder : ExpressionVisitor
    {
        private Expression expression;
        private List<string> locations;


         public LocationFinder(Expression exp)
        {
            this.expression = exp;
        }

        public List<string> Locations
        {
            get
            {
                if (locations == null)
                {
                    locations = new List<string>();
                    this.Visit(this.expression);
                }
                return this.locations;
            }
        }

 
        protected override Expression VisitBinary(BinaryExpression be) 
        {
            if (be.NodeType == ExpressionType.Equal)
            {
                if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(BaseRecords.BaseRecord), "Location"))
                {
                    locations.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(BaseRecords.BaseRecord), "Location"));
                    return be;
                }
                else
                    return base.VisitBinary(be);
            }
            else
                return base.VisitBinary(be);
        }
    }
}
