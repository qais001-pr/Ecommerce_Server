using ECommerce_App.Model;
using ECommerce_App.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public readonly IOrders orderServices;
        public OrderController(IOrders orderServices)
        {
            this.orderServices = orderServices;
        }
        [HttpPost("CreateOrder")]
        [Consumes("application/json")]
        public async Task<IActionResult> insert(CreateOrderDTO createOrder)
        {
            if (createOrder == null)
            {
                return BadRequest();
            }
            var order = new Order
            {
                name=createOrder.name,
                productid = createOrder.productid,
                userid = createOrder.userid,
                quantity = createOrder.quantity,
                TotalPrice = createOrder.totalprice,
                shippingAddress = createOrder.shippingAddress,
            };
            await orderServices.CreateOrder(order);
            return Ok();
        }
    }
}
