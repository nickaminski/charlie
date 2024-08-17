using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto;
using charlie.dto.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class ChatProvider : IChatProvider
    {
        private ILogWriter _logger;
        private IChatRepository _chatRepo;
        private ITimeProvider _time;
        private IUserRepository _userRepo;

        public ChatProvider(ILogWriter logger, ITimeProvider time, IChatRepository chatRepo, IUserRepository userRepo)
        {
            _logger = logger;
            _chatRepo = chatRepo;
            _time = time;
            _userRepo = userRepo;
        }

        public Task<ChatRoom> CreateChatRoomAsync(string chatRoomName, string ownerId)
        {
            var chatRoomMetaData = new ChatRoomMetaData() { Name = chatRoomName, OwnerUserId = ownerId, CreatedDate = _time.CurrentDateTime() };
            return _chatRepo.CreateChatRoomAsync(chatRoomMetaData);
        }

        public Task<IEnumerable<ChatRoomMetaData>> GetAllMetadataAsync()
        {
            return _chatRepo.GetAllMetadataAsync();
        }

        public async Task<IEnumerable<MessagePacket>> GetChatRoomChannelHistoryAsync(string channelId)
        {
            var chatRoom = await _chatRepo.GetChatRoomAsync(channelId);
            return chatRoom.ChatHistory;
        }

        public async Task<bool> JoinChatRoomAsync(string chatRoomId, string userId)
        {
            var user = await _userRepo.GetUserProfileByIdAsync(userId);
            user.Channels.Add(chatRoomId);

            var chatRoom = await _chatRepo.GetChatRoomAsync(chatRoomId);
            chatRoom.MetaData.UserIds.Add(userId);

            await Task.WhenAll(
                _chatRepo.SaveChatRoomAsync(chatRoom),
                _userRepo.SaveUserAsync(user)
            );

            return true;
        }

        public async Task<bool> LeaveChatRoomAsync(string chatRoomId, string userId)
        {
            var user = await _userRepo.GetUserProfileByIdAsync(userId);

            user.Channels.Remove(chatRoomId);
            var chatRoom = await _chatRepo.GetChatRoomAsync(chatRoomId);
            chatRoom.MetaData.UserIds.Remove(userId);

            await Task.WhenAll(
                _chatRepo.SaveChatRoomAsync(chatRoom), 
                _userRepo.SaveUserAsync(user)
            );

            return true;
        }

        public async Task<bool> LeaveChatRoomsAsync(IEnumerable<string> chatRoomIds, string userId)
        {
            var user = await _userRepo.GetUserProfileByIdAsync(userId);
            foreach (var item in chatRoomIds)
            {
                var chatRoom = await _chatRepo.GetChatRoomAsync(item);
                chatRoom.MetaData.UserIds.Remove(userId);
                user.Channels.Remove(item);
                await _chatRepo.SaveChatRoomAsync(chatRoom);
            }
            await _userRepo.SaveUserAsync(user);
            return true;
        }

        public Task<bool> SaveMessageToChatRoomChannelAsync(MessagePacket message)
        {
            return _chatRepo.SaveMessageToChatRoomChannelAsync(message);
        }

    }
}
