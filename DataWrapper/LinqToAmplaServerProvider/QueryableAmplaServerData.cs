using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SE.MESCC.DAL.DataWrapper.LinqToAmplaServerProvider
{

    public class QueryableAmplaServerData<TData> : IOrderedQueryable<TData> 
    {
        public QueryableAmplaServerData( WebReferences.DataWS.AmplaModules module)
        {
            Provider = new AmplaServerQueryProvider()
            {
               // Location = location,
                Module = module
            };
            Expression = System.Linq.Expressions.Expression.Constant(this);
        }
                /// <summary> 
        /// This constructor is called by Provider.CreateQuery(). 
        /// </summary> 
        /// <param name="expression"></param>
        internal QueryableAmplaServerData(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<TData>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            Provider = provider;
            Expression = expression;
        }
        #region Properties
        private IQueryProvider _Provider;
        public IQueryProvider Provider
        {
            get
            {
                return _Provider;
            }

            private set
            {
                _Provider = value;
            }
        }

        public System.Linq.Expressions.Expression Expression { get; private set; }

        public Type ElementType
        {
            get { return typeof(TData); }
        }

        #endregion

        #region Enumerators
        public IEnumerator<TData> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<TData>>(Expression)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<System.Collections.IEnumerable>(Expression)).GetEnumerator();
        }
        #endregion
    }
}

