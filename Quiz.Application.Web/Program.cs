using System.Net;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quiz.Application.Web.Authentication;
using Quiz.Domain;

namespace Quiz.Application.Web {

    public class Program {
        //private static Logger<Program> Logger { get; set; }

        public static void Main(string[] args) {
            // NET6 startup (without Startup.cs)
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Configuration, builder.Services);

            PreConfigureMiddleware(builder.Configuration, builder.Services, builder.Environment);

            var app = builder.Build();
            ConfigureMiddleware(app, app.Environment);
            ConfigureEndpoints(app);

            app.Run();
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
            services.AddMyServices();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<BasicAuthentication>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
#if (DEBUG)
            services.AddRazorPages().AddRazorRuntimeCompilation();
#else
            services.AddRazorPages();
#endif
            // configure session and its timeout
            services.AddSession(options => {
                var section = configuration.GetSection("Session");
                options.IdleTimeout = TimeSpan.FromMinutes(section?.GetValue<int?>("IdleTimeout") ?? 30);
            });
        }

        private static void PreConfigureMiddleware(IConfiguration configuration, IServiceCollection services, IWebHostEnvironment env) {
            // see https://aka.ms/aspnetcore-hsts.
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
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            } else {    // production
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();
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
