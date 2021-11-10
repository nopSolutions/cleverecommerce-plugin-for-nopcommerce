using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.CleverAds.Domain.CleverAds
{
    /// <summary>
    /// Represents request to install new store instance
    /// </summary>
    public class InstallRequest : AuthorizedRequest
    {
        public InstallRequest(string authorization)
        {
            Authorization = authorization;
        }

        /// <summary>
        /// Gets or sets the client identifier
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the merchant store URL
        /// </summary>
        [JsonProperty(PropertyName = "domain")]
        public string StoreUrl { get; set; }

        /// <summary>
        /// Gets or sets the merchant email
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string MerchantEmail { get; set; }

        /// <summary>
        /// Gets or sets the merchant company name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the merchant company address
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        public string CompanyAddress { get; set; }

        /// <summary>
        /// Gets or sets the merchant company phone
        /// </summary>
        [JsonProperty(PropertyName = "phone")]
        public string CompanyPhone { get; set; }

        /// <summary>
        /// Gets or sets the merchant store logo URL
        /// </summary>
        [JsonProperty(PropertyName = "logo_url")]
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the merchant store primary currency
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string PrimaryCurrency { get; set; }

        /// <summary>
        /// Gets or sets the access token to request merchant store endpoints
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the platform name
        /// </summary>
        [JsonProperty(PropertyName = "platform")]
        public string PlatformName { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "api/nopcommerce/create_shop";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;
    }
}