using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Core.Specification;
namespace API.Controllers
{

    public class ProductsController(IGenericRepository<Product>repo) : BaseApiController
    {
        

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
        {

            var spec = new ProductSpecification(specParams);


            return await CreatePageResult(repo, spec, specParams.PageIndex, specParams.PageSize); 
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
            return BadRequest("Problem Creating Product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product) {

            if (product.Id != id ||  ! await ProductExist(id) ) return BadRequest("Cannot Update this Product");

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
            return BadRequest("Problem Deleting Product");
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

        private async Task< bool> ProductExist(int id)
        {
            return await repo.Exist(id);
        }

    }
}
