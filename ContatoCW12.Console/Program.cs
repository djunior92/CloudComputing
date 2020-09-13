using Agenda.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ContatoCW12.WebJob
{
    class Program
    {
        public static ILoggerFactory LoggerFactory;
        public static IConfiguration Configuration;
        public static IServiceProvider ServiceProvider;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var builder = new HostBuilder();

            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
                b.AddTimers();
            });

            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });

            Configuration = LoadConfiguration();

            IServiceCollection services = ConfigureServices();

            services.AddDbContext<AgendaContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<ContatoRepository, ContatoRepository>();
            
            ServiceProvider = services.BuildServiceProvider();

            var host = builder.Build();
            using(host)
            {
                await host.RunAsync();
            }
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection result = new ServiceCollection();

            var config = LoadConfiguration();

            result.AddSingleton(config);

            result.AddLogging();

            result.AddTransient<ContatoRepository, ContatoRepository>();

            return result;
        }
    }
}
