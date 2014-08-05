/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Commands.Blobs
{
    public enum StorageServiceType
    {
        Blob,
        Table,
        Queue,
        Analytics
    }

    [Flags]
    public enum AnalyticsMetricsType
    {
        Logging = 1,
        Hourly = 2,
        Minute = 4
    }
    /// <summary>
    /// Abstract Base class with functions to allow blob handling using the storage services API
    /// </summary>
    internal abstract class BlobCommand : ICommand
    {
        #region private members

        private string _httpVerb = "PUT";

        #endregion

        #region constants

        internal const string HttpVerbPut = "PUT";
        internal const string HttpVerbDelete = "DELETE";
        internal const string HttpVerbPost = "POST";
        internal const string HttpVerbGet = "GET";
        internal const string VersionHeader = "2011-08-18";

        #endregion

        #region Properties

        internal string AccountName { get; set; }
        internal string AccountKey { get; set; }
        internal string ContainerName { get; set; }
        internal string BlobName { get; set; }

        internal string Version { get; set; }

        internal string HttpVerb
        {
            get { return _httpVerb; }
            set { _httpVerb = value; }
        }
        /// <summary>
        /// Used to check a payload from a get request and whether there has been a positive response
        /// </summary>
        internal Func<string, bool> PayloadAnalyser { set; get; } 

        internal string DateHeader { get; set; }

        #endregion

        protected BlobCommand()
        {
            DateHeader = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
        }

        protected abstract StorageServiceType StorageServiceType { get; }

        /// <summary>
        /// Checks to see whether a storage account exists based on checking every second the operation will either return a 502
        /// if the DNS cannot resolve or a 400 if it can 
        /// </summary>
        /// <param name="timeoutInSeconds">The timeout in seconds to stop the checking</param>
        /// <returns>A value indicating that the storage exists</returns>
        public bool CheckStorageAccountExists(int timeoutInSeconds)
        {
            int i = 0;
            bool hasCompleted = false;
            while (i++ < timeoutInSeconds)
            {
                try
                {
                    var request = HttpWebRequest.Create(String.Format("http://{0}.blob.core.windows.net", AccountName));
                    request.GetResponse();
                }
                catch (WebException ex)
                {
                    if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.BadRequest)
                    {
                        hasCompleted = true;
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
            return hasCompleted;
        }

        #region File Handling

        /// <summary>
        /// Finds out the length and returns a byte array of the files 
        /// </summary>
        /// <param name="fileName">The filename being used to retrieve the bytes and length</param>
        /// <param name="contentLength">The length of the file in bytes</param>
        /// <returns>The byte array containing the file data</returns>
        protected byte[] GetPackageFileBytesAndLength(string fileName, out int contentLength)
        {
            byte[] ms = null;
            contentLength = 0;
            if (fileName != null)
            {
                FileStream fs = File.Open(fileName, FileMode.Open);
                contentLength = (int) fs.Length;
                ms = new byte[fs.Length];
                fs.Read(ms, 0, contentLength);
            }
            return ms;
        }

        #endregion

        #region Request Handling

        /// <summary>
        /// Preprares the request for sending and using blob storage
        /// </summary>
        /// <param name="url">The url to send the request to</param>
        /// <param name="authHeader">The signature being used</param>
        /// <param name="fileBytes">The request content</param>
        /// <param name="contentLength">The length of the content</param>
        /// <returns>The HttpWebRequest containing the file</returns>
        private HttpWebRequest PrepareRequest(string url, string authHeader, byte[] fileBytes = null, int contentLength = 0)
        {
            var uri = new Uri(url);
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = HttpVerb;
            request.ContentLength = contentLength;
            request.Headers.Add("x-ms-date", DateHeader);
            request.Headers.Add("x-ms-version", Version ?? VersionHeader);
            request.Headers.Add("Authorization", authHeader);
            switch (contentLength)
            {
                case 0:
                    return request;
            }
            if (StorageServiceType == StorageServiceType.Blob)
            {
                request.Headers.Add("x-ms-blob-type", "BlockBlob");                    
            }
            request.GetRequestStream().Write(fileBytes, 0, fileBytes.Length);
            return request;
        }

        protected void SendWebRequest(string url, string authHeader, byte[] fileBytes = null, int contentLength = 0)
        {
            HttpWebRequest request = PrepareRequest(url, authHeader, fileBytes, contentLength);
            try
            {
                HttpWebResponse response;
                using (response = (HttpWebResponse) request.GetResponse())
                {
                    if (HttpVerb == HttpVerbGet && PayloadAnalyser != null)
                    {
                        var reader = new StreamReader(response.GetResponseStream());
                        PayloadAnalyser(reader.ReadToEnd());
                    }
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        Trace.WriteLine("Container or blob has been created!");
                    }
                    if (response.StatusCode == HttpStatusCode.Accepted)
                    {
                        Trace.WriteLine("Container or blob action has been completed");
                    }
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.Conflict)
                {
                    throw new FluentManagementException("container or blob already exists!", "BlobCommand");
                }
                if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new FluentManagementException("problem with signature!", "BlobCommand");
                }
                throw new FluentManagementException(ex.ToString(), "BlobCommand");
            }
        }

        #endregion

        #region Shared Access Signature

        protected string CreateAuthorizationHeader(String canResource, string options = "", int contentLength = 0)
        {
            string toSign = String.Format("{0}\n\n\n{1}\n\n\n\n\n\n\n\n{5}\nx-ms-date:{2}\nx-ms-version:{3}\n{4}",
                HttpVerb, HttpVerb == HttpVerbGet ? "" : contentLength.ToString(CultureInfo.InvariantCulture), DateHeader, Version ?? VersionHeader, canResource, options);

            string signature;
            using (var hmacSha256 = new HMACSHA256(Convert.FromBase64String(AccountKey)))
            {
                Byte[] bytes = Encoding.UTF8.GetBytes(toSign);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(bytes));
            }

            return "SharedKey " + AccountName + ":" + signature;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Abstract method used to execute the any command againt blob storage
        /// </summary>
        public abstract void Execute();

        public const string TableService = "table";
        public const string BlobService = "blob";
        public const string QueueService = "queue";

        #endregion
    }
}