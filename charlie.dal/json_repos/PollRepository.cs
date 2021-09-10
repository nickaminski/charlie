using charlie.dal.interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using charlie.dto;

namespace charlie.dal.json_repos
{
    public class PollRepository : IPollRepository
    {
        private string _path;

        public PollRepository(IConfiguration config)
        {
            _path = config["PollPath"];
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public async Task<string> CreatePoll(Poll poll)
        {
            var filePath = "";

            foreach (var item in poll.options)
            {
                item.respondants = new List<string>();
            }

            do
            {
                poll.id = Guid.NewGuid().ToString();
                filePath = getPollFilePath(poll.id);
            }
            while (File.Exists(filePath));


            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(poll));

            return poll.id;
        }

        public async Task<IEnumerable<Poll>> GetAll()
        {
            List<Poll> polls = new List<Poll>();
            var filePaths = Directory.GetFiles(_path);
            foreach (var file in filePaths)
            {
                var data = await File.ReadAllTextAsync(file);
                try
                {
                    var poll = JsonConvert.DeserializeObject<Poll>(data);
                    polls.Add(poll);
                }
                catch (Exception) 
                {
                }
            }
            return polls;
        }

        public async Task<Poll> GetPoll(string id)
        {
            var path = getPollFilePath(id);

            if (File.Exists(path))
            {
                var data = await File.ReadAllTextAsync(path);
                var poll = JsonConvert.DeserializeObject<Poll>(data);
                return poll;
            }

            return null;
        }

        public async Task<PollResults> GetPollResults(string id, string ipAddress)
        {
            var path = getPollFilePath(id);

            if (File.Exists(path))
            {
                var data = await File.ReadAllTextAsync(path);
                var poll = JsonConvert.DeserializeObject<Poll>(data);

                var pollResults = new PollResults() 
                {
                    pollId = poll.id,
                    question = poll.question
                };

                pollResults.results = new Dictionary<string, int>();

                foreach (var item in poll.options)
                {
                    pollResults.results.Add(item.text, item.respondants.Count());

                    if (item.respondants.Contains(ipAddress))
                    {
                        pollResults.userChoice = item.text;
                    }
                }

                return pollResults;
            }

            return null;
        }

        public async Task<bool> SavePoll(Poll poll)
        {
            var path = getPollFilePath(poll.id);

            if (File.Exists(path))
            {
                await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(poll));
                return true;
            }

            return false;
        }

        private string getPollFilePath(string id)
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            return string.Format("{0}{1}.json", _path, id);
        }

        
    }
}
