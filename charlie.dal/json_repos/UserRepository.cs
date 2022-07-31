using charlie.dal.interfaces;
using charlie.dto.User;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.dal.json_repos
{
    public class UserRepository : IUserRepository
    {
        private string _path;

        public UserRepository(IConfiguration config)
        {
            _path = config["UserPath"];

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            if (!File.Exists(getUsersFilePath()))
                File.Create(getUsersFilePath()).Close();
        }

        public async Task<UserProfile> SaveUser(UserProfile createUser)
        {
            var existingUsers = (await GetUsers()).ToList();

            if (existingUsers.Count == 0)
            {
                existingUsers.Add(createUser);
            }
            else
            {
                int index = -1;
                index = existingUsers.FindIndex(x => x.UserId.CompareTo(createUser.UserId) > -1);
                if (index == -1)
                {
                    existingUsers.Add(createUser);
                }
                else
                {
                    var replaceIndex = existingUsers.FindIndex(x => x.UserId.CompareTo(createUser.UserId) == 0);
                    if (replaceIndex != -1)
                    {
                        existingUsers.RemoveAt(replaceIndex);
                        existingUsers.Insert(replaceIndex, createUser);
                    }
                    else
                    {
                        existingUsers.Insert(index, createUser);
                    }
                }
            }
            await File.WriteAllTextAsync(getUsersFilePath(), JsonConvert.SerializeObject(existingUsers));
            return createUser;
        }

        public async Task<UserProfile> GetUserProfileById(string id)
        {
            var list = (await GetUsers()).ToList();

            var index = list.BinarySearch(new UserProfile() { UserId = id }, new UserComparer());
            if (index >= 0)
                return list.ElementAt(index);
            return null;
        }

        public async Task<bool> DeleteUser(string id)
        {
            var list = (await GetUsers()).ToList();

            int index = -1;
            index = list.FindIndex(x => x.UserId.CompareTo(id) == 0);
            if (index != -1)
            {
                list.RemoveAt(index);
            }
            else
            {
                return false;
            }

            await File.WriteAllTextAsync(getUsersFilePath(), JsonConvert.SerializeObject(list));
            return true;
        }

        public async Task<IEnumerable<UserProfile>> GetUsers()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            var filePath = getUsersFilePath();
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            var jsonString = await File.ReadAllTextAsync(filePath);
            var list = JsonConvert.DeserializeObject<List<UserProfile>>(jsonString);
            if (list == null)
                list = new List<UserProfile>();

            return list;
        }

        private string getUsersFilePath()
        {
            return _path + "users.json";
        }
    }
}
