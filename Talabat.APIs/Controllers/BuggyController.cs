using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{

    public class BuggyController : ApiBaseController
    {
        private readonly StoreContext _dbcontext;

        public BuggyController(StoreContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        [HttpGet("notfound")]  //api/buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = _dbcontext.Products.Find(100);
            if (product is null)
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok(product);
        }
        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            var product = _dbcontext.Products.Find(100);
            var productToReturn = product.ToString();
            return Ok(productToReturn);
        }
        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }
        [HttpGet("badrequest/{id}")]  //api/buggy/badrequest/five
        public ActionResult GetBadRequest(int id)  //validation error
        {
            return Ok();
        }
    }
}
