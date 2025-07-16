using ECommerce_App.Model;
using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public interface IReview
    {
        Task CreateReviewAsync(Review review);
    }
    public class ReviewService:IReview
    {
        public readonly IMongoCollection<Review> _revierwCollection;

        public ReviewService(IMongoDatabase database) {

            _revierwCollection = database.GetCollection<Review>("Review");
        }

        public async Task CreateReviewAsync(Review review)
        {
            await _revierwCollection.InsertOneAsync(review);
        }
    }
}
