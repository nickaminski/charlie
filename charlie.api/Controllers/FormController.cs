using charlie.bll.interfaces;
using charlie.dto;
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
        public async Task<ActionResult> Get([FromQuery]string id)
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
        public async Task<ActionResult<string>> Post([FromBody]FormModel data)
        {
            _logger.ServerLogInfo("api/Form/Post");
            var result = await _formProvider.CreateForm(data);
            return string.Format("\"{0}\"", result);
        }

        [HttpPut]
        public async Task<bool> Put([FromBody]FormModel data)
        {
            _logger.ServerLogInfo("api/Form/Put");
            return await _formProvider.UpdateForm(data);
        }

    }
}
