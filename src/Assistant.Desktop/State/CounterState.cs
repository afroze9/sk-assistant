namespace Assistant.Desktop.State;

public class CounterState
{
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
}