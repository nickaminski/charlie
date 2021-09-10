using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto;
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
            _logger.ServerLogInfo(string.Format("creating new user {0}:{1}", createUser.UserId, createUser.Username));
            return await _userRepo.SaveUser(new UserProfile() { 
                UserId = createUser.UserId,
                Username = createUser.Username,
                Channels = new HashSet<string> { "public" },
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                DateLastLoggedIn = DateTime.UtcNow
            });
        }

        public async Task<bool> DeleteUser(string id)
        {
            _logger.ServerLogInfo(string.Format("deleting user {0}", id));
            return await _userRepo.DeleteUser(id);
        }

        public async Task<UserProfile> GetUserById(string id)
        {
            _logger.ServerLogInfo(string.Format("provider getting user {0}", id));
            return await _userRepo.GetUserProfileById(id);
        }

        public async Task<IEnumerable<UserProfile>> GetUsers()
        {
            _logger.ServerLogInfo(string.Format("getting all users"));
            return await _userRepo.GetUsers();
        }

        public async Task<UserProfile> SaveUser(UserProfile user)
        {
            _logger.ServerLogInfo(string.Format("saving user data {0}:{1}", user.UserId, user.Username));
            user.UpdatedDate = DateTime.UtcNow;
            return await _userRepo.SaveUser(user);
        }
    }
}
