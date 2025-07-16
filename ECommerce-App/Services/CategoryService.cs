using ECommerce_App.Model;
using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public interface ICategoryService
    {
        Task CreateAsync(Category category);
        Task<bool> CheckCategoryExists(Category category);
        Task<bool> CheckUpdateCategoryExists(Category category);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<bool> DeleteCategory(string id);
        Task<bool> UpdateCategory(string id, Category category);
        Task UpdateProductCountByCategory(string categoryId);
    }

    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMongoCollection<Product> _productCollection;

        public CategoryService(IMongoDatabase database)
        {
            _categoryCollection = database?.GetCollection<Category>("Categories") ??
                throw new ArgumentNullException(nameof(database), "Database cannot be null");

            _productCollection = database.GetCollection<Product>("Products") ??
                throw new InvalidOperationException("Products collection not found");
        }

        public async Task CreateAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            await _categoryCollection.InsertOneAsync(category);
        }

        public async Task UpdateProductCountByCategory(string categoryId)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(categoryId))
                throw new ArgumentException("Category ID cannot be null or empty", nameof(categoryId));

            // Validate collections are initialized
            if (_productCollection == null)
                throw new InvalidOperationException("Product collection is not initialized");

            if (_categoryCollection == null)
                throw new InvalidOperationException("Category collection is not initialized");

            try
            {
                // Count products in this category
                var productCount = await _productCollection
                    .CountDocumentsAsync(p => p.CategoryId == categoryId);

                // Update category's product count
                var filter = Builders<Category>.Filter.Eq(c => c.id, categoryId);
                var update = Builders<Category>.Update.Set(c => c.products, (int)productCount);

                var result = await _categoryCollection.UpdateOneAsync(filter, update);

                if (!result.IsAcknowledged)
                {
                    throw new InvalidOperationException("Database operation was not acknowledged");
                }

                if (result.MatchedCount == 0)
                {
                    // This is not necessarily an error - just means the category had no products
                    // Adjust based on your business requirements
                }
            }
            catch (MongoException ex)
            {
                throw new InvalidOperationException("Database operation failed", ex);
            }
        }
        public async Task<bool> CheckCategoryExists(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var existingCategory = await _categoryCollection
                .Find(c => c.name == category.name)
                .FirstOrDefaultAsync();

            return existingCategory == null?false:true;
        }

        public async Task<bool> CheckUpdateCategoryExists(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var existingCategory = await _categoryCollection
                .Find(c => c.name == category.name ||
                          c.description == category.description)
                .FirstOrDefaultAsync();

            return existingCategory == null;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryCollection.Find(_ => true).ToListAsync();
        }

        public async Task<bool> DeleteCategory(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Category ID cannot be null or empty", nameof(id));

            try
            {
                // First delete products to maintain referential integrity
                await _productCollection.DeleteManyAsync(p => p.CategoryId == id);

                var deleteResult = await _categoryCollection.DeleteOneAsync(c => c.id == id);

                return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateCategory(string id, Category category)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Category ID cannot be null or empty", nameof(id));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var updateResult = await _categoryCollection.ReplaceOneAsync(
                c => c.id == id,
                category,
                new ReplaceOptions { IsUpsert = false });

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}