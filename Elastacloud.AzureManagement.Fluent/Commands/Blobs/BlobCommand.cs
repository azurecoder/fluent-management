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

namespace Elastacloud.AzureManagement.Fluent.Commands.Blobs
{
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

        internal string HttpVerb
        {
            get { return _httpVerb; }
            set { _httpVerb = value; }
        }

        internal string DateHeader { get; set; }

        #endregion

        protected BlobCommand()
        {
            DateHeader = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
        }

        #region File Handling

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

        private HttpWebRequest PrepareRequest(string url, string authHeader, byte[] fileBytes = null,
                                              int contentLength = 0)
        {
            var uri = new Uri(url);
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = HttpVerb;
            request.ContentLength = contentLength;
            request.Headers.Add("x-ms-date", DateHeader);
            request.Headers.Add("x-ms-version", VersionHeader);
            request.Headers.Add("Authorization", authHeader);
            if (contentLength != 0)
            {
                request.Headers.Add("x-ms-blob-type", "BlockBlob");
                request.GetRequestStream().Write(fileBytes, 0, fileBytes.Length);
            }
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
                    Trace.WriteLine("container or blob already exists!");
                }
                if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.Forbidden)
                {
                    Trace.WriteLine("problem with signature!");
                }
            }
        }

        #endregion

        #region Shared Access Signature

        protected string CreateAuthorizationHeader(String canResource, string options = "", int contentLength = 0)
        {
            string toSign = String.Format("{0}\n\n\n{1}\n\n\n\n\n\n\n\n{5}\nx-ms-date:{2}\nx-ms-version:{3}\n{4}",
                                          HttpVerb, contentLength, DateHeader, VersionHeader, canResource, options);

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

        public abstract void Execute();

        #endregion
    }
}