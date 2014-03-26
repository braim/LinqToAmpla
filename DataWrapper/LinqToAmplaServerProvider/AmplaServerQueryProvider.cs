using SE.MESCC.DAL.DataWrapper.LinqToAmplaServerProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SE.MESCC.DAL.DataWrapper.LinqToAmplaServerProvider
{
    class AmplaServerQueryProvider : IQueryProvider
    {

        //public IQueryable CreateIQueryable(Expression expression)
        //{
        //    return new QueryableAmplaServerData<TFactoryType>(this, expression);
        //}
        //public IExecutionContext CreateExecutionContext()
        //{
        //    return new AmplaServerQueryContext<TFactoryType>();
        //}
        public string Location { get; set; }
        public WebReferences.DataWS.AmplaModules Module { get; set; }
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                _Context = (IExecutionContext)Activator.CreateInstance(typeof(AmplaServerQueryContext<>).MakeGenericType(elementType));
                return (IQueryable)Activator.CreateInstance(typeof(QueryableAmplaServerData<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        // Queryable's collection-returning standard query operators call this method. 
        public IQueryable<TData> CreateQuery<TData>(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            _Context = (IExecutionContext)Activator.CreateInstance(typeof(AmplaServerQueryContext<>).MakeGenericType(elementType));
            return new QueryableAmplaServerData<TData>(this, expression);
        }
        IExecutionContext _Context = null;
        public object Execute(Expression expression)
        {
            return _Context.Execute(expression, false, Module);
        }

        // Queryable's "single value" standard query operators call this method.
        // It is also called from QueryableAmplaServerData.GetEnumerator(). 
        public TResult Execute<TResult>(Expression expression)
        {
            bool IsEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            return (TResult)_Context.Execute(expression, IsEnumerable,Module);
        }
    }
}
