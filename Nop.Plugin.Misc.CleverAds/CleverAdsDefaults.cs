using Nop.Core;

namespace Nop.Plugin.Misc.CleverAds
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class CleverAdsDefaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Misc.CleverAds";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets the platform name used in requests
        /// </summary>
        public static string PlatformName => "nopcommerce";

        /// <summary>
        /// Gets the credentials to authenticate requests
        /// </summary>
        public static (string Email, string Password) Credentials => ("nopcommerce@cleverppc.com", "n0pc0m3rc3_2021");

        /// <summary>
        /// Gets the hash secret
        /// </summary>
        public static string HashSecret => "4n7fdidvdrzvwe5hb0i4blohf4d8crc";

        /// <summary>
        /// Gets a system name of customer object
        /// </summary>
        public static string SystemCustomerName => "CleverAdsSystemCustomer";

        /// <summary>
        /// Gets the base URL of service
        /// </summary>
        public static string ServiceUrl => "https://nopcommerce.cleverecommerce.com/";

        /// <summary>
        /// Gets a default period (in seconds) before the request times out
        /// </summary>
        public static int RequestTimeout => 15;

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Misc.CleverAds.Configure";

        /// <summary>
        /// Gets the route name to fetch products
        /// </summary>
        public static string ProductsRouteName => "Plugin.Misc.CleverAds.Products";

        /// <summary>
        /// Gets the route name to fetch product by id
        /// </summary>
        public static string ProductRouteName => "Plugin.Misc.CleverAds.Product";
    }
}