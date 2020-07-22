#if NETSTANDARD2_0

using System.Collections.Generic;
using System.Threading.Tasks;
using MercadoPago.Validation;
using Newtonsoft.Json.Linq;

namespace MercadoPago
{
    public abstract partial class Resource<T>
    {
        internal static async Task<MPAPIResponse> InvokeAsync(HttpMethod httpMethod, string path, PayloadType payloadType, JObject payload, string accessToken, Dictionary<string, string> queryParameters, bool useCache, int requestTimeout, int retries)
        {
            path = CreatePath(path, accessToken, queryParameters);

            var cacheKey = GetCacheKey(httpMethod, path);

            var response = TryGetFromCache(useCache, cacheKey);

            if (response == null)
            {
                response = await new MPRESTClient().ExecuteRequestAsync(
                    httpMethod,
                    path,
                    payloadType,
                    payload,
                    requestTimeout,
                    retries);

                TryAddToCache(useCache, cacheKey, response);
            }

            return response;
        }

        internal static async Task<T> GetAsync(string path, string accessToken = null, bool useCache = false, int requestTimeout = 0, int retries = 1)
        {
            var resource = new T();
            var response = await InvokeAsync(HttpMethod.GET, path, PayloadType.NONE, null, accessToken, null, useCache, requestTimeout, retries);

            ProcessResponse(path, resource, response, HttpMethod.GET);
            return resource;
        }

        internal Task<T> PostAsync(string path, bool useCache = false, int requestTimeout = 0, int retries = 1)
            => SendAsync(this as T, HttpMethod.POST, path, useCache, requestTimeout, retries);

        internal Task<T> PutAsync(string path, bool useCache = false, int requestTimeOut = 0, int retries = 1)
            => SendAsync(this as T, HttpMethod.PUT, path, useCache, requestTimeOut, retries);

        internal Task<T> DeleteAsync(string path, bool useCache = false, int requestTimeOut = 0, int retries = 1)
        {
            Send(this as T, HttpMethod.DELETE, path, useCache, requestTimeOut, retries);
            return Task.FromResult<T>(null);
        }

        internal static async Task<T> SendAsync(T resource, HttpMethod httpMethod, string path, bool useCache = false, int requestTimeout = 0, int retries = 1)
        {
            var postOrPut = httpMethod == HttpMethod.POST || httpMethod == HttpMethod.PUT;

            var payload = GetPayload(resource, httpMethod);

            if (postOrPut)
                Validator.Validate(resource);

            var response = await InvokeAsync(httpMethod, path, PayloadType.JSON, payload, resource.UserAccessToken, null, useCache, requestTimeout, retries);

            ProcessResponse(path, resource, response, httpMethod);
            return resource;
        }
    }
}

#endif
