using charlie.dto.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IUserRepository
    {
        Task<UserProfile> SaveUser(UserProfile createUser);
        Task<UserProfile> GetUserProfileById(string id);
        Task<UserProfile> GetUserProfileByName(string name);
        Task<bool> DeleteUser(string id);
        Task<IEnumerable<UserProfile>> GetUsers();
    }
}
