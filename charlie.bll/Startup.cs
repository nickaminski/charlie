using charlie.dal.interfaces;
using charlie.dal.json_repos;
using Microsoft.Extensions.DependencyInjection;

namespace charlie.bll
{
    public static class Startup
    {
        public static IServiceCollection ConfigureBLLServices(this IServiceCollection services)
        {
            services.AddTransient<IYGoProRepository, YGoProRepository>();
            services.AddTransient<ICardSetRepository, CardSetRepository>();
            services.AddTransient<ICardRepository, CardRepository>();
            services.AddTransient<IDeckRepository, DeckRepository>();
            services.AddTransient<IPollRepository, PollRepository>();
            services.AddTransient<IFormRepository, FormRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IChatRepository, ChatRepository>();

            return services;
        }
    }
}
