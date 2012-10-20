using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Linq
{
    public class LinqToAzureOrderedQueryable<TData> : IOrderedQueryable<TData>
    {

        #region Constructors
        /// <summary> 
        /// This constructor is called by the client to create the data source. 
        /// </summary> 
        public LinqToAzureOrderedQueryable(LinqToAzureInputs inputs)
        {
            Provider = new LinqToAzureQueryableProvider(inputs);
            Expression = Expression.Constant(this);
        }

        /// <summary> 
        /// This constructor is called by Provider.CreateQuery(). 
        /// </summary>
        /// <param name="provider"> </param>
        /// <param name="expression"></param>
        public LinqToAzureOrderedQueryable(LinqToAzureQueryableProvider provider, Expression expression)
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
        #endregion

        #region Properties

        public IQueryProvider Provider { get; private set; }
        public Expression Expression { get; private set; }

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
