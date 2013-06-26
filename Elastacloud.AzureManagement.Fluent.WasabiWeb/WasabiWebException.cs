using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.WasabiWeb
{
    /// <summary>
    /// A generic exception for the scaling block
    /// </summary>
    public class WasabiWebException : ApplicationException
    {
        /// <summary>
        /// used go construct a scaling exception
        /// </summary>
        public WasabiWebException(string message) : base(message)
        {            
        }
    }
}
