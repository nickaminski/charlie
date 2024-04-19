using System;
using System.Collections.Generic;

namespace charlie.dto.User
{
    public class UserProfile
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public ICollection<string> Channels { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime DateLastLoggedIn { get; set; }
    }

    public class UserComparer : IComparer<UserProfile>
    {
        public int Compare(UserProfile x, UserProfile y)
        {
            return x.UserId.CompareTo(y.UserId.ToString());
        }
    }
}
