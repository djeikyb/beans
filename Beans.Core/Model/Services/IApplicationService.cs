namespace Beans.Model.Services;

public interface IApplicationService
{
    // Task OpenFileAsync(Func<Stream, Task> callback, List<string> fileTypes, string title);
    // Task SaveFileAsync(Func<Stream, Task> callback, List<string> fileTypes, string title, string fileName, string defaultExtension);
    void ToggleTheme();
    void ToggleTopmost();
    // Task SetClipboardTextAsync(string text);
    void Exit();
}
