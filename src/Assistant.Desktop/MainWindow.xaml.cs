using System.Windows;

using Assistant.Desktop.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;

namespace Assistant.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IAuthService _authService;
    private readonly IGraphService _graphService;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(
        IAuthService authService, 
        IGraphService graphService,
        ILogger<MainWindow> logger)
    {
        _authService = authService;
        _graphService = graphService;
        _logger = logger;
        InitializeComponent();
    }

    private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        AuthenticationResult? result = await _authService.SignInUserAsync();
        if (result != null)
        {
            UserName.Content = result.Account.Username;
            _logger.LogInformation("Logged in with {UserName}", result.Account.Username);
        }
    }

    private async void MeButton_OnClick(object sender, RoutedEventArgs e)
    {
        User? user = await _graphService.GetMeAsync();

        if (user != null)
        {
            UserName.Content = user.DisplayName;
        }
    }
}