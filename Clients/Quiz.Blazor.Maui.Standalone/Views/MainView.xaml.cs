using Quiz.Application.UI;

namespace Quiz.Blazor.Maui.Standalone.Views;
public partial class MainView : ContentPage {

    private readonly IUIAppService _uiAppService;

    public MainView(IUIAppService uiAppService) {
        _uiAppService = uiAppService;
        InitializeComponent();
        Loaded += ContentPage_Loaded;

        //using var scope = _serviceProvider.CreateScope();

        //var rootBlazor = (myBlazor.RootComponents.Count > 0) ? myBlazor.RootComponents[0] : null;
        //if (rootBlazor != null) {
        //    rootBlazor.Parameters = new Dictionary<string, object?> {
        //            { "Navigation", "test" }
        //        };
        //}
    }

    private void ContentPage_Loaded(object sender, EventArgs e) {
        myBackgroundImage.Source = _uiAppService.IsMobileDevice() ? "index_bg_dark_xs.jpg" : "index_bg.jpg";

#if WINDOWS && RELEASE
        var webView2 = (blazorWebView.Handler.PlatformView as Microsoft.UI.Xaml.Controls.WebView2);
        await webView2.EnsureCoreWebView2Async();

        var settings = webView2.CoreWebView2.Settings;
        settings.IsZoomControlEnabled = false;
        settings.AreBrowserAcceleratorKeysEnabled = false;
#endif
    }

    public void ShowActivityIndicator(bool visibility) {
        //myBlazorView.IsEnabled = !visibility;
        myActivityIndicator.IsRunning = visibility;
    }
}