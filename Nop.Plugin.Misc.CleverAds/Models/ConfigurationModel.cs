using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CleverAds.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        public bool Enabled { get; set; }

        [NopResourceDisplayName("Plugins.Misc.CleverAds.Fields.Email")]
        [EmailAddress]
        public string Email { get; set; }
        public bool Email_OverrideForStore { get; set; }

        public string Hmac { get; set; }

        public bool IsLocal { get; set; }

        #endregion
    }
}