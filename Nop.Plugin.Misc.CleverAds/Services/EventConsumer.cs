using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Plugin.Misc.CleverAds.Domain.CleverAds;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.CleverAds.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>,
        IConsumer<EntityInsertedEvent<ProductPicture>>,
        IConsumer<EntityDeletedEvent<ProductPicture>>,
        IConsumer<EntityInsertedEvent<ProductProductTagMapping>>,
        IConsumer<EntityDeletedEvent<ProductProductTagMapping>>,
        IConsumer<EntityInsertedEvent<ProductCategory>>,
        IConsumer<EntityDeletedEvent<ProductCategory>>,
        IConsumer<EntityInsertedEvent<ProductManufacturer>>,
        IConsumer<EntityDeletedEvent<ProductManufacturer>>,
        IConsumer<EntityInsertedEvent<ProductSpecificationAttribute>>,
        IConsumer<EntityDeletedEvent<ProductSpecificationAttribute>>,
        IConsumer<EntityInsertedEvent<ProductAttributeMapping>>,
        IConsumer<EntityDeletedEvent<ProductAttributeMapping>>,
        IConsumer<EntityDeletedEvent<Store>>
    {
        #region Fields

        private readonly CleverAdsService _cleverAdsService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public EventConsumer(CleverAdsService cleverAdsService,
            IProductService productService,
            ISettingService settingService,
            IStoreService storeService)
        {
            _cleverAdsService = cleverAdsService;
            _productService = productService;
            _settingService = settingService;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Update entry details
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entry">Entity entry</param>
        /// <param name="operationType">Operation type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task UpdateEntryAsync<TEntity>(TEntity entry, OperationType operationType) where TEntity : BaseEntity
        {
            if (entry is null)
                return;

            var enabled = await _settingService.GetSettingByKeyAsync<bool>($"{nameof(CleverAdsSettings)}.{nameof(CleverAdsSettings.Enabled)}");
            if (!enabled)
                return;

            //if entries are updated frequently, there will be many requests to third-party services,
            //so add a setting to disable these notifications
            var webhooksEnabled = await _settingService
                .GetSettingByKeyAsync<bool>($"{nameof(CleverAdsSettings)}.{nameof(CleverAdsSettings.WebhooksEnabled)}");
            if (!webhooksEnabled)
                return;

            var stores = await _storeService.GetAllStoresAsync();
            var storeIds = stores.Select(store => store.Id).Union(new List<int>() { 0 }).ToList();
            foreach (var storeId in storeIds)
            {
                var clientId = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(CleverAdsSettings)}.{nameof(CleverAdsSettings.ClientId)}", storeId: storeId);
                if (!string.IsNullOrEmpty(clientId))
                    await _cleverAdsService.UpdateEntryAsync(clientId, entry, operationType);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
        {
            await UpdateEntryAsync(eventMessage.Entity, OperationType.Create);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            await UpdateEntryAsync(eventMessage.Entity, OperationType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
        {
            await UpdateEntryAsync(eventMessage.Entity, OperationType.Delete);
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
        {
            await UpdateEntryAsync(eventMessage.Entity, OperationType.Create);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            await UpdateEntryAsync(eventMessage.Entity, OperationType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
        {
            await UpdateEntryAsync(eventMessage.Entity, OperationType.Delete);
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductPicture> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductProductTagMapping> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductProductTagMapping> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductManufacturer> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductManufacturer> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductSpecificationAttribute> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductSpecificationAttribute> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductAttributeMapping> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductAttributeMapping> eventMessage)
        {
            var entry = await _productService.GetProductByIdAsync(eventMessage.Entity?.ProductId ?? 0);
            await UpdateEntryAsync(entry, OperationType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Store> eventMessage)
        {
            if (eventMessage.Entity is not Store store)
                return;

            var enabled = await _settingService
                .GetSettingByKeyAsync<bool>($"{nameof(CleverAdsSettings)}.{nameof(CleverAdsSettings.Enabled)}");
            var clientId = await _settingService
                .GetSettingByKeyAsync<string>($"{nameof(CleverAdsSettings)}.{nameof(CleverAdsSettings.ClientId)}", storeId: store.Id);

            if (enabled && !string.IsNullOrEmpty(clientId))
                await _cleverAdsService.UninstallAsync(clientId);
        }

        #endregion
    }
}