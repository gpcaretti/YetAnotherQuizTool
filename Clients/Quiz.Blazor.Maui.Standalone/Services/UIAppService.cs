using Quiz.Application.UI;
using Quiz.Blazor.Maui.Standalone.Views;
using MauiApplication = Microsoft.Maui.Controls.Application;

namespace Quiz.Blazor.Maui.Standalone.Services;

internal class UIAppService : IUIAppService {

    private MainView? _mainView;
    public MainView CurrentPage {
        get {
            _mainView ??= (MainView)MauiApplication.Current!.MainPage!;
            return _mainView;
        }
    }

    private int _counter = 0;

    public bool IsActivityIndicatorVisible => _counter > 0;

    public event Action<bool>? OnActivityIndicatorVisibilityChanged;

    public void ShowActivityIndicator() {
        _counter++;
        NotifyActivityIndicatorChanged(true);
        CurrentPage.ShowActivityIndicator(true);
        //if (milliseconds > 0) {
        //    Task.Run(async () => {
        //        await Task.Delay(milliseconds);
        //        mainView.HideActivityIndicator();
        //    });
        //}
    }

    public void HideActivityIndicator() {
        if (_counter > 0) _counter--;
        NotifyActivityIndicatorChanged(false);
        CurrentPage.ShowActivityIndicator(false);
    }

    public Task ShowAlert(string message, string cancel) {
        return ShowAlert(null, message, cancel);
    }

    public Task ShowAlert(string? title, string? message, string cancel) {
        return CurrentPage.DisplayAlert(title, message, cancel);
    }

    private void NotifyActivityIndicatorChanged(bool visibility) {
        OnActivityIndicatorVisibilityChanged?.Invoke(visibility);
    }

}