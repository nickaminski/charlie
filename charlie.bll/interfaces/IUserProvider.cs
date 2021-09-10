using charlie.dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface IUserProvider : IProvider<UserProfile>
    {
        Task<UserProfile> GetUserById(string id);
        Task<UserProfile> CreateUser(CreateUser createUser);
        Task<UserProfile> SaveUser(UserProfile deck);
        Task<bool> DeleteUser(string id);
        Task<IEnumerable<UserProfile>> GetUsers();
    }
}
