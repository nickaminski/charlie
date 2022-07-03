using charlie.bll.interfaces;
using charlie.dto;
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
        private ILogWriter _logger;

        public UserController(IUserProvider userProv, ILogWriter logger)
        {
            _userProv = userProv;
            _logger = logger;
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
                _logger.ServerLogInfo(string.Format("User Controller Get"));
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
        public async Task<UserProfile> Post([FromBody] CreateUser createUser)
        {
            if (createUser == null || string.IsNullOrEmpty(createUser.UserId) || string.IsNullOrEmpty(createUser.Username))
            {
                return null;
            }
            return await _userProv.CreateUser(createUser);
        }

        [HttpPut]
        public async Task<UserProfile> Put([FromBody] UserProfile userProfile)
        {
            if (userProfile == null || string.IsNullOrEmpty(userProfile.UserId) || string.IsNullOrEmpty(userProfile.Username))
            {
                return null;
            }
            return await _userProv.SaveUser(userProfile);
        }

        [HttpPatch]
        public async Task<UserProfile> Patch([FromBody] UserProfile userProfile)
        {
            if (userProfile == null || string.IsNullOrEmpty(userProfile.UserId) || string.IsNullOrEmpty(userProfile.Username))
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
