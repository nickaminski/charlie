using System;
using System.Collections.Generic;

namespace charlie.dto
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public HashSet<string> Channels { get; set; }
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
