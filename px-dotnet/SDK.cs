using MercadoPago;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;

namespace MercadoPago
{
    public class SDK
    {
        private const int DEFAULT_REQUESTS_TIMEOUT = 30000;
        private const int DEFAULT_REQUESTS_RETRIES = 3;
        private const string DefaultBaseUrl = "https://api.mercadopago.com";
                
        private static string UserToken = null;
        private static int _requestsTimeout = DEFAULT_REQUESTS_TIMEOUT;
        private static int _requestsRetries = DEFAULT_REQUESTS_RETRIES;
        private static IWebProxy _proxy;        
        private static readonly object AccessTokenLock = new object();
        public static string RefreshToken = null;

        /// <summary>  
        ///  Property that represent the client secret token.
        /// </summary>
        public static string ClientSecret
        {
            get => _clientSecret;
            set
            {
                if (!string.IsNullOrEmpty(_clientSecret))
                {
                    throw new MPConfException("clientSecret setting can not be changed");
                }
                _clientSecret = value;
            }
        }

        private static string _clientSecret;

        /// <summary>
        /// Property that represents a client id.
        /// </summary>
        public static string ClientId
        {
            get => _clientId;
            set
            {
                if (!string.IsNullOrEmpty(_clientId))
                {
                    throw new MPConfException("clientId setting can not be changed");
                }
                _clientId = value;
            }
        }

        private static string _clientId;

        /// <summary>
        /// MercadoPago AccessToken.
        /// </summary>
        public static string AccessToken
        {
            get => _accessToken;
            set
            {
                if (!string.IsNullOrEmpty(_accessToken))
                {
                    throw new MPConfException("accessToken setting can not be changed");
                }
                _accessToken = value;
            }
        }

        private static string _accessToken;

        /// <summary>
        /// MercadoPAgo app id.
        /// </summary>
        public static string AppId
        {
            get => _appId;
            set
            {
                if (!string.IsNullOrEmpty(_appId))
                {
                    throw new MPConfException("appId setting can not be changed");
                }
                _appId = value;
            }
        }

        private static string _appId;

        /// <summary>
        /// Api base URL. Currently https://api.mercadopago.com
        /// </summary>
        public static string BaseUrl { get; private set; } = DefaultBaseUrl;

        /// <summary>
        /// Api requests timeout
        /// </summary>
        public static int RequestsTimeout {
            get { return _requestsTimeout; }
            set { _requestsTimeout = value; }
        }

        /// <summary>
        /// Api requests retries
        /// </summary>
        public static int RequestsRetries {
            get
            { return _requestsRetries; }
            set { _requestsRetries = value; }
        }

        public static IWebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        public static string Version { get; } = typeof(SDK).Assembly.GetName().Version.ToString();

#if NET40

        /// <summary>
        /// Dictionary based configuration. Valid configuration keys:
        /// clientSecret, clientId, accessToken, appId
        /// </summary>
        /// <param name="configurationParams"></param>
        public static void SetConfiguration(IDictionary<string, string> configurationParams)
        {
            if (configurationParams == null)
                throw new ArgumentException("Invalid configurationParams parameter");

            configurationParams.TryGetValue("clientSecret", out _clientSecret);
            configurationParams.TryGetValue("clientId", out _clientId);
            configurationParams.TryGetValue("accessToken", out _accessToken);
            configurationParams.TryGetValue("appId", out _appId);

            String requestsTimeoutStr;
            if (configurationParams.TryGetValue("requestsTimeout", out requestsTimeoutStr))
            {
                Int32.TryParse(requestsTimeoutStr, out _requestsTimeout);
            }

            String requestsRetriesStr;
            if (configurationParams.TryGetValue("requestsRetries", out requestsRetriesStr))
            {
                Int32.TryParse(requestsRetriesStr, out _requestsRetries);
            }

            String proxyHostName;
            String proxyPortStr;
            int proxyPort;
            if (configurationParams.TryGetValue("proxyHostName", out proxyHostName) 
                && configurationParams.TryGetValue("proxyPort", out proxyPortStr)
                && Int32.TryParse(proxyPortStr, out proxyPort))
            {
                _proxy = new WebProxy(proxyHostName, proxyPort);
                
                String proxyUsername;
                String proxyPassword;
                if (configurationParams.TryGetValue("proxyUsername", out proxyUsername)
                    && configurationParams.TryGetValue("proxyPassword", out proxyPassword))
                {
                    _proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                }
            }
        }

