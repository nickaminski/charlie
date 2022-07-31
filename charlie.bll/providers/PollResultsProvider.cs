using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto.Poll;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class PollResultsProvider : IPollResultsProvider
    {
        private IPollRepository _pollRepository;
        private ILogWriter _logger;

        public PollResultsProvider(IPollRepository pollRepository, ILogWriter logger)
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }

        public async Task<PollResults> GetPollResults(string id, string ipAddress)
        {
            return await _pollRepository.GetPollResults(id, ipAddress);
        }

        public async Task<bool> SubmitResults(string id, string selectedChoice, string ipAddress)
        {
            var poll = await _pollRepository.GetPoll(id);

            var date = poll.expirationDate;
            var time = getTimeSpan(poll);
            date = date.Date + time;

            if (date < DateTime.Now)
            {
                return false;
            }

            foreach (var item in poll.options)
            {
                if (item.respondants.Contains(ipAddress))
                    return false;
            }

            var choice = poll.options.Where(x => x.text == selectedChoice).FirstOrDefault();

            if (choice != null)
            {
                choice.respondants.Add(ipAddress);
                return await _pollRepository.SavePoll(poll);
            }

            return false;
        }

        private TimeSpan getTimeSpan(Poll poll)
        {
            var hours = 0;
            if (poll.expirationTime.hour == 12 && poll.expirationTime.meridiem == "AM")
            {
                hours = 0;
            }
            else
            {
                hours = poll.expirationTime.hour;

                if (poll.expirationTime.hour != 12 && poll.expirationTime.meridiem == "PM")
                {
                    hours += 12;
                }
            }

            return new TimeSpan(hours, poll.expirationTime.minute, 0);
        }
    }
}
