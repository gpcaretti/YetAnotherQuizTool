namespace Quiz.Blazor.Maui.Standalone.Views;
public partial class MainView : ContentPage {

    public MainView() {
        InitializeComponent();

        //using var scope = _serviceProvider.CreateScope();

        //var rootBlazor = (myBlazor.RootComponents.Count > 0) ? myBlazor.RootComponents[0] : null;
        //if (rootBlazor != null) {
        //    rootBlazor.Parameters = new Dictionary<string, object?> {
        //            { "Navigation", "test" }
        //        };
        //}
    }

    public void ShowActivityIndicator(bool visibility) {
        myBlazorView.IsEnabled = !visibility;
        myActivityIndicator.IsRunning = visibility;
    }
}