using charlie.dal.interfaces;
using charlie.dto.User;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
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

            existingUsers.Add(createUser);

            await File.WriteAllTextAsync(getUsersFilePath(), JsonConvert.SerializeObject(existingUsers));
            return createUser;
        }

        public async Task<UserProfile> GetUserProfileByName(string name)
        {
            var list = (await GetUsers()).ToList();
            return list.FirstOrDefault(x => x.Username.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<UserProfile> GetUserProfileById(string id)
        {
            var list = (await GetUsers()).ToList();
            return list.FirstOrDefault(x => x.UserId.Equals(id));
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
