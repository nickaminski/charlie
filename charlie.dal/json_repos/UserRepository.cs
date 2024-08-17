using charlie.dal.interfaces;
using charlie.dto.Card;
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

        public async Task<UserProfile> SaveUserAsync(UserProfile userData)
        {
            var list = (await GetUsersAsync()).ToList();

            if (list.Count == 0)
            {
                list.Add(userData);
            }
            else
            {
                int index = -1;
                index = list.FindIndex(x => x.UserId.CompareTo(userData.UserId) > -1);
                if (index == -1)
                {
                    list.Add(userData);
                }
                else
                {
                    var replaceIndex = list.FindIndex(x => x.UserId.CompareTo(userData.UserId) == 0);
                    if (replaceIndex != -1)
                    {
                        list.RemoveAt(replaceIndex);
                        list.Insert(replaceIndex, userData);
                    }
                    else
                    {
                        list.Insert(index, userData);
                    }
                }
            }
            await File.WriteAllTextAsync(getUsersFilePath(), JsonConvert.SerializeObject(list));
            return userData;
        }

        public async Task<UserProfile> GetUserProfileByNameAsync(string name)
        {
            var list = (await GetUsersAsync()).ToList();
            return list.FirstOrDefault(x => x.Username.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<UserProfile> GetUserProfileByIdAsync(string id)
        {
            var list = (await GetUsersAsync()).ToList();
            return list.FirstOrDefault(x => x.UserId.Equals(id));
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var list = (await GetUsersAsync()).ToList();

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

        public async Task<IEnumerable<UserProfile>> GetUsersAsync()
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
