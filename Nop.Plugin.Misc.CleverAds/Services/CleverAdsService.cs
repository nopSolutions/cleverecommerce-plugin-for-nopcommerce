using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Misc.CleverAds.Domain.CleverAds;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Web.Factories;

namespace Nop.Plugin.Misc.CleverAds.Services
{
    /// <summary>
    /// Represents the plugin service
    /// </summary>
    public class CleverAdsService
    {
        #region Fields

        private readonly CleverAdsHttpClient _cleverAdsHttpClient;
        private readonly CurrencySettings _currencySettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CleverAdsService(CleverAdsHttpClient cleverAdsHttpClient,
            CurrencySettings currencySettings,
            ICommonModelFactory commonModelFactory,
            ICurrencyService currencyService,
            ICustomerService customerService,
            ILogger logger,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _cleverAdsHttpClient = cleverAdsHttpClient;
            _currencySettings = currencySettings;
            _commonModelFactory = commonModelFactory;
            _currencyService = currencyService;
            _customerService = customerService;
            _logger = logger;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result; error if exists
        /// </returns>
        private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function)
        {
            try
            {
                return (await function(), default);
            }
            catch (Exception exception)
            {
                var errorMessage = exception.Message;
                var customer = await _workContext.GetCurrentCustomerAsync();
                await _logger.ErrorAsync($"{CleverAdsDefaults.SystemName} error. {errorMessage}", exception, customer);

                return (default, errorMessage);
            }
        }

        /// <summary>
        /// Get the authentication access token
        /// </summary>
        /// <param name="email">Email to authenticate</param>
        /// <param name="password">Password to authenticate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the access token
        /// </returns>
        private async Task<string> GetAccessTokenAsync(string email, string password)
        {
            if (email is null)
                throw new ArgumentNullException(nameof(email));

            if (password is null)
                throw new ArgumentNullException(nameof(password));

            var request = new AuthenticationRequest { Email = email, Password = password };
            var authentication = await _cleverAdsHttpClient.RequestAsync<AuthenticationRequest, AuthenticationResponse>(request)
                ?? throw new NopException($"Empty result");

            return authentication.AccessToken;
        }

        /// <summary>
        /// Generate a cryptographic random number with the passed key size
        /// </summary>
        /// <param name="keySize">Key size</param>
        /// <returns>Result</returns>
        private static string GenerateClientId(int keySize)
        {
            using var provider = new RNGCryptoServiceProvider();
            var key = new byte[keySize];
            provider.GetBytes(key);

            return Convert.ToBase64String(key).TrimEnd('=');
        }

        /// <summary>
        /// Generate a hash-based message authentication code (HMAC) for the passed object
        /// </summary>
        /// <param name="payload">Object to hash</param>
        /// <returns>HMAC string</returns>
        private static string GenerateHmac(object payload)
        {
            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
            var keyBytes = Encoding.UTF8.GetBytes(CleverAdsDefaults.HashSecret);

            using var cryptographer = new HMACSHA256(keyBytes);
            var hashedBytes = cryptographer.ComputeHash(payloadBytes);

            var innerKey = Convert.ToBase64String(payloadBytes).TrimEnd('=');
            //var outerKey = Convert.ToBase64String(hashedBytes).TrimEnd('=');
            var outerKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(BitConverter.ToString(hashedBytes).Replace("-", "").ToLower())).TrimEnd('=');

            return $"{innerKey}.{outerKey}";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install new store instance with the passed email
        /// </summary>
        /// <param name="store">Store to install</param>
        /// <param name="email">Merchant email</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains client identifier and HMAC for current store; error message if exists
        /// </returns>
        public async Task<((string ClientId, string Hmac), string Error)> InstallAsync(Store store, string email)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (string.IsNullOrEmpty(email))
                    throw new NopException("Email is not set");

                if (string.IsNullOrEmpty(store?.Url))
                    throw new NopException("Store URL is not set");

                if (store.Url.Contains("localhost"))
                    throw new NopException($"The plugin cannot be used on localhost sites ({store.Url})");

                var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)
                    ?? throw new NopException("Primary currency is not set");

