using charlie.bll.interfaces;
using charlie.dto.User;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        public async Task<ActionResult<UserProfile>> Get([FromQuery] string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            else
            {
                var user = await _userProv.GetUserById(id);
                if (user != null)
                    return Ok(user);

                return NotFound();
            }
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<UserProfile>> GetUsers()
        {
            return await _userProv.GetUsers();
        }

        [HttpPost]
        public async Task<UserProfile> Create([FromBody] CreateUser createUser)
        {
            if (createUser == null || string.IsNullOrEmpty(createUser.Username))
            {
                return null;
            }
            return await _userProv.CreateUser(createUser);
        }

        [HttpPatch]
        [HttpPut]
        public async Task<UserProfile> Update([FromBody] UpdateUser userProfile)
        {
            if (userProfile == null || string.IsNullOrEmpty(userProfile.Username))
            {
                return null;
            }
            return await _userProv.SaveUser(userProfile);
        }

        [HttpDelete]
        public async Task<bool> Delete([FromQuery] string id)
        {
            return await _userProv.DeleteUser(id);
        }

    }
}
