/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent
{
    public class BasicHttpRequestBuilder : QueryManager.IHttpRequestBuilder
    {
        private Uri _requestUri;
        private List<X509Certificate> _certificates;
        private Dictionary<string, string> _headers;
        private string _method;
        private string _contentType;
        private string _body;
        private string _accept;

        public BasicHttpRequestBuilder()
        {
            _certificates = new List<X509Certificate>();
            _headers = new Dictionary<string, string>();
        }

        public void SetUri(Uri requestUri)
        {
            _requestUri = requestUri;
        }

        public void AddCertificate(X509Certificate certificate)
        {
            // clear the collection - currently keeps appending and an error ensues around multiple account usage
            // TODO: refactor this and understand why we're keeping a single instance of of the HttpBuilder 
            _certificates.Clear();
            _certificates.Add(certificate);
        }

        public void AddHeader(string key, string value)
        {
            _headers.Add(key, value);
        }

        public void SetMethod(string method)
        {
            _method = method;
        }

        public void SetContentType(string contentType)
        {
            _contentType = contentType;
        }

        public void SetBody(string body)
        {
            _body = body;
        }

        public void SetAccept(string accept)
        {
            _accept = accept;
        }

        public void ReplaceOrAddHeader(string key, string value)
        {
            if (HttpHeaderExists(key))
                _headers.Remove(key);
            _headers.Add(key, value);
        }

        public bool HttpHeaderExists(string key)
        {
            return _headers.ContainsKey(key);
        }

        public HttpWebRequest Create()
        {
            var request = (HttpWebRequest) WebRequest.Create(_requestUri);
            if (_certificates.Any())
            {
                foreach (var x509Certificate in _certificates)
                {
                    request.ClientCertificates.Add(x509Certificate);
                }
            }

            if (_headers.Any())
            {
                foreach (var header in _headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            request.Method = _method;
            request.ContentType = _contentType;
            request.Accept = _accept ?? "application/xml";
            request.ContentLength = 0;

            if (!string.IsNullOrEmpty(_body))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(_body);
                request.ContentLength = bytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            return request;
        }
    }
}