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
        public IActionResult Get()
        {
            return Ok(true);
        }
    }
}
