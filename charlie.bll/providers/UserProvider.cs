using charlie.bll.interfaces;
using charlie.common.exceptions;
using charlie.dal.interfaces;
using charlie.dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class UserProvider : IUserProvider
    {
        private IUserRepository _userRepo;
        private ILogWriter _logger;
        private IChatRepository _chatRepo;

        public UserProvider(IUserRepository userRepo, ILogWriter logger, IChatRepository chatRepo)
        {
            _userRepo = userRepo;
            _logger = logger;
            _chatRepo = chatRepo;
        }

        public async Task<UserProfile> CreateUser(CreateUser createUser)
        {
            _logger.ServerLogInfo("creating new user {0}", createUser.Username);

            var user = await _userRepo.GetUserProfileByNameAsync(createUser.Username);

            if (user != null)
                throw new HttpResponseException(400, "User already exists");

            var chatRoomsMeta = (await _chatRepo.GetAllMetadataAsync()).ToList();
            var id = chatRoomsMeta.FirstOrDefault(x => x.OwnerUserId == "system" && x.Name == "Public").Id.Value.ToString();

            var newUser = new UserProfile()
            {
                UserId = Guid.NewGuid(),
                Username = createUser.Username,
                Channels = new HashSet<string> { id },
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                DateLastLoggedIn = DateTime.UtcNow
            };
            var chatRoom = await _chatRepo.GetChatRoomAsync(id);
            chatRoom.MetaData.UserIds.Add(newUser.UserId.ToString());

            await Task.WhenAll(
                    _chatRepo.SaveChatRoomAsync(chatRoom),
                    _userRepo.SaveUserAsync(newUser)
                  );

            return newUser;
        }

        public async Task<bool> DeleteUser(string id)
        {
            _logger.ServerLogInfo("deleting user {0}", id);

            var gid = Guid.Parse(id);
            var user = await _userRepo.GetUserProfileByIdAsync(gid);

            if (user == null) return false;

            foreach (var item in user.Channels)
            {
                var chatRoom = await _chatRepo.GetChatRoomAsync(item);
                chatRoom.MetaData.UserIds.Remove(id);
                await _chatRepo.SaveChatRoomAsync(chatRoom);
            }

            return await _userRepo.DeleteUserAsync(gid);
        }

        public async Task<UserProfile> GetUserByIdAsync(string id)
        {
            _logger.ServerLogInfo("provider getting user by id {0}", id);

            return await _userRepo.GetUserProfileByIdAsync(Guid.Parse(id));
        }

        public async Task<UserProfile> GetUserByName(string name)
        {
            _logger.ServerLogInfo("provider getting user by name {0}", name);
            return await _userRepo.GetUserProfileByNameAsync(name);
        }

        public async Task<IEnumerable<UserProfile>> GetUsers()
        {
            _logger.ServerLogInfo("getting all users");
            return await _userRepo.GetUsersAsync();
        }

        public async Task<UserProfile> SaveUser(UpdateUser user)
        {
            var currentUser = await _userRepo.GetUserProfileByIdAsync(Guid.Parse(user.Id));

            if (currentUser == null)
            {
                _logger.ServerLogWarning("did not find user {0}", user.Id);
                return null;
            }

            _logger.ServerLogInfo("saving user data {0}", user.Username);

            if (!string.IsNullOrEmpty(user.Username)) currentUser.Username = user.Username;

            if (user.Channels != null) currentUser.Channels = user.Channels;

            currentUser.UpdatedDate = DateTime.UtcNow;
            return await _userRepo.SaveUserAsync(currentUser);
        }

        public async Task<UserProfile> SignIn(SigninRequest request)
        {
            _logger.ServerLogInfo("signing in {0}", request.username);
            var user = await _userRepo.GetUserProfileByNameAsync(request.username);
            if (user == null) throw new HttpResponseException(404, "user not found");
            user.DateLastLoggedIn = DateTime.Now;
            await _userRepo.SaveUserAsync(user);
            return user;
        }
    }
}
