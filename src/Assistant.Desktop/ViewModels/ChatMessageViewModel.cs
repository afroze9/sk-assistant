using System.ComponentModel;

namespace Assistant.Desktop.ViewModels;

public class ChatMessageViewModel : INotifyPropertyChanged
{
    private string _role;
    public string Role
    {
        get => _role;
        set
        {
            _role = value;
            OnPropertyChanged(nameof(Role));
        }
    }

    private string _message;
    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            OnPropertyChanged(nameof(Message));
        }
    }

    public ChatMessageViewModel()
    {
        _role = string.Empty;
        _message = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}