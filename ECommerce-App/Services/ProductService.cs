using ECommerce_App.Model;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ECommerce_App.Services
{
    public interface IProduct
    {
        Task CreateAsync(Product product);
        Task updateProduct(string id, Product p);
        Task<IEnumerable<Product>> GetAllProducts();
        Task DeleteProduct(string id);
        Task<Product?> GetProductByid(string id);

        Task<IEnumerable<Product>> GetProductsByCategoriesID(string categoryid);
        Task<string?> GetProductDetailsJsonAsync(string productid);

    }
    public class ProductService : IProduct
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

        public async Task updateProduct(string id, Product p)
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

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                return await _productCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving products: {ex.Message}");
            }
        }


        public async Task DeleteProduct(string id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var result = await _productCollection.DeleteOneAsync(filter);
            if (result.DeletedCount == 0)
            {
                throw new Exception($"Product with ID {id} not found.");
            }
        }

        public async Task<Product?> GetProductByid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var product = await _productCollection.Find(filter).FirstOrDefaultAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoriesID(string categoryid)
        {
            var products = await _productCollection.Find(c => c.CategoryId == categoryid).ToListAsync();
            return products;
        }


        public async Task<string?> GetProductDetailsJsonAsync(string productid)
        {
            var id = new ObjectId(productid);
            var pipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument("_id", id)),
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "Category" },
            { "localField", "categoryid" },
            { "foreignField", "_id" },
            { "as", "categories" }
        }),
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "Review" },
            { "localField", "_id" },
            { "foreignField", "Productid" },
            { "as", "Reviews" }
        })
    };

            var cursor = await _productCollection.AggregateAsync<BsonDocument>(pipeline);
            var list = await cursor.ToListAsync();

            var doc = list.FirstOrDefault();

            if (doc == null)
                return null;

            // Return pretty formatted JSON string
            return doc.ToJson(new JsonWriterSettings { Indent = true });
        }
    }
}