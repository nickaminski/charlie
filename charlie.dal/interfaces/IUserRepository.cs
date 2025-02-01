using charlie.dto.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IUserRepository
    {
        Task<UserProfile> SaveUserAsync(UserProfile createUser);
        Task<UserProfile> GetUserProfileByIdAsync(Guid id);
        Task<UserProfile> GetUserProfileByNameAsync(string name);
        Task<bool> DeleteUserAsync(Guid id);
        Task<IEnumerable<UserProfile>> GetUsersAsync();
    }
}
