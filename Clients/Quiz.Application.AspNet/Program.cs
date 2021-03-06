using System.Net;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quiz.Application.Web.Authentication;
using Quiz.Domain;

namespace Quiz.Application.Web {

    public class Program {

        private static ILogger<Program> Logger;

        public static void Main(string[] args) {
            // NET6 startup (without Startup.cs)
            var builder = WebApplication.CreateBuilder(args);

            ConfigureLogger(builder, builder.Configuration);
            ConfigureServices(builder.Configuration, builder.Services);

            PreConfigureMiddleware(builder.Configuration, builder.Services, builder.Environment);

            var app = builder.Build();
            ConfigureMiddleware(app, app.Environment);
            ConfigureEndpoints(app);

            Logger = app.Services.GetService<ILogger<Program>>();

            Logger.LogInformation("Web app running...");
            app.Run();
        }

        private static void ConfigureLogger(WebApplicationBuilder builder, ConfigurationManager configuration) {
            builder.Host.ConfigureLogging(logging => {
                //logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Information);
            });
        }

        // Add services to the container
        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services) {
            var connectionString = configuration.GetConnectionString("QuizDBConnection");
            services.AddDbContext<QuizDBContext>(options => options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            if (string.IsNullOrEmpty(connectionString)) {
                //Logger.LogCritical("DB connection string 'QuizDBConnection' not found! Cannot proceed further. Program terminated");
                throw new Exception("DB connection not found");
            } else {
                //Logger.LogInformation("DB connection set");
            }

            // Add services to the container.
            services.AddLogging();
            services.AddMyServices();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<BasicAuthentication>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
#if (DEBUG)
            // useful to allow modification to cshtml pages during development
            services.AddRazorPages().AddRazorRuntimeCompilation();
#else
            services.AddRazorPages();
#endif
            // configure http session and its timeout
            services.AddSession(options => {
                var timeout = configuration.GetSection("Session")?.GetValue<int?>("IdleTimeout") ?? 30;
                options.IdleTimeout = TimeSpan.FromMinutes(timeout);
            });

            // configure form post size
            services.Configure<FormOptions>(options => {
                options.ValueCountLimit = 32 * 1024;
                options.ValueLengthLimit = 16 * 1024 * 1024;
                //options.MultipartBodyLengthLimit = int.MaxValue;
                //options.MultipartHeadersLengthLimit = int.MaxValue;// 64 * 1024;
            });
        }

        private static void PreConfigureMiddleware(IConfiguration configuration, IServiceCollection services, IWebHostEnvironment env) {
            // HSTS and automatic forward to HTTPS (see https://aka.ms/aspnetcore-hsts).
            if (!env.IsDevelopment()) {
                services.AddHsts(options => {
                    var section = configuration.GetSection("Hsts");
                    options.Preload = section?.GetValue<bool?>("Preload") ?? false;
                    options.MaxAge = TimeSpan.FromDays(section?.GetValue<int?>("MaxAge") ?? 60);
                    options.IncludeSubDomains = section?.GetValue<bool?>("IncludeSubDomains") ?? false;
                    //options.ExcludedHosts.Add("example.com");
                    //options.ExcludedHosts.Add("www.example.com");
                });

                services.AddHttpsRedirection(options => {
                    options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
                    options.HttpsPort = 443;
                });
            }
        }

        private static void ConfigureMiddleware(IApplicationBuilder app, IWebHostEnvironment env) {
            // Configure the HTTP request pipeline
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            } else {    // production
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession(
                //new SessionOptions() {
                //    IdleTimeout = TimeSpan.FromMinutes(Environment.con con configuration.GetSection("Session")?.GetValue<int?>("IdleTimeout") ?? 30),
                //}
            );

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private static void ConfigureEndpoints(IEndpointRouteBuilder app) {
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
        }

    }
}
