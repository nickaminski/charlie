using charlie.api.Filters;
using charlie.api.Hubs;
using charlie.bll;
using charlie.bll.interfaces;
using charlie.bll.providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
            services.AddSingleton<ICachingService, CachingService>();
            services.AddSingleton<ILoggerFormatter, LoggerFormatter>();
            services.AddSingleton<ITimeProvider, TimeProvider>();
            services.AddSingleton<ILogWriter, TimedWriter>();
            services.AddSingleton<IRandomNumberProvider, RandomNumberProvider>();

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
            services.AddDistributedMemoryCache();

            services.AddControllers(options =>
            {
                options.Filters.Add<HttpResponseExceptionFilter>();
            }).AddNewtonsoftJson();

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

            var path = Path.Combine(Directory.GetCurrentDirectory(), Configuration["CardImagesPath"]);
            Directory.CreateDirectory(path);

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(path),
                RequestPath = "/files"
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/chat");
                endpoints.MapHub<CasinoHub>("/casino");
            });
        }
    }
}
