using System.IO;

using Assistant.Desktop.Configuration;
using Assistant.Desktop.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Serilog;
using Serilog.Core;

namespace Assistant.Desktop;

public class Program
{
    [STAThread]
    public static void Main()
    {
        IHost host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration =>
            {
                configuration
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
            })
            .ConfigureLogging(logging =>
            {
                Logger logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File("logs/sk-assistant.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                logging.ClearProviders();
                logging.AddSerilog(logger);
            })
            .ConfigureServices(services =>
            {
                IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                services.AddSingleton(configuration);
                services.Configure<IdentityOptions>(options => configuration.GetSection("Identity").Bind(options));

                IConfigurationSection aiModelSection = configuration.GetRequiredSection("AiModels");
                services.Configure<AiModelOptions>(options => aiModelSection.Bind(options));

                AiModelOptions aiModelOptions = aiModelSection.Get<AiModelOptions>()!;

                services.AddSingleton<App>();
                services.AddSingleton<MainWindow>();

                services.AddSingleton<IAuthService, AuthService>();
                services.AddSingleton<IGraphService, GraphService>();
                services
                    .AddOpenAIAudioToText(aiModelOptions.AudioToTextModel, aiModelOptions.Key)
                    .AddOpenAIChatCompletion(aiModelOptions.ChatCompletionModel, aiModelOptions.Key)
                    .AddKernel();

                services.AddSingleton<MainWindow>();
            })
            .Build();

        try
        {
            App app = host.Services.GetRequiredService<App>();
            app.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}