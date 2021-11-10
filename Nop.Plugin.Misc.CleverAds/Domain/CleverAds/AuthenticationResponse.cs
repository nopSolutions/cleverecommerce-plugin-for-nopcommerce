using Newtonsoft.Json;

namespace Nop.Plugin.Misc.CleverAds.Domain.CleverAds
{
    /// <summary>
    /// Represents authentication details
    /// </summary>
    public class AuthenticationResponse : Response
    {
        /// <summary>
        /// Gets or sets the authentication access token
        /// </summary>
        [JsonProperty(PropertyName = "auth_token")]
        public string AccessToken { get; set; }
    }
}