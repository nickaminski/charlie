using charlie.api.Hubs;
using charlie.bll;
using charlie.bll.interfaces;
using charlie.bll.providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace charlie.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<ILoggerFormatter, LoggerFormatter>();
            services.AddSingleton<ILogWriter, TimedWriter>();

            services.AddHostedService<LoggingService>();

            services.ConfigureBLLServices();

            services.AddTransient<ICardProvider, CardProvider>();
            services.AddTransient<ICardSetProvider, CardSetProvider>();
            services.AddTransient<IDeckProvider, DeckProvider>();
            services.AddTransient<IPollProvider, PollProvider>();
            services.AddTransient<IPollResultsProvider, PollResultsProvider>();
            services.AddTransient<IFormProvider, FormProvider>();
            services.AddTransient<IUserProvider, UserProvider>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://192.168.0.4:8080", "http://192.168.0.4:8181", "http://192.168.0.4:8282")
                    .AllowCredentials());

                options.AddPolicy("DevCors", builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            services.AddSignalR();
            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerGen(setupAction => {
                setupAction.SwaggerDoc(
                    "charlieOpenApiSpec",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "charlie API",
                        Version = "1"
                    });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    setupAction.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseCors("DevCors");

            }
            else
            {
                app.UseCors("CorsPolicy");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSwagger();

            app.UseSwaggerUI(setupAction => {
                setupAction.SwaggerEndpoint(
                    "/swagger/charlieOpenApiSpec/swagger.json", 
                    "charlie API"
                    );
                setupAction.RoutePrefix = "api";
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/chat");
            });
        }
    }
}
