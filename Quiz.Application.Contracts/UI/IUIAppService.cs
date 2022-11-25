namespace Quiz.Application.UI {

    /// <summary>
    ///     User interface utilities
    /// </summary>
    public interface IUIAppService {

        event Action<bool>? OnActivityIndicatorVisibilityChanged;
        bool IsActivityIndicatorVisible { get; }

        void ShowActivityIndicator();
        void HideActivityIndicator();
        Task ShowAlert(string? message, string cancel = "Cancel");
        Task ShowAlert(string? title, string? message, string cancel);
    }
}