        /// <summary>
        /// Initializes the configurations based in a confiiguration object.
        /// Searches for the configuration keys: "ClientId, "ClientSecret", "AccessToken", and "AppId"
        /// </summary>
        /// <param name="config"></param>
        public static void SetConfiguration(Configuration config)
        {
			if (config == null)
                throw new ArgumentException("config parameter cannot be null");

            _clientSecret = GetConfigValue(config, "ClientSecret");
            _clientId = GetConfigValue(config, "ClientId");
            _accessToken = GetConfigValue(config, "AccessToken");
            _appId = GetConfigValue(config, "AppId");

            String requestsTimeoutStr = GetConfigValue(config, "RequestsTimeout");
            Int32.TryParse(requestsTimeoutStr, out _requestsTimeout);

            String requestsRetriesStr = GetConfigValue(config, "RequestsRetries");
            Int32.TryParse(requestsRetriesStr, out _requestsRetries);

            String proxyHostName = GetConfigValue(config, "ProxyHostName");
            String proxyPortStr = GetConfigValue(config, "ProxyPort");
            int proxyPort;
            if (!String.IsNullOrEmpty(proxyHostName) && Int32.TryParse(proxyPortStr, out proxyPort))
            {
                _proxy = new WebProxy(proxyHostName, proxyPort);
                
                String proxyUsername = GetConfigValue(config, "ProxyUsername");
                String proxyPassword = GetConfigValue(config, "ProxyPassword");
                if (!String.IsNullOrEmpty(proxyUsername) && !String.IsNullOrEmpty(proxyPassword))
                {
                    _proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                }
            }
        }

#endif

        /// <summary>
        /// Clean all the configuration variables
        /// (FOR TESTING PURPOSES ONLY)
        /// </summary>
        internal static void CleanConfiguration()
        {
            _clientSecret = null;
            _clientId = null;
            _accessToken = null;
            _appId = null;
            BaseUrl = DefaultBaseUrl;
        }

        /// <summary>
        /// Changes base Url
        /// (FOR TESTING PURPOSES ONLY)
        /// </summary>
        internal static void SetBaseUrl(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

#if NET40

        private static string GetConfigValue(Configuration config, string key)
        {
            string value = null;
            KeyValueConfigurationElement keyValue = config.AppSettings.Settings[key];
            if (keyValue != null)
            {
                value = keyValue.Value;
            }
            return value;
        }

#endif

        /// <summary>
        /// Authenticate with MercadoPago API, using SDK.ClientId and SDK.ClientSecret. The resulting token is stored in the SDK.AccessToken property.
        /// </summary>
        /// <returns>A valid access token.</returns>
        public static string GetAccessToken()
        {            
            if (string.IsNullOrEmpty(AccessToken))
            {
                lock(AccessTokenLock)
                {
                    if (string.IsNullOrEmpty(AccessToken))
                        AccessToken = MPCredentials.GetAccessToken(ClientId, ClientSecret);
                }
            }
            return AccessToken;
        }

        /// <summary>
        /// Authenticate with MercadoPago API, using provided clientId and clientSecret.
        /// </summary>
        /// <returns>A valid access token.</returns>
        public static string GetAccessToken(string clientId, string clientSecret) =>
            MPCredentials.GetAccessToken(clientId, clientSecret);

        internal static JToken Get(string uri)
        {
            MPRESTClient client = new MPRESTClient();
            return client.ExecuteGenericRequest(HttpMethod.GET, uri, PayloadType.JSON, null);
        }

        internal static JToken Post(string uri, JObject payload)
        {
            MPRESTClient client = new MPRESTClient();
            return client.ExecuteGenericRequest(HttpMethod.POST, uri, PayloadType.JSON, payload);
        }

        internal static JToken Put(string uri, JObject payload)
        {
            MPRESTClient client = new MPRESTClient();
            return client.ExecuteGenericRequest(HttpMethod.PUT, uri, PayloadType.JSON, payload);
        }

    }
}
