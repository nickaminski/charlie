using charlie.dto;
using charlie.dto.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IChatRepository
    {
        public Task<ChatRoom> CreateChatRoomAsync(ChatRoomMetaData data);
        public Task<bool> SaveChatRoomAsync(ChatRoom data);
        public Task<bool> WriteMetadataAsync(IEnumerable<ChatRoomMetaData> data);
        public Task<IEnumerable<ChatRoomMetaData>> GetAllMetadataAsync();
        public Task<ChatRoom> GetChatRoomAsync(string channelId);
        public Task<bool> SaveMessageToChatRoomChannelAsync(MessagePacket message);
    }
}
