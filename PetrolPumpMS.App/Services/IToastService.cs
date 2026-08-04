namespace PetrolPumpMS.App.Services;

public enum ToastKind { Success, Error, Info, Warning }

public record ToastMessage(string Text, ToastKind Kind);

/// <summary>
/// Non-blocking toast/snackbar notifications, shown by a ItemsControl bound to
/// Messages in the shell (MainWindow), instead of modal MessageBox popups.
/// </summary>
public interface IToastService
{
    event Action<ToastMessage>? MessagePosted;
    void Show(string text, ToastKind kind = ToastKind.Info);
    void Success(string text) => Show(text, ToastKind.Success);
    void Error(string text) => Show(text, ToastKind.Error);
}

public class ToastService : IToastService
{
    public event Action<ToastMessage>? MessagePosted;
    public void Show(string text, ToastKind kind = ToastKind.Info) =>
        MessagePosted?.Invoke(new ToastMessage(text, kind));
}
