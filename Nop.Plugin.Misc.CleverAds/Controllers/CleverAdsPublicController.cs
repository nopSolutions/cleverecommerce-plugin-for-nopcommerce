using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.CleverAds.Domain.Catalog;
using Nop.Plugin.Misc.CleverAds.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Factories;

namespace Nop.Plugin.Misc.CleverAds.Controllers
{
    [Produces("application/json")]
    public class CleverAdsPublicController : Controller
    {
        #region Fields

        private readonly CleverAdsService _cleverAdsService;
        private readonly CleverAdsSettings _cleverAdsSettings;
        private readonly ILanguageService _languageService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CleverAdsPublicController(CleverAdsService cleverAdsService,
            CleverAdsSettings cleverAdsSettings,
            ILanguageService languageService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _cleverAdsService = cleverAdsService;
            _cleverAdsSettings = cleverAdsSettings;
            _languageService = languageService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProductObject), StatusCodes.Status200OK)]
        public async Task<IActionResult> Product(int id, [FromQuery] int languageId = 0)
        {
            if (!_cleverAdsSettings.Enabled)
                return BadRequest();

            //check request authorization
            var accessToken = Request.Headers[HeaderNames.Authorization].FirstOrDefault();
            if (!string.Equals(accessToken, _cleverAdsSettings.ClientId, StringComparison.InvariantCultureIgnoreCase))
                return new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

            //set request customer
            var customer = await _cleverAdsService.GetOrCreateSystemCustomerAsync();
            await _workContext.SetCurrentCustomerAsync(customer);

            //set request language
            if (languageId > 0)
            {
                var language = await _languageService.GetLanguageByIdAsync(languageId);
                if (language?.Published ?? false)
                    await _workContext.SetWorkingLanguageAsync(language);
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product is null || product.Deleted)
                return NotFound($"Product Id={id} not found");

            //prepare response object
            var model = await _productModelFactory.PrepareProductDetailsModelAsync(product);
            var value = new ProductObject(model);

            return Ok(value);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProductListObject), StatusCodes.Status200OK)]
        public async Task<IActionResult> Products([FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = int.MaxValue,
            [FromQuery] IList<int> categoryIds = null,
            [FromQuery] IList<int> manufacturerIds = null,
            [FromQuery] int vendorId = 0,
            [FromQuery] int warehouseId = 0,
            [FromQuery] ProductType? productType = null,
            [FromQuery] bool visibleIndividuallyOnly = false,
            [FromQuery] bool excludeFeaturedProducts = false,
            [FromQuery] decimal? priceMin = null,
            [FromQuery] decimal? priceMax = null,
            [FromQuery] int productTagId = 0,
            [FromQuery] string keywords = null,
            [FromQuery] bool searchDescriptions = false,
            [FromQuery] bool searchManufacturerPartNumber = true,
            [FromQuery] bool searchSku = true,
            [FromQuery] bool searchProductTags = false,
            [FromQuery] int languageId = 0,
            [FromQuery] ProductSortingEnum orderByType = ProductSortingEnum.Position)
        {
            if (!_cleverAdsSettings.Enabled)
                return BadRequest();

            //check request authorization
            var accessToken = Request.Headers[HeaderNames.Authorization].FirstOrDefault();
            if (!string.Equals(accessToken, _cleverAdsSettings.ClientId, StringComparison.InvariantCultureIgnoreCase))
                return new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

            //set request customer
            var customer = await _cleverAdsService.GetOrCreateSystemCustomerAsync();
            await _workContext.SetCurrentCustomerAsync(customer);

            //set request language
            if (languageId > 0)
            {
                var language = await _languageService.GetLanguageByIdAsync(languageId);
                if (language?.Published ?? false)
                    await _workContext.SetWorkingLanguageAsync(language);
            }

            var store = await _storeContext.GetCurrentStoreAsync();
            var products = await _productService.SearchProductsAsync(pageIndex: pageIndex,
                pageSize: pageSize,
                categoryIds: categoryIds,
                manufacturerIds: manufacturerIds,
                storeId: store.Id,
                vendorId: vendorId,
                warehouseId: warehouseId,
                productType: productType,
                visibleIndividuallyOnly: visibleIndividuallyOnly,
                excludeFeaturedProducts: excludeFeaturedProducts,
                priceMin: priceMin,
                priceMax: priceMax,
                productTagId: productTagId,
                keywords: keywords,
                searchDescriptions: searchDescriptions,
                searchManufacturerPartNumber: searchManufacturerPartNumber,
                searchSku: searchSku,
                searchProductTags: searchProductTags,
                languageId: languageId,
                orderBy: orderByType);

            //prepare response object
            var models = await products
                .SelectAwait(async product => await _productModelFactory.PrepareProductDetailsModelAsync(product))
                .ToListAsync();
            var value = new ProductListObject(products, models);

            return Ok(value);
        }

        #endregion
    }
}