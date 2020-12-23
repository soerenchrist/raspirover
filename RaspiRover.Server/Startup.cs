using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RaspiRover.GPIO.Config;
using RaspiRover.Server.Hubs;
using System.IO;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RaspiRover.Server
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
            Pi.Init<BootstrapWiringPi>();

            var roverConfig = ParseConfig();
            services.AddSingleton(_ => roverConfig);
            services.AddSignalR(o =>
            {

                o.EnableDetailedErrors = true;
                o.MaximumReceiveMessageSize = 102400;
            });
            services.AddControllers();
            services.AddHostedService<RaspberryHubClient>();
        }

        private RoverConfiguration ParseConfig()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            var path = Configuration.GetValue<string>("ConfigurationPath");
            var content = File.ReadAllText(path);

            return deserializer.Deserialize<RoverConfiguration>(content);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ControlHub>("/control");
            });
        }
    }
}
