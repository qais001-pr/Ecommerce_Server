using ECommerce_App.Model;
using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public interface ICategory
    {
        Task CreateAsync(Category category);
        Task<bool> CheckCategoryExists(Category category);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<bool> DeleteCategory(string id);
        Task<bool> UpdateCategory(string id, Category category);


    }
    public class CategoryService : ICategory
    {
        public readonly IMongoCollection<Category> _categoryCollection;
        public CategoryService(IMongoDatabase database)
        {
            _categoryCollection = database.GetCollection<Category>("Categories");
        }



        public async Task CreateAsync(Category category)
        {
            await _categoryCollection.InsertOneAsync(category);
        }



        public async Task<bool> CheckCategoryExists(Category category)
        {
            var existingCategory = await _categoryCollection
                .Find(c => c.name == category.name)
                .FirstOrDefaultAsync();
            if (existingCategory == null)
            {

                return true;
            }
            return false;
        }

        public async Task<bool> CheckUpdateCategoryExists(Category category)
        {
            var existingCategory = await _categoryCollection
                .Find(c => c.name == category.name && c.description == category.description)
                .FirstOrDefaultAsync();
            if (existingCategory == null)
            {

                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryCollection.Find(_ => true).ToListAsync();
        }

        public async Task<bool> DeleteCategory(string id)
        {
            var deleteResult = await _categoryCollection.DeleteOneAsync(m => m.id == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }


        public async Task<bool> UpdateCategory(string id, Category category)
        {
            if (category == null)
                return false;

            var updateResult = await _categoryCollection.ReplaceOneAsync(
                m => m.id == id,
                category,
                new ReplaceOptions { IsUpsert = false }); // Set IsUpsert based on your needs

            return updateResult.IsAcknowledged &&
                   updateResult.ModifiedCount > 0;
        }
    }
}
