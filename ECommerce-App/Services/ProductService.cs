using ECommerce_App.Model;
using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public interface IProduct
    {
        Task CreateAsync(Product product);
        Task updateProduct(string id, Product p);
    }
    public class ProductService:IProduct
    {
        private readonly IMongoCollection<Product> _productCollection;
        public ProductService(IMongoDatabase database)
        {
            _productCollection = database.GetCollection<Product>("Products");
        }
        public async Task CreateAsync(Product product)
        {
            await _productCollection.InsertOneAsync(product);
        }

        public async Task updateProduct(string id,Product p)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var update = Builders<Product>.Update
                .Set(p => p.Name, p.Name)
                .Set(p => p.Description, p.Description)
                .Set(p => p.Price, p.Price)
                .Set(p => p.Quantity, p.Quantity)
                .Set(p => p.CategoryId, p.CategoryId)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);
            if (p.ImageBytes != null && p.ImageContentType != null)
            {
                update = update
                    .Set(p => p.ImageBytes, p.ImageBytes)
                    .Set(p => p.ImageContentType, p.ImageContentType)
                    .Set(p => p.ImageExtension, p.ImageExtension);
            }
            await _productCollection.UpdateOneAsync(filter, update);
        }
    }
}
