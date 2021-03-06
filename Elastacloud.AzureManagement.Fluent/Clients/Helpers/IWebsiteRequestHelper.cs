﻿/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/


namespace Elastacloud.AzureManagement.Fluent.Clients.Helpers
{
    /// <summary>
    /// Used to make requests to the website or various helper classes
    /// </summary>
    public interface IWebsiteRequestHelper
    {
        /// <summary>
        /// Used to get a string response given a uri, username and password
        /// </summary>
        string GetStringResponse(string username, string password, string uri);
        /// <summary>
        /// Used to get a return value from a post request
        /// </summary>
        string PostStringResponse(string username, string password, string uri, string content);

        /// <summary>
        /// Executes a command request and doesn't wait or process the response but checks the status and throws an exception if not acheived
        /// </summary>
        /// <param name="uri">the uri requested</param>
        /// <param name="status">The Http Status response code expected</param>
        void ExecuteCommand(string uri, int status);
    }
}
