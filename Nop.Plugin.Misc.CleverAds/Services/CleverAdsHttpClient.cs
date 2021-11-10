using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.CleverAds.Domain.CleverAds;

namespace Nop.Plugin.Misc.CleverAds.Services
{
    /// <summary>
    /// Represents HTTP client to request third-party services
    /// </summary>
    public class CleverAdsHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public CleverAdsHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(CleverAdsDefaults.ServiceUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(CleverAdsDefaults.RequestTimeout);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, CleverAdsDefaults.UserAgent);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

            _httpClient = httpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Request services
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">Request</param>
        /// <returns>The asynchronous task whose result contains response details</returns>
        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) where TRequest : Request where TResponse : Response
        {
            try
            {
                //prepare request parameters
                var requestString = JsonConvert.SerializeObject(request);
                var requestContent = new StringContent(requestString, Encoding.UTF8, MimeTypes.ApplicationJson);
                var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), request.Path) { Content = requestContent };

                //add authorization
                if (request is AuthorizedRequest authorizedRequest && !string.IsNullOrEmpty(authorizedRequest.Authorization))
                    requestMessage.Headers.Add(HeaderNames.Authorization, authorizedRequest.Authorization);

                //execute request and get response
                var httpResponse = await _httpClient.SendAsync(requestMessage);
                httpResponse.EnsureSuccessStatusCode();

                //return result
                var responseString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty);
                if (!string.IsNullOrEmpty(result?.Error))
                    throw new NopException($"Request error - {result.Error}");

                return result;
            }
            catch (AggregateException exception)
            {
                //rethrow actual exception
                throw exception.InnerException;
            }
        }

        #endregion
    }
}