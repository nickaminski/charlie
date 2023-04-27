using System.Collections.Generic;
using System.Linq;

namespace charlie.dto.Chat
{
    public class ChatRoom
    {
        public ChatRoomMetaData MetaData { get; set; }
        public IEnumerable<MessagePacket> ChatHistory { get; set; } = Enumerable.Empty<MessagePacket>();
    }
}
