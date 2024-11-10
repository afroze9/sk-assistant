using Microsoft.FluentUI.AspNetCore.Components;

namespace Assistant.Desktop.State;

public class AppState
{
    private DesignThemeModes currentThemeMode = DesignThemeModes.Dark;
    private UserState userState = new();
   
    public DesignThemeModes CurrentThemeMode
    {
        get => currentThemeMode;
        set
        {
            currentThemeMode = value;
            OnChange?.Invoke();
        }
    }

    public UserState GetUserState() => userState;

    public void SetUserState(string displayName, string email)
    {
        userState.UserDisplayName = displayName;
        userState.UserEmail = email;
        OnChange?.Invoke();
    }
    
    public event Action? OnChange;
}

public class UserState
{
    public string UserEmail { get; set; } = string.Empty;
    
    public string UserDisplayName { get; set; } = string.Empty;
}