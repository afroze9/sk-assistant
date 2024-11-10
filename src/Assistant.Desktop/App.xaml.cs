using System.IO;
using System.Windows;

using Assistant.Desktop.Configuration;
using Assistant.Desktop.Plugins;
using Assistant.Desktop.Services;
using Assistant.Desktop.State;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.SemanticKernel;

using Qdrant.Client;

using Serilog;
using Serilog.Core;

namespace Assistant.Desktop;

public partial class App : Application
{
    private readonly IHost host;

    public App()
    {
        host = Host.CreateDefaultBuilder()
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
            .ConfigureServices((services) =>
            {
                IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                services.AddSingleton(configuration);
                services.Configure<SKAssistantOptions>(options =>
                    configuration.GetSection("SKAssistant").Bind(options));
                services.Configure<IdentityOptions>(options => configuration.GetSection("Identity").Bind(options));

                IConfigurationSection aiModelSection = configuration.GetRequiredSection("AiModels");
                services.Configure<AiModelOptions>(options => aiModelSection.Bind(options));
                AiModelOptions aiModelOptions = aiModelSection.Get<AiModelOptions>()!;

                services.AddSingleton<App>();
                services.AddSingleton<MainWindow>();

                services.AddSingleton<IAuthService, AuthService>();
                services.AddSingleton<IGraphClientFactory, GraphClientFactory>();
                services
                    .AddOpenAIAudioToText(aiModelOptions.AudioToTextModel, aiModelOptions.Key)
                    .AddOpenAITextToAudio("tts-1", aiModelOptions.Key)
                    .AddOpenAIChatCompletion(aiModelOptions.ChatCompletionModel, aiModelOptions.Key)
                    .AddOpenAITextEmbeddingGeneration(aiModelOptions.EmbeddingGenerationModel, aiModelOptions.Key)
                    .AddKernel();

                services.AddSingleton<KernelPlugin>(sp =>
                    KernelPluginFactory.CreateFromType<GraphPlugin>(serviceProvider: sp));
                services.AddSingleton<KernelPlugin>(sp =>
                    KernelPluginFactory.CreateFromType<DateTimePlugin>(serviceProvider: sp));
                services.AddSingleton<KernelPlugin>(sp =>
                    KernelPluginFactory.CreateFromType<KnowledgePlugin>(serviceProvider: sp));
                services.AddSingleton<QdrantClient>(sp => new QdrantClient("localhost"));
                services.AddQdrantVectorStore("localhost");

                services.AddSingleton<IAiService, AiService>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<IKnowledgeService, KnowledgeService>();
                
                services.AddWpfBlazorWebView();
                services.AddSingleton<AppState>();
                services.AddFluentUIComponents();

                // Enable Developer tools in Debug mode
#if DEBUG
                services.AddBlazorWebViewDeveloperTools();
#endif
            })
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = new MainWindow(host.Services);
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await Log.CloseAndFlushAsync();
        await host.StopAsync();
        host.Dispose();
        base.OnExit(e);
    }
}