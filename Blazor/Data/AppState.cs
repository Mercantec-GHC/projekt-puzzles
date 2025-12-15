/// <summary>
/// Manages the application's user authentication state and notifies subscribers when the state changes.
/// </summary>
public class AppState
{
    /// <summary>
    /// Gets the currently authenticated user. Null if no user is logged in.
    /// </summary>
    public User CurrentUser { get; private set; }

    /// <summary>
    /// Event triggered whenever the authentication state changes (login or logout).
    /// Subscribers can use this to update UI or perform actions on state change.
    /// </summary>
    public event Action OnChange;

    /// <summary>
    /// Logs in a user and updates the authentication state.
    /// </summary>
    /// <param name="user">The user to log in.</param>
    public void login(User user)
    {
        // Set the current user to the provided user instance
        CurrentUser = user;
        // Notify all subscribers that the state has changed
        NotifyStateChanged();
    }

    /// <summary>
    /// Logs out the current user and updates the authentication state.
    /// </summary>
    public void logout()
    {
        // Clear the current user
        CurrentUser = null;
        // Notify all subscribers that the state has changed
        NotifyStateChanged();
    }

    /// <summary>
    /// Invokes the <see cref="OnChange"/> event to notify subscribers of a state change.
    /// </summary>
    private void NotifyStateChanged() => OnChange?.Invoke();
}