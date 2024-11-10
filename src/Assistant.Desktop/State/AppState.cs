using Assistant.Desktop.Entities;

using Microsoft.FluentUI.AspNetCore.Components;

using Message = Assistant.Desktop.Entities.Message;

namespace Assistant.Desktop.State;

public class AppState
{
    private DesignThemeModes _currentThemeMode = DesignThemeModes.Dark;
    private UserState _userState = new();
    private List<Conversation> _conversations = [];
    private Conversation? _currentConversation = new ();
   
    public DesignThemeModes CurrentThemeMode
    {
        get => _currentThemeMode;
        set
        {
            _currentThemeMode = value;
            OnChange?.Invoke();
        }
    }

    public UserState GetUserState() => _userState;

    public void SetUserState(string displayName, string email)
    {
        _userState.UserDisplayName = displayName;
        _userState.UserEmail = email;
        OnChange?.Invoke();
    }
    
    public IReadOnlyList<Conversation> Conversations => _conversations.AsReadOnly();
    
    public void SetConversations(List<Conversation> conversations)
    {
        _conversations = conversations;
        OnChange?.Invoke();
    }
    
    public void ClearConversations()
    {
        _conversations.Clear();
        OnChange?.Invoke();
    }
    
    public Conversation? GetCurrentConversation() => _currentConversation;
    
    public void SetCurrentConversation(Conversation conversation)
    {
        _currentConversation = conversation;
        OnChange?.Invoke();
    }
    
    public void AddCurrentConversationToList(Conversation conversation)
    {
        _conversations.Add(conversation);
        OnChange?.Invoke();
    }
    
    public void AddMessageToCurrentConversation(Message message)
    {
        if (_currentConversation is not null)
        {
            _currentConversation.Messages.Add(message);
            OnChange?.Invoke();
        }
    }
    
    public event Action? OnChange;
}

public class UserState
{
    public string UserEmail { get; set; } = string.Empty;
    
    public string UserDisplayName { get; set; } = string.Empty;
}