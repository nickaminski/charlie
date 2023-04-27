﻿using charlie.bll.interfaces;
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
        private IChatProvider _chatProv;

        public UserProvider(IUserRepository userRepo, ILogWriter logger, IChatProvider chatProv)
        {
            _userRepo = userRepo;
            _logger = logger;
            _chatProv = chatProv;
        }

        public async Task<UserProfile> CreateUser(CreateUser createUser)
        {
            _logger.ServerLogInfo("creating new user {0}", createUser.Username);

            var user = await _userRepo.GetUserProfileByName(createUser.Username);

            if (user != null)
                throw new HttpResponseException(400, "User already exists");

            var chatRoomsMeta = (await _chatProv.GetAllMetaData()).ToList();
            var idx = chatRoomsMeta.ToList().FindIndex(x => x.OwnerUserId == "system" && x.Name == "Public");

            var newUser = new UserProfile()
            {
                UserId = Guid.NewGuid().ToString(),
                Username = createUser.Username,
                Channels = new HashSet<string> { chatRoomsMeta[idx].Id.Value.ToString() },
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                DateLastLoggedIn = DateTime.UtcNow
            };

            await _chatProv.JoinChatRoom(chatRoomsMeta[idx].Id.Value.ToString(), newUser.UserId);
            return await _userRepo.SaveUser(newUser);
        }

        public async Task<bool> DeleteUser(string id)
        {
            _logger.ServerLogInfo("deleting user {0}", id);
            var user = await _userRepo.GetUserProfileById(id);
            if (user == null) return false;

            await _chatProv.LeaveChatRooms(user.Channels, id);

            return await _userRepo.DeleteUser(id);
        }

        public async Task<UserProfile> GetUserById(string id)
        {
            _logger.ServerLogInfo("provider getting user by id {0}", id);
            return await _userRepo.GetUserProfileById(id);
        }

        public async Task<UserProfile> GetUserByName(string name)
        {
            _logger.ServerLogInfo("provider getting user by name {0}", name);
            return await _userRepo.GetUserProfileByName(name);
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

            if (user.Channels != null) currentUser.Channels = user.Channels;

            currentUser.UpdatedDate = DateTime.UtcNow;
            return await _userRepo.SaveUser(currentUser);
        }
    }
}
