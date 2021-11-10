using Newtonsoft.Json;

namespace Nop.Plugin.Misc.CleverAds.Domain.CleverAds
{
    /// <summary>
    /// Represents base request with authorization
    /// </summary>
    public abstract class AuthorizedRequest : Request
    {
        /// <summary>
        /// Gets the request authorization value
        /// </summary>
        [JsonIgnore]
        public string Authorization { get; set; }
    }
}