using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.CleverAds.Domain.CleverAds
{
    /// <summary>
    /// Represents request to uninstall current store
    /// </summary>
    public class UninstallRequest : AuthorizedRequest
    {
        public UninstallRequest(string authorization)
        {
            Authorization = authorization;
        }

        /// <summary>
        /// Gets or sets the client identifier
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the platform name
        /// </summary>
        [JsonProperty(PropertyName = "platform")]
        public string PlatformName { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "api/nopcommerce/uninstall_shop";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;
    }
}