using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class PollProvider : IPollProvider
    {
        private IPollRepository _pollRepository;

        public PollProvider(IPollRepository pollRepository)
        {
            _pollRepository = pollRepository;
        }
        public async Task<string> CreatePoll(PollViewModel newPoll)
        {
            return await _pollRepository.CreatePoll(new Poll() {
                 id = newPoll.id,
                 expirationDate = newPoll.expirationDate,
                 expirationTime = newPoll.expirationTime,
                 options = newPoll.options,
                 question = newPoll.question
            });
        }

        public async Task<IEnumerable<PollViewModel>> GetAll(string clientIp)
        {
            var polls = await _pollRepository.GetAll();
            var viewModels = new List<PollViewModel>();

            foreach (var item in polls)
            {
                var p = new PollViewModel()
                {
                    id = item.id,
                    expirationDate = item.expirationDate,
                    question = item.question,
                    expirationTime = item.expirationTime,
                    options = item.options
                };

                p.answered = item.options.Where(x => x.respondants.Contains(clientIp)).FirstOrDefault() != null;
                p.totalResponses = item.options.Select(x => x.respondants.Count())
                                               .Sum();

                viewModels.Add(p);
            }

            return viewModels;
        }

        public async Task<PollViewModel> GetPoll(string id, string clientIp)
        {
            var poll = await _pollRepository.GetPoll(id);

            var p = new PollViewModel()
            {
                id = poll.id,
                expirationDate = poll.expirationDate,
                question = poll.question,
                expirationTime = poll.expirationTime,
                options = poll.options
            };

            p.answered = poll.options.Where(x => x.respondants.Contains(clientIp)).FirstOrDefault() != null;
            p.totalResponses = poll.options.Select(x => x.respondants.Count())
                                           .Sum();

            return p;
        }
    }
}
