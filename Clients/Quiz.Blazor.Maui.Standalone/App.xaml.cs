using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Quiz.Blazor.Maui.Standalone {

    public partial class App : Microsoft.Maui.Controls.Application {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public App(
            IServiceProvider serviceProvider,
            IConfiguration configuration) {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            MainPage = new MainPage();
        }

        protected override Window CreateWindow(IActivationState? activationState) {
            var window = base.CreateWindow(activationState);
            window.Created += async (s, e) => await OnMyCreated(s, e);
            window.Activated += async (s, e) => await OnMyActivated(s, e);
            window.Deactivated += async (s, e) => await OnMyDeactivated(s, e);
            window.Title = $"Yeat Another Quiz Tool - v. {AppInfo.Current.VersionString}";

            return window;
        }

        private async Task OnMyDeactivated(object? sender, EventArgs e) {
           // throw new NotImplementedException();
        }

        private async Task OnMyActivated(object? sender, EventArgs e) {
            //throw new NotImplementedException();
        }

        private async Task OnMyCreated(object? sender, EventArgs evt) {
            // Set path to the SQLite database (it will be created if it does not exist)
            var connectionString = _configuration.GetConnectionString("QuizDBConnection")
                                        // TODO ?? throw new InvalidOperationException("Connection string 'QuizDBConnection' not found.");
                                        ?? "yaqt_20.sqlite";
            connectionString = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    connectionString);

#if DEBUG
            // this is for pure debug: reset the database for each running in DEBUG mode
            File.Delete(connectionString!);
#endif
            if (!File.Exists(connectionString) || (new FileInfo(connectionString).Length < (10 * 1024))) {
                // copy the db from resources
                try {
                    var dir = System.IO.Path.GetDirectoryName(connectionString);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);

                    using var inStream = await FileSystem.OpenAppPackageFileAsync("yaqt_20.sqlite");
                    using var outStream = File.Create(connectionString!);
                    await inStream.CopyToAsync(outStream);

                    outStream.Close();
                    inStream.Close();
                }
                catch (Exception ex) {
                    throw;
                }
            }

            //using (var scope = _serviceProvider.CreateScope()) {
            //	await using var appContext = scope.ServiceProvider.GetRequiredService<QuizDBContext>();
            //	// applies all pending migrations to the db and if it doesn’t exist create it
            //	await appContext.Database.EnsureCreatedAsync();
            //	// seed data
            //	await SeedData(appContext);
            //}
        }
    }
}