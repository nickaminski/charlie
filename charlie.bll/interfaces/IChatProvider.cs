using charlie.dto;
using charlie.dto.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface IChatProvider
    {
        public Task<ChatRoom> CreateChatRoomAsync(string chatRoomName, string ownerId);
        public Task<bool> JoinChatRoomAsync(string chatRoomId, string userId);
        public Task<bool> LeaveChatRoomAsync(string chatRoomId, string userId);
        public Task<bool> LeaveChatRoomsAsync(IEnumerable<string> chatRoomIds, string userId);
        public Task<IEnumerable<ChatRoomMetaData>> GetAllMetadataAsync();
        public Task<IEnumerable<MessagePacket>> GetChatRoomChannelHistoryAsync(string channelId);
        public Task<bool> SaveMessageToChatRoomChannelAsync(MessagePacket message);
    }
}
