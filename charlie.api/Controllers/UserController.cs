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
        public async Task<UserProfile> Get([FromQuery] string id = "", [FromQuery]string username = "")
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(username))
            {
                return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var user = await _userProv.GetUserById(id);
                    if (user != null)
                        return user;
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    var user = await _userProv.GetUserByName(username);
                    if (user != null)
                        return user;
                }

                return null;
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
            var result = await _userProv.CreateUser(createUser);
            return result;
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
