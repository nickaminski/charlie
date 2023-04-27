using System;
using System.Collections.Generic;

namespace charlie.dto.User
{
    public class UpdateUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public ICollection<string> Channels { get; set; }
    }
}
