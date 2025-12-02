public class AppState
{
    public User CurrentUser { get; private set; }
    public event Action OnChange;
    public void login(User user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }
    public void logout()
    {
        CurrentUser = null;
        NotifyStateChanged();
    }
    private void NotifyStateChanged() => OnChange?.Invoke();
}