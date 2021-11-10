using Newtonsoft.Json;

namespace Nop.Plugin.Misc.CleverAds.Domain.CleverAds
{
    /// <summary>
    /// Represents response from the service
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}