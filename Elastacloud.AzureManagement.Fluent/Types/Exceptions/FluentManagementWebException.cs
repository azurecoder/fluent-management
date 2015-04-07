/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Net;
using System.IO;

namespace Elastacloud.AzureManagement.Fluent.Types.Exceptions
{
    /// <summary>
    /// A special type of exception used to capture the command that failed
    /// </summary>
    public class FluentManagementWebException : ApplicationException
    {
        private WebException _webException = null;
        /// <summary>
        /// Used to construct a fluent management exceptino
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="commandName">The name of the command</param>
        public FluentManagementWebException(WebException webException)
        {
            _webException = webException;
        }

        /// <summary>
        /// Used to capture the name of the command that failed
        /// </summary>
        public override string Message
        {
            get
            {
                string message;
                using (var reader = new StreamReader(_webException.Response.GetResponseStream()))
                {
                    message = reader.ReadToEnd();
                }
                return message;
            }
        }
    }
}