                //prepare HMAC and client identifier (32 is enough for sha256 algorithm)
                var clientId = GenerateClientId(32);
                var hmac = GenerateHmac(new { store_hash = clientId, timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), email = email });

                //get access token
                var accessToken = await GetAccessTokenAsync(CleverAdsDefaults.Credentials.Email, CleverAdsDefaults.Credentials.Password);

                //invoke request
                var request = new InstallRequest(accessToken)
                {
                    ClientId = clientId,
                    StoreUrl = $"{store.Url.TrimEnd('/')}/",
                    CompanyName = store.CompanyName ?? string.Empty,
                    CompanyAddress = store.CompanyAddress ?? string.Empty,
                    CompanyPhone = store.CompanyPhoneNumber ?? string.Empty,
                    MerchantEmail = email,
                    LogoUrl = (await _commonModelFactory.PrepareLogoModelAsync())?.LogoPath ?? string.Empty,
                    PrimaryCurrency = currency.CurrencyCode,
                    AccessToken = clientId,
                    PlatformName = CleverAdsDefaults.PlatformName
                };
                await _cleverAdsHttpClient.RequestAsync<InstallRequest, InstallResponse>(request);

                return (clientId, hmac);
            });
        }

        /// <summary>
        /// Uninstall the store instance
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of request; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> UninstallAsync(string clientId)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (string.IsNullOrEmpty(clientId))
                    throw new NopException("Client identifier is not set");

                //get access token
                var accessToken = await GetAccessTokenAsync(CleverAdsDefaults.Credentials.Email, CleverAdsDefaults.Credentials.Password);

                //invoke request
                var request = new UninstallRequest(accessToken)
                {
                    ClientId = clientId,
                    PlatformName = CleverAdsDefaults.PlatformName
                };
                await _cleverAdsHttpClient.RequestAsync<UninstallRequest, UninstallResponse>(request);

                return true;
            });
        }

        /// <summary>
        /// Update the passed entry details
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="clientId">Client identifier</param>
        /// <param name="entry">Entity entry</param>
        /// <param name="operationType">Operation type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of request; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> UpdateEntryAsync<TEntity>(string clientId, TEntity entry, OperationType operationType)
            where TEntity : BaseEntity
        {
            return await HandleFunctionAsync(async () =>
            {
                if (string.IsNullOrEmpty(clientId))
                    throw new NopException("Client identifier is not set");

                //get access token
                var accessToken = await GetAccessTokenAsync(CleverAdsDefaults.Credentials.Email, CleverAdsDefaults.Credentials.Password);

                //invoke request
                var request = new UpdateRequest<TEntity>(accessToken, entry, operationType)
                {
                    ClientId = clientId,
                    PlatformName = CleverAdsDefaults.PlatformName
                };
                await _cleverAdsHttpClient.RequestAsync<UpdateRequest<TEntity>, Response>(request);

                return true;
            });
        }

        /// <summary>
        /// Get system customer used for requests from third-party services
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a customer object
        /// </returns>
        public async Task<Customer> GetOrCreateSystemCustomerAsync()
        {
            var customer = await _customerService.GetCustomerBySystemNameAsync(CleverAdsDefaults.SystemCustomerName);
            if (customer is null)
            {
                var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName)
                    ?? throw new NopException($"'{NopCustomerDefaults.GuestsRoleName}' role could not be loaded");

                customer = new Customer
                {
                    Email = CleverAdsDefaults.Credentials.Email,
                    Username = CleverAdsDefaults.Credentials.Email,
                    CustomerGuid = Guid.NewGuid(),
                    AdminComment = $"System guest record used for requests from Clever Ads services.",
                    Active = true,
                    IsSystemAccount = true,
                    SystemName = CleverAdsDefaults.SystemCustomerName,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                    RegisteredInStoreId = (await _storeContext.GetCurrentStoreAsync()).Id
                };

                await _customerService.InsertCustomerAsync(customer);
                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
                {
                    CustomerRoleId = guestRole.Id,
                    CustomerId = customer.Id
                });
            }

            if (customer is null)
                throw new NopException("System customer object could not be loaded");

            return customer;
        }

        #endregion
    }
}