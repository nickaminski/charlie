using charlie.dal.interfaces;
using charlie.dto;
using charlie.dto.Chat;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.dal.json_repos
{
    public class ChatRepository : IChatRepository
    {
        private string _path;

        public ChatRepository(IConfiguration config)
        {
            _path = config["ChatPath"];

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            EnsureSystemFilesExist();
        }

        public async Task<ChatRoom> CreateChatRoomAsync(ChatRoomMetaData data)
        {
            data.Id = Guid.NewGuid();
            var newChatRoom = new ChatRoom() { MetaData = data };
            var filePath = getFilePath(data.Id.ToString());
            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(newChatRoom));
            var metadataPath = getFilePath("meta_table");
            var metadata = await GetAllMetadataAsync();
            metadata.ToList().Add(data);
            await WriteMetadataAsync(metadata);
            return newChatRoom;
        }

        public async Task<IEnumerable<ChatRoomMetaData>> GetAllMetadataAsync()
        {
            var jsonString = await File.ReadAllTextAsync(getFilePath("meta_table"));
            return JsonConvert.DeserializeObject<List<ChatRoomMetaData>>(jsonString);
        }

        public async Task<bool> WriteMetadataAsync(IEnumerable<ChatRoomMetaData> data)
        {
            var metadataPath = getFilePath("meta_table");
            await File.WriteAllTextAsync(metadataPath, JsonConvert.SerializeObject(data));
            return true;
        }

        public async Task<ChatRoom> GetChatRoomAsync(string chatRoomId)
        {
            var filePath = getFilePath(chatRoomId);
            if (!File.Exists(filePath))
                return null;

            var jsonString = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<ChatRoom>(jsonString);
        }

        public async Task<bool> SaveChatRoomAsync(ChatRoom data)
        {
            var filePath = getFilePath(data.MetaData.Id?.ToString());
            if (!File.Exists(filePath))
                return false;

            var allMetadata = await GetAllMetadataAsync();
            var idx = allMetadata.ToList().FindIndex(x => x.Id == data.MetaData.Id);
            allMetadata.ToList()[idx] = data.MetaData;
            await WriteMetadataAsync(allMetadata);

            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(data));
            return true;
        }

        public async Task<bool> SaveMessageToChatRoomChannelAsync(MessagePacket message)
        {
            var filePath = getFilePath(message.ChannelId);
            if (!File.Exists(filePath))
                return false;

            message.Id = Guid.NewGuid().ToString();

            var jsonString = await File.ReadAllTextAsync(filePath);
            var chatRoom = JsonConvert.DeserializeObject<ChatRoom>(jsonString);
            chatRoom.ChatHistory = chatRoom.ChatHistory.Append(message).OrderBy(x => x.Timestamp);
            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(chatRoom));
            return true;
        }

        private string getFilePath(string channelId)
        {
            return string.Format("{0}{1}.json", _path, channelId);
        }

        private void EnsureSystemFilesExist()
        {
            var path = getFilePath("meta_table");
            var metadata = new List<ChatRoomMetaData>();
            if (!File.Exists(path))
            {
                metadata.Add(new ChatRoomMetaData() {
                    CreatedDate = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Name = "Public",
                    OwnerUserId = "system",
                    UserIds = new List<string>()
                });
                File.WriteAllText(path, JsonConvert.SerializeObject(metadata));
            }

            if (metadata.Count == 0)
            {
                var jsonString = File.ReadAllText(path);
                metadata.AddRange(JsonConvert.DeserializeObject<List<ChatRoomMetaData>>(jsonString));
            }

            var publicRoom = metadata.FirstOrDefault(x => x.Name == "Public");
            if (publicRoom != null && !File.Exists(getFilePath(publicRoom.Id.ToString())))
            {
                var newChatRoom = new ChatRoom() { MetaData = publicRoom };
                var filePath = getFilePath(publicRoom.Id.ToString());
                File.WriteAllText(filePath, JsonConvert.SerializeObject(newChatRoom));
            }
        }
    }
}
