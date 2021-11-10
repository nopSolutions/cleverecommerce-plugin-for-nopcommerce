using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;

namespace Nop.Plugin.Misc.CleverAds.Domain.CleverAds
{
    /// <summary>
    /// Represents request to update entry details
    /// </summary>
    public class UpdateRequest<TEntity> : AuthorizedRequest where TEntity : BaseEntity
    {
        public UpdateRequest(string authorization, TEntity entry, OperationType operationType)
        {
            Authorization = authorization;
            Id = entry.Id.ToString();
            EntityName = typeof(TEntity).Name.ToLowerInvariant();
            OperationTypeName = operationType.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Gets or sets the client identifier
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the entry identifier
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the platform name
        /// </summary>
        [JsonProperty(PropertyName = "platform")]
        public string PlatformName { get; set; }

        /// <summary>
        /// Gets the operation type name
        /// </summary>
        [JsonIgnore]
        public string OperationTypeName { get; set; }

        /// <summary>
        /// Gets the entity name
        /// </summary>
        [JsonIgnore]
        public string EntityName { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"api/nopcommerce/{OperationTypeName}_{EntityName}_webhook";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;
    }
}