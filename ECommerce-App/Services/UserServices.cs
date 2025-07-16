using ECommerce_App.Model;
using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public interface IUsers
    {
        Task createUser(User u);
    }
    public class UserServices : IUsers
    {
        public IMongoCollection<User> userCollection;
        public UserServices(IMongoDatabase database)
        {
            userCollection = database.GetCollection<User>("User");
        }

        public async Task createUser(User u)
        {
            await userCollection.InsertOneAsync(u);
        }
    }
}
