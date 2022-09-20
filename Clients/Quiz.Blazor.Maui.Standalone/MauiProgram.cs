using Blazored.Modal;
using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quiz.Application;
using Quiz.Blazor.Maui.Standalone.Services;
using Quiz.Domain;

namespace Quiz.Blazor.Maui.Standalone {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>();

            ConfigureLogger(builder);
            ConfigureLooknFeel(builder);
            PreConfigureMiddleware(builder);
            ConfigureServices(builder);

            return builder.Build();
        }

        private static void ConfigureLogger(MauiAppBuilder builder) {
            // TODO configure log
        }

        // TODO rename it
        private static void ConfigureLooknFeel(MauiAppBuilder builder) {
            builder
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .UseMauiCommunityToolkit();
        }

        private static void PreConfigureMiddleware(MauiAppBuilder builder) {
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
        }

        private static void ConfigureServices(MauiAppBuilder builder) {
            ConfigureDatabase(builder);

            // add libraries' services
            builder.Services.AddBlazoredModal();

            // register application services
            builder.Services.AddQuizStandardServices();
            builder.Services.AddSingleton<WeatherForecastService>();


            // Check if the db exists, if not copy it from internal resourses, if not exist, create it blank
            static void ConfigureDatabase(MauiAppBuilder builder) {
                // Set path to the SQLite database (it will be created if it does not exist)
                var connectionString = builder.Configuration.GetConnectionString("QuizDBConnection")
                                            // TODO ?? throw new InvalidOperationException("Connection string 'QuizDBConnection' not found.");
                                            ?? "yaqt_20.sqlite";
                connectionString = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        connectionString);

                //builder.Services.AddSingleton<IPath, DbPath>();

                // SQLite to access Identity tables
                // TODO builder.Services.AddDbContext<QuizIdentityDBContext>(options => options.UseSqlite($"Filename={connectionString}"));
                // SQLite to access Application tables
                builder.Services.AddDbContext<QuizDBContext>(options => options.UseSqlite($"Filename={connectionString}"));
                //builder.Services.AddDbContext<QuizDBContext>(options => options.UseSqlite($"Data Source={connectionString}"));
            }
        }

    }
}