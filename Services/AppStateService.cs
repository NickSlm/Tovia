namespace ToDoListPlus.Services
{
    public class AppStateService
    {
        public event Action? UserLoggedIn;
        public event Action? UserLoggedOut;

        public void SignIn()
        {

            UserLoggedIn?.Invoke();
        }

        public void SignOut()
        {
            UserLoggedOut?.Invoke();
        }
    }
}
