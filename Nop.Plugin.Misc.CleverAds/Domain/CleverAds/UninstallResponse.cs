using Newtonsoft.Json;

namespace Nop.Plugin.Misc.CleverAds.Domain.CleverAds
{
    /// <summary>
    /// Represents uninstallation details
    /// </summary>
    public class UninstallResponse : Response
    {
        /// <summary>
        /// Gets or sets the store instance identifier
        /// </summary>
        [JsonProperty(PropertyName = "shop_id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}