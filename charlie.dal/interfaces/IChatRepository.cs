using charlie.dto;
using charlie.dto.Chat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IChatRepository
    {
        public Task<ChatRoom> CreateChatRoom(ChatRoomMetaData data);
        public Task<bool> SaveChatRoom(ChatRoom data);
        public Task<bool> WriteMetadata(IEnumerable<ChatRoomMetaData> data);
        public Task<List<ChatRoomMetaData>> GetAllMetadata();
        public Task<ChatRoom> GetChatRoom(string channelId);
        public Task<bool> SaveMessageToChatRoomChannel(MessagePacket message);
    }
}
