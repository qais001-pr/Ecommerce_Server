using ECommerce_App.Model;
using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public interface IUsers
    {
        Task createUser(User u);
        Task<User> GetUser(string email);
    }
    public class UserServices : IUsers
    {
        public IMongoCollection<User> userCollection;
        public UserServices(IMongoDatabase database)
        {
            userCollection = database.GetCollection<User>("User");
        }

        public async Task<User> GetUser(string email)
        {
            User u = await userCollection.Find(u => u.email == email).FirstOrDefaultAsync();
            return u;
        }
        public async Task createUser(User u)
        {
            await userCollection.InsertOneAsync(u);
        }
        
    }
}
