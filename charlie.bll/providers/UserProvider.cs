using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class UserProvider : IUserProvider
    {
        private IUserRepository _userRepo;
        private ILogWriter _logger;

        public UserProvider(IUserRepository userRepo, ILogWriter logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<UserProfile> CreateUser(CreateUser createUser)
        {
            _logger.ServerLogInfo("creating new user {0}", createUser.Username);
            return await _userRepo.SaveUser(new UserProfile() { 
                UserId = Guid.NewGuid().ToString(),
                Username = createUser.Username,
                Channels = new HashSet<string> { "public" },
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                DateLastLoggedIn = DateTime.UtcNow
            });
        }

        public async Task<bool> DeleteUser(string id)
        {
            _logger.ServerLogInfo("deleting user {0}", id);
            return await _userRepo.DeleteUser(id);
        }

        public async Task<UserProfile> GetUserById(string id)
        {
            _logger.ServerLogInfo("provider getting user {0}", id);
            return await _userRepo.GetUserProfileById(id);
        }

        public async Task<IEnumerable<UserProfile>> GetUsers()
        {
            _logger.ServerLogInfo("getting all users");
            return await _userRepo.GetUsers();
        }

        public async Task<UserProfile> SaveUser(UpdateUser user)
        {
            var currentUser = await _userRepo.GetUserProfileById(user.Id.ToString());

            if (currentUser == null)
            {
                _logger.ServerLogWarning("did not find user {0}", user.Id);
                return null;
            }

            _logger.ServerLogInfo("saving user data {0}", user.Username);

            if (!string.IsNullOrEmpty(user.Username)) currentUser.Username = user.Username;

            currentUser.UpdatedDate = DateTime.UtcNow;
            return await _userRepo.SaveUser(currentUser);
        }
    }
}
