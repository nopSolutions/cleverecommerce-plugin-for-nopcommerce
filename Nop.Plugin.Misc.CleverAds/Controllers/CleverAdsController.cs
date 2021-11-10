using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.CleverAds.Models;
using Nop.Plugin.Misc.CleverAds.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.CleverAds.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class CleverAdsController : BasePluginController
    {
        #region Fields

        private readonly CleverAdsService _cleverAdsService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CleverAdsController(CleverAdsService cleverAdsService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _cleverAdsService = cleverAdsService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CleverAdsSettings>(storeId);
            if (storeId > 0)
            {
                //we don't need some of the shared settings that loaded above, so load them separately for chosen store
                settings.Email = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(CleverAdsSettings)}.{nameof(CleverAdsSettings.Email)}", storeId: storeId);
                settings.Hmac = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(CleverAdsSettings)}.{nameof(CleverAdsSettings.Hmac)}", storeId: storeId);
            }

            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeId,
                Enabled = settings.Enabled,
                Email = settings.Email,
                Hmac = settings.Hmac
            };
            if (storeId > 0)
                model.Email_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.Email, storeId);

            //plugin cannot be used on localhost sites
            if (_webHelper.IsLocalRequest(Request))
            {
                model.IsLocal = true;
                var warningLocale = await _localizationService.GetResourceAsync("Plugins.Misc.CleverAds.Configuration.Localhost");
                _notificationService.WarningNotification(warningLocale);
            }

            return View("~/Plugins/Misc.CleverAds/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EnablePlugin(bool value)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            //set this setting for all stores
            await _settingService.SetSettingAsync($"{nameof(CleverAdsSettings)}.{nameof(CleverAdsSettings.Enabled)}", value);

            return Json(new { Result = true });
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("install")]
        public async Task<IActionResult> Install(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //get the specified store and load settings
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var store = storeId > 0
                ? await _storeService.GetStoreByIdAsync(storeId)
                : await _storeContext.GetCurrentStoreAsync();
            var settings = await _settingService.LoadSettingAsync<CleverAdsSettings>(storeId);

            //try to install the store instance with the passed email
            var ((clientId, hmac), error) = await _cleverAdsService.InstallAsync(store, model.Email);
            if (!string.IsNullOrEmpty(error))
            {
                var errorLocale = await _localizationService.GetResourceAsync("Plugins.Misc.CleverAds.Configuration.Error");
                var errorMessage = string.Format(errorLocale, error, Url.Action("List", "Log"));
                _notificationService.ErrorNotification(errorMessage, false);

                return await Configure();
            }

            settings.Email = model.Email;
            settings.ClientId = clientId;
            settings.Hmac = hmac;
            var overrideSettings = model.Email_OverrideForStore;
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.Email, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.ClientId, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.Hmac, overrideSettings, storeId, false);
            await _settingService.ClearCacheAsync();

            var successLocale = await _localizationService.GetResourceAsync("Plugins.Misc.CleverAds.Configuration.Installed");
            _notificationService.SuccessNotification(successLocale);

            return await Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("uninstall")]
        public async Task<IActionResult> Uninstall()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CleverAdsSettings>(storeId);
            if (storeId > 0)
            {
                //we don't need some of the shared settings that loaded above, so load them separately for chosen store
                settings.ClientId = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(CleverAdsSettings)}.{nameof(CleverAdsSettings.ClientId)}", storeId: storeId);
            }

            //uninstall the current store instance
            var (_, error) = await _cleverAdsService.UninstallAsync(settings.ClientId);
            if (!string.IsNullOrEmpty(error))
            {
                var errorLocale = await _localizationService.GetResourceAsync("Plugins.Misc.CleverAds.Configuration.Error");
                var errorMessage = string.Format(errorLocale, error, Url.Action("List", "Log"));
                _notificationService.ErrorNotification(errorMessage, false);

                return await Configure();
            }

            //reset settings
            settings.Email = string.Empty;
            settings.ClientId = string.Empty;
            settings.Hmac = string.Empty;
            var overrideSettings = storeId > 0;
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.Email, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.ClientId, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.Hmac, overrideSettings, storeId, false);
            await _settingService.ClearCacheAsync();

            var successLocale = await _localizationService.GetResourceAsync("Plugins.Misc.CleverAds.Configuration.Uninstalled");
            _notificationService.SuccessNotification(successLocale);

            return await Configure();
        }

        #endregion
    }
}