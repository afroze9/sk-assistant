using System.IO;
using System.Windows;
using System.Windows.Input;

using Assistant.Desktop.Services;
using Assistant.Desktop.ViewModels;

using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToAudio;

using NAudio.Wave;

namespace Assistant.Desktop;

public partial class MainWindow : Window
{
    private readonly IAuthService _authService;
    private readonly IAiService _aiService;
    private readonly ITextToAudioService _textToAudioService;
    private readonly ILogger<MainWindow> _logger;
    private readonly MainWindowViewModel _viewModel;

    public MainWindow(
        IAuthService authService,
        IAiService aiService,
        ITextToAudioService textToAudioService,
        ILogger<MainWindow> logger)
    {
        _authService = authService;
        _aiService = aiService;
        _textToAudioService = textToAudioService;
        _logger = logger;
        _viewModel = new MainWindowViewModel();

        InitializeComponent();

        DataContext = _viewModel;
    }

    private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        AuthenticationResult? result = await _authService.SignInUserAsync();

        if (result != null)
        {
            _logger.LogInformation("Logged in with {UserName}", result.Account.Username);
        }
    }

    private void ChatInputTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SendButton_OnClick(sender, e);
        }
    }

    private async void SendButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(ChatInputTextBox.Text))
        {
            _viewModel.ChatMessages.Add(new ChatMessageViewModel
            {
                Message = ChatInputTextBox.Text.Trim(), 
                Role = "User",
            });

            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChatInputTextBox.Clear();
                    ChatInputTextBox.Focus();
                    ChatListBox.ScrollIntoView(_viewModel.ChatMessages.Last());
                    SendButton.IsEnabled = false;
                });
            });
            
            await Task.Run(async () =>
            {
                ChatMessageViewModel response = await _aiService.GenerateAsync(_viewModel.ChatMessages.ToList());
                AudioContent audio = await _textToAudioService.GetAudioContentAsync(response.Message, new OpenAITextToAudioExecutionSettings()
                {
                    Voice = "nova",
                    ResponseFormat = "mp3",
                });
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _viewModel.ChatMessages.Add(response);
                    ChatListBox.ScrollIntoView(_viewModel.ChatMessages.Last());
                    SendButton.IsEnabled = true;
                });

                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    using (var stream = new MemoryStream(audio.Data?.ToArray() ?? []))
                    await using (var reader = new Mp3FileReader(stream))
                    using (var waveOut = new WaveOutEvent())
                    {
                        waveOut.Init(reader);
                        waveOut.Play();

                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            await Task.Delay(100);
                        }
                    }
                });
            });
        }
    }
}