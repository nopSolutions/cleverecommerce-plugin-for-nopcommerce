using FluentValidation;
using Nop.Plugin.Misc.CleverAds.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.CleverAds.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
        }

        #endregion
    }
}