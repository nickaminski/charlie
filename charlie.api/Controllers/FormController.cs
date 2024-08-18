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
        IFormProvider _formProvider;

        public FormController(IFormProvider formProvider)
        {
            _formProvider = formProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string id)
        {
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
            var result = await _formProvider.CreateForm(data);
            return Ok(string.Format("\"{0}\"", result));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]FormModel data)
        {
            var result = await _formProvider.UpdateForm(data);
            return Ok(result);
        }

    }
}
