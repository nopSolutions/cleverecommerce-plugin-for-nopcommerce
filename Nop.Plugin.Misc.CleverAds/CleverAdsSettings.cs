using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.CleverAds
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class CleverAdsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets merchant email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the client identifier
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the hash-based message authentication code
        /// </summary>
        public string Hmac { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product and category webhooks are enabled
        /// </summary>
        public bool WebhooksEnabled { get; set; }
    }
}