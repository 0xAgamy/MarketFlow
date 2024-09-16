using API.RequestHeplers;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaserApiController:ControllerBase
    {

        protected async Task<ActionResult> CreatePageResult<T>(IGenericRepository<T>repo,
            ISpecification<T>spec,int PageIndex,int PageSieze) where T : BaseEntity
        {
            var items = await repo.ListAsync(spec);

            var count = await repo.CountAsync(spec);

            var pagination = new Pagination<T>(PageIndex,PageSieze,count,items);
            return Ok(pagination);
        }
    }
}
