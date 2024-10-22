using System.IO;
using System.Windows;

using Assistant.Desktop.Configuration;
using Assistant.Desktop.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Assistant.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        ServiceCollection serviceCollection = new ServiceCollection();
        LoadConfiguration(serviceCollection);
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
    }
    
    private void LoadConfiguration(ServiceCollection services)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();
        
        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<IdentityOptions>(options => configuration.GetSection("Identity").Bind(options));
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IGraphService, GraphService>();
    }

    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
