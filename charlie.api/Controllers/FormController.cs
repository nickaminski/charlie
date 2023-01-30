using charlie.bll.interfaces;
using charlie.dto.Form;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {

        ILogWriter _logger;
        IFormProvider _formProvider;

        public FormController(ILogWriter logger, IFormProvider formProvider)
        {
            _logger = logger;
            _formProvider = formProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string id)
        {
            _logger.ServerLogInfo("api/Form/Get");
            if (string.IsNullOrEmpty(id))
                return Ok(await _formProvider.GetAll());
            else
            {
                var form = await _formProvider.GetById(id);
                if (form != null)
                    return Ok(form);
                else
                    return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]FormModel data)
        {
            _logger.ServerLogInfo("api/Form/Post");
            var result = await _formProvider.CreateForm(data);
            return Ok(string.Format("\"{0}\"", result));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]FormModel data)
        {
            _logger.ServerLogInfo("api/Form/Put");
            var result = await _formProvider.UpdateForm(data);
            return Ok(result);
        }

    }
}
