using Microsoft.AspNetCore.Mvc;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        public StatusController()
        {
        }

        [HttpGet]
        public ActionResult<bool> Get()
        {
            return Ok(true);
        }
    }
}
