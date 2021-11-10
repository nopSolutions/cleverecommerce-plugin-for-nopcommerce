using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.CleverAds.Domain.CleverAds
{
    /// <summary>
    /// Represents request to get access token
    /// </summary>
    public class AuthenticationRequest : Request
    {
        /// <summary>
        /// Gets or sets the email to authenticate
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password to authenticate
        /// </summary>
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "api/nopcommerce/authenticate";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;
    }
}