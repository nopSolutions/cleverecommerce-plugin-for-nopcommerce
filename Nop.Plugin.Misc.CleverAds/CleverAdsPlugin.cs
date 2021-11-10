using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Plugin.Misc.CleverAds.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.CleverAds
{
    /// <summary>
    /// Represents Clever Ads plugin
    /// </summary>
    public class CleverAdsPlugin : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly CleverAdsService _cleverAdsService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IUrlHelperFactory _urlHelperFactory;

        #endregion

        #region Ctor

        public CleverAdsPlugin(CleverAdsService cleverAdsService,
            IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            ISettingService settingService,
            IUrlHelperFactory urlHelperFactory)
        {
            _cleverAdsService = cleverAdsService;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _settingService = settingService;
            _urlHelperFactory = urlHelperFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(CleverAdsDefaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _cleverAdsService.GetOrCreateSystemCustomerAsync();
            await _settingService.SaveSettingAsync(new CleverAdsSettings { WebhooksEnabled = true });
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.CleverAds.Configuration.Disabled"] = "Disabled",
                ["Plugins.Misc.CleverAds.Configuration.Enabled"] = "Enabled",
                ["Plugins.Misc.CleverAds.Configuration.Error"] = "Error: {0} (see details in the <a href=\"{1}\" target=\"_blank\">log</a>)",
                ["Plugins.Misc.CleverAds.Configuration.Installed"] = "Access is granted, now you are ready to start your ad on Google",
                ["Plugins.Misc.CleverAds.Configuration.Localhost"] = "The plugin cannot be used on localhost sites",
                ["Plugins.Misc.CleverAds.Configuration.Uninstall"] = "Revoke access",
                ["Plugins.Misc.CleverAds.Configuration.Uninstalled"] = "Profile access has been successfully revoked",
                ["Plugins.Misc.CleverAds.Fields.Email"] = "Email",
                ["Plugins.Misc.CleverAds.Fields.Email.Hint"] = "Enter your email to get access to Clever Ads.",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<CleverAdsSettings>();
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.CleverAds");

            await base.UninstallAsync();
        }

        #endregion
    }
}