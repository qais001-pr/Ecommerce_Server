
using ECommerce_App.Model;
using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public interface IWhishlists
    {
        Task CreateWhishlist(whishlists whishlists);
    }
    public class WishlistsService : IWhishlists
    {
        private readonly IMongoCollection<whishlists> _whishlistsCollection;
        public WishlistsService(IMongoDatabase database)
        {
            _whishlistsCollection = database.GetCollection<whishlists>("wishlists");
        }
        public async Task CreateWhishlist(whishlists whishlists)
        {
            await _whishlistsCollection.InsertOneAsync(whishlists);
        }
    }
}
