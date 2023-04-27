using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto;
using charlie.dto.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class ChatProvider : IChatProvider
    {
        private ILogWriter _logger;
        private IChatRepository _chatRepo;
        private ITimeProvider _time;

        public ChatProvider(ILogWriter logger, ITimeProvider time, IChatRepository chatRepo)
        {
            _logger = logger;
            _chatRepo = chatRepo;
            _time = time;
        }

        public async Task<ChatRoom> CreateChatRoom(string chatRoomName, string ownerId)
        {
            var chatRoomMetaData = new ChatRoomMetaData() { Name = chatRoomName, OwnerUserId = ownerId, CreatedDate = _time.CurrentDateTime() };
            return await _chatRepo.CreateChatRoom(chatRoomMetaData);
        }

        public async Task<IEnumerable<ChatRoomMetaData>> GetAllMetaData()
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
            var chatRoom = await _chatRepo.GetChatRoom(chatRoomId);
            chatRoom.MetaData.UserIds.Add(userId);
            await _chatRepo.SaveChatRoom(chatRoom);
            return true;
        }

        public async Task<bool> LeaveChatRoom(string chatRoomId, string userId)
        {
            var chatRoom = await _chatRepo.GetChatRoom(chatRoomId);
            chatRoom.MetaData.UserIds.Remove(userId);
            await _chatRepo.SaveChatRoom(chatRoom);
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
