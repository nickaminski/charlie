using charlie.dto.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface IUserProvider : IProvider<UserProfile>
    {
        Task<UserProfile> GetUserById(string id);
        Task<UserProfile> CreateUser(CreateUser createUser);
        Task<UserProfile> SaveUser(UpdateUser updateUser);
        Task<bool> DeleteUser(string id);
        Task<UserProfile> GetUserByName(string name);
        Task<IEnumerable<UserProfile>> GetUsers();
    }
}
