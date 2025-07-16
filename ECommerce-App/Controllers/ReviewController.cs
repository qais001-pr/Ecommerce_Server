using ECommerce_App.Model;
using ECommerce_App.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        public IReview reviewServices;
        public ReviewController(IReview _reviewService)
        {
            this.reviewServices = _reviewService; 
        }
        [HttpPost("Create")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateReview(CreateReviewDTO review)
        {
            if (string.IsNullOrEmpty(review.productid) ||
                string.IsNullOrWhiteSpace(review.userid) ||
                string.IsNullOrWhiteSpace(review.comments)
                )
                return BadRequest(new { status = 400, message = "Fill all the fields completely" });

            var rev = new Review
            {
                comments = review.comments,
                productid = review.productid,
                userid = review.userid,
            };
            await reviewServices.CreateReviewAsync(rev);
            return Ok();
        }
    }
}
