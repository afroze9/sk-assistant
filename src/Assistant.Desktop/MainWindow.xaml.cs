using System.Windows;

using Assistant.Desktop.Services;

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

    public MainWindow(IAuthService authService, IGraphService graphService)
    {
        _authService = authService;
        _graphService = graphService;
        InitializeComponent();
    }

    private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        AuthenticationResult? result = await _authService.SignInUserAsync();
        if (result != null)
        {
            UserName.Content = result.Account.Username;
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