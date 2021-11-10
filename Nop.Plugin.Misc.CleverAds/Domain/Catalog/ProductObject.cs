using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Plugin.Misc.CleverAds.Domain.Catalog
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class ProductObject
    {
        #region Ctor

        public ProductObject(ProductDetailsModel model)
        {
            Id = model.Id;
            Name = model.Name;
            SeName = model.SeName;
            ShortDescription = model.ShortDescription;
            FullDescription = model.FullDescription;
            Sku = model.Sku;
            Mpn = model.ManufacturerPartNumber;
            Gtin = model.Gtin;
            VisibleIndividually = model.VisibleIndividually;
            ProductType = model.ProductType;
            IsShipEnabled = model.IsShipEnabled;
            IsFreeShipping = model.IsFreeShipping;
            DeliveryDate = model.DeliveryDate;
            IsRental = model.IsRental;
            RentalStartDate = model.RentalStartDate;
            RentalEndDate = model.RentalEndDate;
            AvailableEndDate = model.AvailableEndDate;
            ManageInventoryMethod = model.ManageInventoryMethod;
            StockAvailability = model.StockAvailability;
            InStock = model.InStock;
            DefaultPicture = new PictureObject(model.DefaultPictureModel);
            Pictures = model.PictureModels?.Select(item => new PictureObject(item)).ToList();
            Price = new PriceObject(model.ProductPrice);
            Breadcrumb = new ProductBreadcrumbObject(model.Breadcrumb);
            Tags = model.ProductTags?.Select(item => new TagObject(item)).ToList();
            Specifications = model.ProductSpecificationModel?.Groups?.Any(group => group.Attributes?.Any() == true) == true 
                ? new SpecificationsObject(model.ProductSpecificationModel) 
                : null;
            Attributes = model.ProductAttributes?.Select(item => new AttributeObject(item)).ToList();
            Manufacturers = model.ProductManufacturers?.Select(item => new ManufacturerObject(item)).ToList();
            TierPrices = model.TierPrices?.Select(item => new TierPriceObject(item)).ToList();
            AssociatedProducts = model.AssociatedProducts?.Select(item => new ProductObject(item)).ToList();
        }

        #endregion

        #region Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string SeName { get; set; }

        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }

        public string Sku { get; set; }

        public string Mpn { get; set; }

        public string Gtin { get; set; }

        public bool VisibleIndividually { get; set; }

        public ProductType ProductType { get; set; }

        public bool IsShipEnabled { get; set; }

        public bool IsFreeShipping { get; set; }

        public string DeliveryDate { get; set; }

        public bool IsRental { get; set; }

        public DateTime? RentalStartDate { get; set; }

        public DateTime? RentalEndDate { get; set; }

        public DateTime? AvailableEndDate { get; set; }

        public ManageInventoryMethod ManageInventoryMethod { get; set; }

        public string StockAvailability { get; set; }

        public bool InStock { get; set; }

        public PictureObject DefaultPicture { get; set; }

        public IList<PictureObject> Pictures { get; set; }

        public PriceObject Price { get; set; }

        public ProductBreadcrumbObject Breadcrumb { get; set; }

        public IList<TagObject> Tags { get; set; }

        public SpecificationsObject Specifications { get; set; }

        public IList<AttributeObject> Attributes { get; set; }

        public IList<ManufacturerObject> Manufacturers { get; set; }

        public IList<TierPriceObject> TierPrices { get; set; }

        public IList<ProductObject> AssociatedProducts { get; set; }

        #endregion

        #region Nested Classes

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class PictureObject
        {
            #region Ctor

            public PictureObject(PictureModel model)
            {
                ImageUrl = model.ImageUrl;
                ThumbImageUrl = model.ThumbImageUrl;
                FullSizeImageUrl = model.FullSizeImageUrl;
                Title = model.Title;
                AlternateText = model.AlternateText;
            }

            #endregion

            #region Properties

            public string ImageUrl { get; set; }

            public string ThumbImageUrl { get; set; }

            public string FullSizeImageUrl { get; set; }

            public string Title { get; set; }

            public string AlternateText { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class PriceObject
        {
            #region Ctor

            public PriceObject(ProductDetailsModel.ProductPriceModel model)
            {
                CurrencyCode = model.CurrencyCode;
                Price = model.Price;
                PriceValue = model.PriceValue;
                CustomerEntersPrice = model.CustomerEntersPrice;
                CallForPrice = model.CallForPrice;
                OldPrice = model.OldPrice;
                IsRental = model.IsRental;
                RentalPrice = model.RentalPrice;
            }

            #endregion

            #region Properties

            public string CurrencyCode { get; set; }

            public string Price { get; set; }

            public decimal PriceValue { get; set; }

            public bool CustomerEntersPrice { get; set; }

            public bool CallForPrice { get; set; }

            public string OldPrice { get; set; }

            public bool IsRental { get; set; }

            public string RentalPrice { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class ManufacturerObject
        {
            #region Ctor

            public ManufacturerObject(ManufacturerBriefInfoModel model)
            {
                Id = model.Id;
                Name = model.Name;
                SeName = model.SeName;
                IsActive = model.IsActive;
            }

            #endregion

            #region Properties

            public int Id { get; set; }

            public string Name { get; set; }

            public string SeName { get; set; }

            public bool IsActive { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class TagObject
        {
            #region Ctor

            public TagObject(ProductTagModel model)
            {
                Id = model.Id;
                Name = model.Name;
                SeName = model.SeName;
            }

            #endregion

            #region Properties

            public int Id { get; set; }

            public string Name { get; set; }

            public string SeName { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class TierPriceObject
        {
            #region Ctor

            public TierPriceObject(ProductDetailsModel.TierPriceModel model)
            {
                Price = model.Price;
                Quantity = model.Quantity;
            }

            #endregion

            #region Properties

            public string Price { get; set; }

            public int Quantity { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class SpecificationsObject
        {
            #region Ctor

            public SpecificationsObject(ProductSpecificationModel model)
            {
                Groups = model.Groups?
                    .Where(group => group.Attributes?.Any() == true)
                    .Select(item => new SpecificationGroupObject(item))
                    .ToList();
            }

            #endregion

            #region Properties

            public IList<SpecificationGroupObject> Groups { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class SpecificationGroupObject
        {
            #region Ctor

            public SpecificationGroupObject(ProductSpecificationAttributeGroupModel model)
            {
                Name = model.Name;
                Specifications = model.Attributes?.Select(item => new SpecificationObject(item)).ToList();
            }

            #endregion

            #region Properties

            public string Name { get; set; }

            public IList<SpecificationObject> Specifications { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class SpecificationObject
        {
            #region Ctor

            public SpecificationObject(ProductSpecificationAttributeModel model)
            {
                Name = model.Name;
                Values = model.Values?.Select(item => new SpecificationValueObject(item)).ToList();
            }

            #endregion

            #region Properties

            public string Name { get; set; }

            public IList<SpecificationValueObject> Values { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class SpecificationValueObject
        {
            #region Ctor

            public SpecificationValueObject(ProductSpecificationAttributeValueModel model)
            {
                Value = model.ValueRaw;
            }

            #endregion

            #region Properties

            public string Value { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class AttributeObject
        {
            #region Ctor

            public AttributeObject(ProductDetailsModel.ProductAttributeModel model)
            {
                Name = model.Name;
                Description = model.Description;
                TextPrompt = model.TextPrompt;
                IsRequired = model.IsRequired;
                DefaultValue = model.DefaultValue;
                Values = model.Values?.Select(item => new AttributeValueObject(item)).ToList();
            }

            #endregion

            #region Properties

            public string Name { get; set; }

            public string Description { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public string DefaultValue { get; set; }

            public IList<AttributeValueObject> Values { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class AttributeValueObject
        {
            #region Ctor

            public AttributeValueObject(ProductDetailsModel.ProductAttributeValueModel model)
            {
                Name = model.Name;
                PriceAdjustment = model.PriceAdjustment;
                PriceAdjustmentUsePercentage = model.PriceAdjustmentUsePercentage;
                PriceAdjustmentValue = model.PriceAdjustmentValue;
                IsPreSelected = model.IsPreSelected;
                PictureId = model.PictureId;
                CustomerEntersQty = model.CustomerEntersQty;
                Quantity = model.Quantity;
            }

            #endregion

            #region Properties

            public string Name { get; set; }

            public string PriceAdjustment { get; set; }

            public bool PriceAdjustmentUsePercentage { get; set; }

            public decimal PriceAdjustmentValue { get; set; }

            public bool IsPreSelected { get; set; }

            public int PictureId { get; set; }

            public bool CustomerEntersQty { get; set; }

            public int Quantity { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class ProductBreadcrumbObject
        {
            #region Ctor

            public ProductBreadcrumbObject(ProductDetailsModel.ProductBreadcrumbModel model)
            {
                ProductName = model.ProductName;
                ProductSeName = model.ProductSeName;
                CategoryBreadcrumb = model.CategoryBreadcrumb?.Select(item => new CategoryObject(item)).ToList();
            }

            #endregion

            #region Properties

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public IList<CategoryObject> CategoryBreadcrumb { get; set; }

            #endregion
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class CategoryObject
        {
            #region Ctor

            public CategoryObject(CategorySimpleModel model)
            {
                Id = model.Id;
                Name = model.Name;
                SeName = model.SeName;
                Route = model.Route;
                SubCategories = model.SubCategories?.Select(item => new CategoryObject(item)).ToList();
            }

            #endregion

            #region Properties

            public int Id { get; set; }

            public string Name { get; set; }

            public string SeName { get; set; }

            public string Route { get; set; }

            public List<CategoryObject> SubCategories { get; set; }

            #endregion
        }

        #endregion
    }
}