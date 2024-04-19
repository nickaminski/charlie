using charlie.bll.interfaces;
using charlie.dto.User;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IUserProvider _userProv;

        public UserController(IUserProvider userProv)
        {
            _userProv = userProv;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string id = "", [FromQuery]string username = "")
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(username))
            {
                return BadRequest();
            }
            else
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var user = await _userProv.GetUserById(id);
                    if (user != null)
                        return Ok(user);
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    var user = await _userProv.GetUserByName(username);
                    if (user != null)
                        return Ok(user);
                }

                return NotFound();
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Signin([FromBody]SigninRequest signinRequest)
        {
            if (string.IsNullOrEmpty(signinRequest.username))
            {
                return BadRequest();
            }
            else
            {
                return Ok(await _userProv.SignIn(signinRequest));
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userProv.GetUsers());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUser createUser)
        {
            if (createUser == null || string.IsNullOrEmpty(createUser.Username))
            {
                return BadRequest();
            }
            var result = await _userProv.CreateUser(createUser);
            return Ok(result);
        }

        [HttpPatch]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUser userProfile)
        {
            if (userProfile == null || string.IsNullOrEmpty(userProfile.Username))
            {
                return BadRequest();
            }
            return Ok(await _userProv.SaveUser(userProfile));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            return Ok(await _userProv.DeleteUser(id));
        }

    }
}
