using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.CleverAds.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: CleverAdsDefaults.ConfigurationRouteName,
                pattern: "Admin/CleverAds/Configure",
                defaults: new { controller = "CleverAds", action = "Configure" });

            endpointRouteBuilder.MapControllerRoute(name: CleverAdsDefaults.ProductsRouteName,
                pattern: "cleverecommerce/products",
                defaults: new { controller = "CleverAdsPublic", action = "Products" });

            endpointRouteBuilder.MapControllerRoute(name: CleverAdsDefaults.ProductRouteName,
                pattern: "cleverecommerce/products/{id:min(0)}",
                defaults: new { controller = "CleverAdsPublic", action = "Product" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}