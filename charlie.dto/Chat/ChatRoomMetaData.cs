using System;
using System.Collections.Generic;
using System.Linq;

namespace charlie.dto.Chat
{
    public class ChatRoomMetaData
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OwnerUserId { get; set; }
        public ICollection<string> UserIds { get; set; } = new HashSet<string>();
    }
}
