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

        public async Task<ChatRoom> CreateChatRoom(string chatRoomName, string ownerId)
        {
            var chatRoomMetaData = new ChatRoomMetaData() { Name = chatRoomName, OwnerUserId = ownerId, CreatedDate = _time.CurrentDateTime() };
            return await _chatRepo.CreateChatRoom(chatRoomMetaData);
        }

        public async Task<IEnumerable<ChatRoomMetaData>> GetAllMetadata()
        {
            return await _chatRepo.GetAllMetadata();
        }

        public async Task<IEnumerable<MessagePacket>> GetChatRoomChannelHistory(string channelId)
        {
            var chatRoom = await _chatRepo.GetChatRoom(channelId);
            return chatRoom.ChatHistory;
        }

        public async Task<bool> JoinChatRoom(string chatRoomId, string userId)
        {
            var user = await _userRepo.GetUserProfileById(userId);
            user.Channels.Add(chatRoomId);

            var chatRoom = await _chatRepo.GetChatRoom(chatRoomId);
            chatRoom.MetaData.UserIds.Add(userId);

            await Task.WhenAll(
                _chatRepo.SaveChatRoom(chatRoom),
                _userRepo.SaveUser(user)
            );

            return true;
        }

        public async Task<bool> LeaveChatRoom(string chatRoomId, string userId)
        {
            var user = await _userRepo.GetUserProfileById(userId);

            user.Channels.Remove(chatRoomId);
            var chatRoom = await _chatRepo.GetChatRoom(chatRoomId);
            chatRoom.MetaData.UserIds.Remove(userId);

            await Task.WhenAll(
                _chatRepo.SaveChatRoom(chatRoom), 
                _userRepo.SaveUser(user)
            );

            return true;
        }

        public async Task<bool> LeaveChatRooms(IEnumerable<string> chatRoomIds, string userId)
        {
            foreach (var item in chatRoomIds)
            {
                var chatRoom = await _chatRepo.GetChatRoom(item);
                chatRoom.MetaData.UserIds.Remove(userId);
                await _chatRepo.SaveChatRoom(chatRoom);
            }
            return true;
        }

        public async Task<bool> WriteMetadata(IEnumerable<ChatRoomMetaData> data)
        {
            return await _chatRepo.WriteMetadata(data);
        }

        public async Task<bool> SaveMessageToChatRoomChannel(MessagePacket message)
        {
            return await _chatRepo.SaveMessageToChatRoomChannel(message);
        }

    }
}
