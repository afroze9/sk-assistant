using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Assistant.Desktop.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ObservableCollection<ChatMessageViewModel> _chatMessages;
    public ObservableCollection<ChatMessageViewModel> ChatMessages
    {
        get => _chatMessages;
        set
        {
            _chatMessages = value;
            OnPropertyChanged(nameof(ChatMessages));
        }
    }

    public MainWindowViewModel()
    {
        _chatMessages = new ObservableCollection<ChatMessageViewModel>();
        _chatMessages.Add(new ChatMessageViewModel()
        {
            Message = "You are an efficient assistant and respond with only whats needed and nothing else",
            Role = "System",
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}