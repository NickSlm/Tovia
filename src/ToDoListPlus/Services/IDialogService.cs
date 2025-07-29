namespace ToDoListPlus.Services
{
    public interface IDialogService
    {
        bool? ShowLoginDialog();
        void ShowMessage(string message, string title);
    }
}
