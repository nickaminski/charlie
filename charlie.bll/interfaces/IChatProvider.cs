using charlie.dto;
using charlie.dto.Chat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface IChatProvider
    {
        public Task<ChatRoom> CreateChatRoom(string chatRoomName, string ownerId);
        public Task<bool> JoinChatRoom(string chatRoomId, string userId);
        public Task<bool> LeaveChatRoom(string chatRoomId, string userId);
        public Task<bool> LeaveChatRooms(IEnumerable<string> chatRoomIds, string userId);
        public Task<IEnumerable<ChatRoomMetaData>> GetAllMetadata();
        public Task<bool> WriteMetadata(IEnumerable<ChatRoomMetaData> data);
        public Task<IEnumerable<MessagePacket>> GetChatRoomChannelHistory(string channelId);
        public Task<bool> SaveMessageToChatRoomChannel(MessagePacket message);
    }
}
