using MercadoPago;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MercadoPago
{
    internal partial class MPRESTClient
    {
        private IWebProxy _proxy;

        public string ProxyHostName { get; set; }

        public int ProxyPort { get; set; } = -1;

        /// <summary>
        /// class to simulate HttpClient class, available from .NET 4.0 onward.
        /// </summary>
        private class MPRequest
        {
            public HttpWebRequest Request { get; set; }
            public byte[] RequestPayload { get; set; }
        }

        static MPRESTClient()
        {
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;

            var v1 = Convert.ToUInt64((SecurityProtocolType)3072);
            var v2 = Convert.ToUInt64(ServicePointManager.SecurityProtocol);

            var hasFlag = (v2 & v1) == v1;

            if (!hasFlag)
                throw new NotSupportedException($"MercadoPago API requires TLS 1.2, which is not enabled on this machine. Please refer to https://www.mercadopago.com.ar/developers/es/guides/pci-compliant-merchants/disabling-tls-10 for details on how to fix this error.");
        }

        #region Core Methods
        /// <summary>
        /// Class constructor.
        /// </summary>
        public MPRESTClient() {}

        /// <summary>
        /// Set class variables.
        /// </summary>
        /// <param name="proxyHostName">Proxy host to use.</param>
        /// <param name="proxyPort">Proxy port to use in the proxy host.</param>
        public MPRESTClient(string proxyHostName, int proxyPort)
        {
            _proxy = new WebProxy(proxyHostName, proxyPort);
            ProxyHostName = proxyHostName;
            ProxyPort = proxyPort;
        }

        public JToken ExecuteGenericRequest(
            HttpMethod httpMethod,
            string path,
            PayloadType payloadType,
            JObject payload)
        {
            if (SDK.GetAccessToken() != null)
            {
                path = SDK.BaseUrl + path + "?access_token=" + SDK.GetAccessToken();
            }

            MPRequest mpRequest = CreateRequest(httpMethod, path, payloadType, payload, null, 0, 0);

            if (new HttpMethod[] { HttpMethod.POST, HttpMethod.PUT }.Contains(httpMethod))
            {
                Stream requestStream = mpRequest.Request.GetRequestStream();
                requestStream.Write(mpRequest.RequestPayload, 0, mpRequest.RequestPayload.Length);
                requestStream.Close();
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)mpRequest.Request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream, Encoding.UTF8);
                    String StringResponse = reader.ReadToEnd();
                    return JToken.Parse(StringResponse);
                }

            }
            catch (WebException ex)
            {
                HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                Stream dataStream = errorResponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream, Encoding.UTF8);
                String StringResponse = reader.ReadToEnd();
                return JToken.Parse(StringResponse);
            }

        }

        /// <summary>
        /// Execute a request to an endpoint.
        /// </summary>
        /// <param name="httpMethod">Method to use in the request.</param>
        /// <param name="path">Endpoint we are pointing.</param>
        /// <param name="payloadType">Type of payload we are sending along with the request.</param>
        /// <param name="payload">The data we are sending.</param>
        /// <param name="colHeaders">Extra headers to send with the request.</param>
        /// <returns>Api response with the result of the call.</returns>
        public MPAPIResponse ExecuteRequest(
            HttpMethod httpMethod,
            string path,
            PayloadType payloadType,
            JObject payload,
            bool includeHeaders,
            int requestTimeout,
            int retries)
        {
            var headers =
                includeHeaders
                    ? new WebHeaderCollection
                    {
                        "HTTP.CONTENT_TYPE: application/json",
                        "HTTP.USER_AGENT: MercadoPago .NET SDK v1.0.1"
                    }
                    : null;

            try
            {
                return ExecuteRequestCore(httpMethod, path, payloadType, payload, headers, requestTimeout, retries);
            }
            catch (Exception ex)
            {
                throw new MPRESTException(ex.Message);
            }
        }

        /// <summary>
        /// Core module implementation. Execute a request to an endpoint.
        /// This method is deprecated and will be removed in a future version, please use the
        /// <see cref="ExecuteRequest(HttpMethod, string, PayloadType, JObject, WebHeaderCollection, int, int)" /> method instead.
        /// </summary>
        /// <returns>Api response with the result of the call.</returns>
        internal MPAPIResponse ExecuteRequestCore(
            HttpMethod httpMethod,
            string path,
            PayloadType payloadType,
            JObject payload,
            WebHeaderCollection colHeaders,
            int connectionTimeout,
            int retries)
        {

            MPRequest mpRequest = CreateRequest(httpMethod, path, payloadType, payload, colHeaders, connectionTimeout, retries);
            string result = string.Empty;

            if ((httpMethod == HttpMethod.POST || httpMethod == HttpMethod.PUT) && mpRequest.RequestPayload != null)
            {
                Stream requestStream = mpRequest.Request.GetRequestStream();
                requestStream.Write(mpRequest.RequestPayload, 0, mpRequest.RequestPayload.Length);
                requestStream.Close();
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)mpRequest.Request.GetResponse())
                {

                    return new MPAPIResponse(httpMethod, mpRequest.Request, payload, response);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                    return new MPAPIResponse(httpMethod, mpRequest.Request, payload, errorResponse);
                }
                else
                {
                    if (--retries == 0)
                        throw;
                    return ExecuteRequestCore(httpMethod, path, payloadType, payload, colHeaders, connectionTimeout, retries);
                }

            }
        }

        /// <summary>
        /// Create a request to use in the call to a certain endpoint.
        /// </summary>
        /// <returns>Api response with the result of the call.</returns>
        private MPRequest CreateRequest(HttpMethod httpMethod,
            string path,
            PayloadType payloadType,
            JObject payload,
            WebHeaderCollection colHeaders,
            int connectionTimeout,
            int retries)
        {
            var requestOptions = CreateRequestOptions(colHeaders, connectionTimeout, retries);
            return CreateRequest(httpMethod, path, payloadType, payload, requestOptions);
        }

        /// <summary>
        /// Create a request to use in the call to a certain endpoint.
        /// </summary>
        /// <returns>Api response with the result of the call.</returns>
        private MPRequest CreateRequest(HttpMethod httpMethod,
            string path,
            PayloadType payloadType,
            JObject payload,
            MPRequestOptions requestOptions)
        {

            if (string.IsNullOrEmpty(path))
                throw new MPRESTException("Uri can not be an empty string.");

            if (httpMethod.Equals(HttpMethod.GET))
            {
                if (payload != null)
                {
                    throw new MPRESTException("Payload not supported for this method.");
                }
            }
            else if (httpMethod.Equals(HttpMethod.POST))
            {
                //if (payload == null)
                //{
                //    throw new MPRESTException("Must include payload for this method.");
                //}
            }
            else if (httpMethod.Equals(HttpMethod.PUT))
            {
                if (payload == null)
                {
                    throw new MPRESTException("Must include payload for this method.");
                }
            }
            else if (httpMethod.Equals(HttpMethod.DELETE))
            {
                if (payload != null)
                {
                    throw new MPRESTException("Payload not supported for this method.");
                }
            }

            MPRequest mpRequest = new MPRequest();
            mpRequest.Request = (HttpWebRequest)HttpWebRequest.Create(path);
            mpRequest.Request.Method = httpMethod.ToString();

            if (requestOptions.Timeout > 0)
            {
                mpRequest.Request.Timeout = requestOptions.Timeout;
            }

            if (payload != null) // POST & PUT
            {
                byte[] data;
                if (payloadType != PayloadType.JSON)
                {
                    var parametersDict = payload.ToObject<Dictionary<string, string>>();
                    StringBuilder parametersString = new StringBuilder();
                    parametersString.Append(string.Format("{0}={1}", parametersDict.First().Key, parametersDict.First().Value));
                    parametersDict.Remove(parametersDict.First().Key);
                    foreach (var value in parametersDict)
                    {
                        parametersString.Append(string.Format("&{0}={1}", value.Key, value.Value.ToString()));
                    }

                    data = Encoding.ASCII.GetBytes(parametersString.ToString());
                }
                else
                {
                    data = Encoding.ASCII.GetBytes(payload.ToString());
                }

                mpRequest.Request.ContentLength = data.Length;
                mpRequest.Request.ContentType = payloadType == PayloadType.JSON ? "application/json" : "application/x-www-form-urlencoded";
                mpRequest.RequestPayload = data;
            }

            IWebProxy proxy = requestOptions.Proxy != null ? requestOptions.Proxy : (_proxy != null ? _proxy : SDK.Proxy);
            if (proxy != null)
            {
                mpRequest.Request.Proxy = proxy;
            }

            return mpRequest;
        }

        private MPRequestOptions CreateRequestOptions(WebHeaderCollection colHeaders, int connectionTimeout, int retries)
        {
            var headers = new Dictionary<string, string>();
            if (colHeaders != null)
            {
                foreach (var header in colHeaders)
                {
                    headers.Add(header.ToString(), colHeaders[header.ToString()]);
                }
            }

            return new MPRequestOptions
            {
                Timeout = connectionTimeout,
                Retries = retries,
                CustomHeaders = headers
            };
        }

        #endregion
    }
}
