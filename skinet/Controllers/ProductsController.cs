using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext _context;

        public ProductsController(StoreContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var prd = await _context.Products.FindAsync(id);
            if (prd is null) return NotFound();

            return prd;
        }


        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product) {

            if (product.Id != id || !ProductExist(id) ) return BadRequest("Cannot Update this Product");

            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();


        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult>DeleteProduct(int id)
        {
            var prd = await _context.Products.FindAsync(id);

            if (prd is null) return NotFound();


            _context.Products.Remove(prd);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProductExist(int id)
        {
            return _context.Products.Any(p=>p.Id == id);
        }

    }
}
