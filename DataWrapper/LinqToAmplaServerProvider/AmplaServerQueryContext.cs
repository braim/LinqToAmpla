using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SE.MESCC.DAL.DataWrapper.LinqToAmplaServerProvider
{
    interface IExecutionContext
    {
        object Execute(Expression expression, bool IsEnumerable, WebReferences.DataWS.AmplaModules module);
    }
    class AmplaServerQueryContext<TData> :IExecutionContext where TData : BaseRecords.BaseRecord,new()
    {
        public ExpressionTreeModifier<TData> GetExpressionTreeModifier(IQueryable<TData> p)
        {
            return new ExpressionTreeModifier<TData>(p);
        }
        public  object Execute(Expression expression, bool IsEnumerable,WebReferences.DataWS.AmplaModules module)
        {
            // The expression must represent a query over the data source. 
            if (!IsQueryOverDataSource(expression))
                throw new InvalidProgramException("No query over the data source was specified.");

            // Find the call to Where() and get the lambda expression predicate.
            InnermostWhereFinder whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);
            LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            // Send the lambda expression through the partial evaluator.
            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            // Get the place name(s) to query the Web service with.
            LocationFinder lf = new LocationFinder(lambdaExpression.Body);
            List<string> locations = lf.Locations;

            if (locations.Count != 1)
                throw new InvalidQueryException("You must specify exactly one location in your query.");

            
            // Call the Web service and get the results.
           TData[] places = new WebServiceHelper(locations[0], module).GetRecords<TData>("", null).ToArray();

            // Copy the IEnumerable places to an IQueryable.
            IQueryable<TData> queryablePlaces = places.AsQueryable<TData>();

            // Copy the expression tree that was passed in, changing only the first 
            // argument of the innermost MethodCallExpression.
            //ExpressionTreeModifier treeCopier = new ExpressionTreeModifier(queryablePlaces);
            ExpressionTreeModifier<TData> treeCopier = GetExpressionTreeModifier(queryablePlaces);
            Expression newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods. 
            if (IsEnumerable)
                return queryablePlaces.Provider.CreateQuery(newExpressionTree);
            else
                return queryablePlaces.Provider.Execute(newExpressionTree);
        }

        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance, 
            // expression is of type ConstantExpression, not MethodCallExpression. 
            return (expression is MethodCallExpression);
        }
    }
}
