using ECommerce_App.Model;
using MongoDB.Bson;
using MongoDB.Driver;

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
        Task<BsonDocument> GetProductDetailsAsync(string productid);
        Task<BsonDocument> searchproducts(string name);

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
        public async Task<BsonDocument?> GetProductDetailsAsync(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));
            }

            if (!ObjectId.TryParse(productId, out var id))
            {
                throw new ArgumentException("Invalid Product ID format", nameof(productId));
            }

            try
            {
                var pipeline = new[]
                {
            new BsonDocument("$match", new BsonDocument("_id", id)),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Categories" },
                { "localField", "categoryid" },
                { "foreignField", "_id" },
                { "as", "Categories" }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Review" },
                { "localField", "_id" },
                { "foreignField", "Productid" },
                { "as", "reviewsData" }
            }),
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$reviewsData" },
                { "preserveNullAndEmptyArrays", true }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "User" },
                { "localField", "reviewsData.Userid" },
                { "foreignField", "_id" },
                { "as", "reviewsData.User" }
            }),
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$reviewsData.User" },
                { "preserveNullAndEmptyArrays", true }
            }),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$_id" },
                { "productDetails", new BsonDocument("$first", "$$ROOT") },
                { "Categories", new BsonDocument("$first", "$Categories") },
                { "reviewsData", new BsonDocument("$push", "$reviewsData") }
            }),
            new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1 },
                { "name", "$productDetails.name" },
                { "description", "$productDetails.description" },
                { "price", "$productDetails.price" },
                { "rating", "$productDetails.rating" },
                { "image", "$productDetails.imageBytes" },
                { "imageContent", "$productDetails.imageContentType" },
                { "quantity", "$productDetails.quantity" },
                { "Categories", 1 },
                { "reviewsData", 1 }
            })
        };

                var result = await _productCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();
                return result.FirstOrDefault();
            }
            catch (MongoException ex)
            {
                throw new ApplicationException("Error accessing database", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred", ex);
            }
        }
        public async Task<BsonDocument> searchproducts(string name)
        {
            try
            {
                var pipeline = new[]
                {
            new BsonDocument("$match",
             new BsonDocument
             (
                 "name",new BsonDocument{{"$regex",$"^{name}"},{"$options","i"} }
             )),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Categories" },
                { "localField", "categoryid" },
                { "foreignField", "_id" },
                { "as", "Categories" }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Review" },
                { "localField", "_id" },
                { "foreignField", "Productid" },
                { "as", "reviewsData" }
            }),
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$reviewsData" },
                { "preserveNullAndEmptyArrays", true }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "User" },
                { "localField", "reviewsData.Userid" },
                { "foreignField", "_id" },
                { "as", "reviewsData.User" }
            }),
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$reviewsData.User" },
                { "preserveNullAndEmptyArrays", true }
            }),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$_id" },
                { "productDetails", new BsonDocument("$first", "$$ROOT") },
                { "Categories", new BsonDocument("$first", "$Categories") },
                { "reviewsData", new BsonDocument("$push", "$reviewsData") }
            }),
            new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1 },
                { "name", "$productDetails.name" },
                { "description", "$productDetails.description" },
                { "price", "$productDetails.price" },
                { "rating", "$productDetails.rating" },
                { "image", "$productDetails.imageBytes" },
                { "imageContent", "$productDetails.imageContentType" },
                { "quantity", "$productDetails.quantity" },
                { "Categories", 1 },
                { "reviewsData", 1 }
            })
        };

                var result = await _productCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();
                return result.FirstOrDefault();
            }
            catch (MongoException ex)
            {
                throw new ApplicationException("Error accessing database", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred", ex);
            }
        }
    }
}