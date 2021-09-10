using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CancelController : ControllerBase
    {
        public CancelController() { }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                await Task.Run(() => {
                    Thread.Sleep(1000);
                });
                Debug.WriteLine("running");
            }
            Debug.WriteLine("canceled");
            return Ok();
        }
    }
}
