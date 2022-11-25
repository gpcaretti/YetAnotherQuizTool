using Quiz.Application.UI;

namespace Quiz.Mudblazor.ServerApp.Services;

internal class UIAppService : IUIAppService {
    private int _counter = 0;

    public bool IsActivityIndicatorVisible => _counter > 0;

    public event Action<bool>? OnActivityIndicatorVisibilityChanged;

    public void ShowActivityIndicator() {
        _counter++;
        NotifyActivityIndicatorChanged(true);
    }

    public void HideActivityIndicator() {
        if (_counter > 0) _counter--;
        NotifyActivityIndicatorChanged(false);
    }

    public Task ShowAlert(string message, string cancel) {
        return ShowAlert(null, message, cancel);
    }

    public Task ShowAlert(string? title, string? message, string cancel) {
        throw new NotImplementedException();
        //return CurrentPage.DisplayAlert(title, message, cancel);
    }

    private void NotifyActivityIndicatorChanged(bool visibility) => OnActivityIndicatorVisibilityChanged?.Invoke(visibility);
}