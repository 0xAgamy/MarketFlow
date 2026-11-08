using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CartController(ICartService cartServices):BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
        {
            var cart = await cartServices.GetCartAsync(id);
            return Ok(cart ?? new ShoppingCart { Id=id} );
        }
        [HttpPost]

        public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
        {
            var updatedCart = await cartServices.SetCartAsync(cart);

            if (updatedCart == null) return BadRequest("Problem with Cart");
            return updatedCart;


        }

        [HttpDelete]
        public async Task<ActionResult>DeletesCarts(string id)
        {
            var result = await cartServices.DeleteCartAsync(id);
            if (!result) return BadRequest("Problem Deleting Cart");
            return Ok();
        }

        //TODO: Implement checkout with price validation
    }
}
