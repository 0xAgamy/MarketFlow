using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Specification;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IGenericRepository<Product>repo) : ControllerBase
    {
        

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type,string? sort)
        {

            var spec = new ProductSepcfication(brand,type,sort);
            var product = await repo.ListAsync(spec);

            return Ok(product);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var prd = await repo.GetByIdAsync(id);
            if (prd is null) return NotFound();

            return prd;
        }


        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            repo.Add(product);
            if (await repo.SaveAllAsync()) {
                return CreatedAtAction("GetProduct", new {id=product.Id},product); 
            }
            return BadRequest("Problem Createing Product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product) {

            if (product.Id != id || !ProductExist(id) ) return BadRequest("Cannot Update this Product");

            repo.Update(product);

           if(await  repo.SaveAllAsync()) {return NoContent();}

            return BadRequest("Problem Updating Product");


        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult>DeleteProduct(int id)
        {
            var prd = await repo.GetByIdAsync(id);

            if (prd is null) return NotFound();


            repo.Remove(prd);

            if (await repo.SaveAllAsync()) { return NoContent(); }
            return BadRequest("Problem Deleteing Product");
        }

        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {

            var spec = new BrandListSpecification();

            return Ok (await repo.ListAsync(spec));
        }
        [HttpGet("Types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec = new TypeListSpecification();
            return Ok (await repo.ListAsync(spec));
        }

        private bool ProductExist(int id)
        {
            return repo.Exist(id);
        }

    }
}
