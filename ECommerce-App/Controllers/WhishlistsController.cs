using ECommerce_App.Model;
using ECommerce_App.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhishlistsController : ControllerBase
    {
        public readonly IWhishlists whishlists;
        public WhishlistsController(IWhishlists whishlists)
        {
            this.whishlists = whishlists;
        }
        [HttpPost]
        [Route("InsertWhishlists")]
        [Consumes("application/json")]
        public async Task<IActionResult> createwhistslists(whishlistsCreateDTO whishlistsCreateDTO)
        {
            if (string.IsNullOrEmpty(whishlistsCreateDTO.userid) || string.IsNullOrEmpty(whishlistsCreateDTO.productid))
            {
                return BadRequest();
            }
            var w = new whishlists
            {
                isFavourite = true,
                userid = whishlistsCreateDTO.userid,
                productid= whishlistsCreateDTO.productid
            };
            await whishlists.CreateWhishlist(w);
            return Ok(new { status = 201, message = "Whishlists Updated successfully" });
        }
    }
}
