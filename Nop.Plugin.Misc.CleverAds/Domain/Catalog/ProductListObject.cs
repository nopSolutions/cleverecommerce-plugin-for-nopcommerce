using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Misc.CleverAds.Domain.Catalog
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class ProductListObject
    {
        #region Ctor

        public ProductListObject(IPagedList<Product> pagedList, IList<ProductDetailsModel> items)
        {
            PageIndex = pagedList.PageIndex;
            PageSize = pagedList.PageSize;
            TotalCount = pagedList.TotalCount;
            TotalPages = pagedList.TotalPages;

            Items.AddRange(items.Select(item => new ProductObject(item)));
        }

        #endregion

        #region Properties

        public List<ProductObject> Items { get; set; } = new();

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; }

        public int TotalPages { get; }

        #endregion
    }
}