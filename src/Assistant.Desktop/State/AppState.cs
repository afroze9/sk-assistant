using Microsoft.FluentUI.AspNetCore.Components;

namespace Assistant.Desktop.State;

public class AppState
{
    private DesignThemeModes currentThemeMode = DesignThemeModes.Dark;

    public DesignThemeModes CurrentThemeMode
    {
        get => currentThemeMode;
        set
        {
            currentThemeMode = value;
            OnChange?.Invoke();
        }
    }
    
    
    private int currentCount = 0;
    public int CurrentCount => currentCount;
    
    public event Action? OnChange;
    
    public void IncrementCount()
    {
        currentCount++;
        OnChange?.Invoke();
    }
    
    public void ResetCount()
    {
        currentCount = 0;
        OnChange?.Invoke();
    }
    
    public void SetThemeMode(DesignThemeModes themeMode)
    {
        currentThemeMode = themeMode;
        OnChange?.Invoke();
    }
}