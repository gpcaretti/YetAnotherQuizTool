using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Quiz.Application;
using Quiz.Domain;
using Quiz.Domain.Identity;

namespace Quiz.Blazor.Host.Server {

    public class Program {
        public static void Main(string[] args) {
            try {
                var builder = WebApplication.CreateBuilder(args);
                ConfigureLogger(builder);
                ConfigureServices(builder);
                PreConfigureMiddleware(builder);

                var app = builder.Build();
                ConfigureMiddleware(app);
                ConfigureEndpoints(app);

                app.Run();
            } finally {
                // Log.CloseAndFlush();
            }
        }

        private static void ConfigureLogger(WebApplicationBuilder builder) {
            // TODO: set logger and set its cfg via appsetting.json
            //builder.Host
            //    .UseSerilog((context, services, options) => {
            //        // get the cfg from appsetting.json
            //        options.ReadFrom.Configuration(context.Configuration)
            //        .ReadFrom.Services(services)
            //        .Enrich.FromLogContext()
            //        //.CreateLogger()
            //        ;
            //    });
        }

        // Add services to the container.
        private static void ConfigureServices(WebApplicationBuilder builder) {
            var connectionString = builder.Configuration.GetConnectionString("QuizDBConnection")
                                        ?? throw new InvalidOperationException("Connection string 'QuizDBConnection' not found.");

            // To access Identity tables
            builder.Services.AddDbContext<QuizIdentityDBContext>(options => options.UseSqlServer(connectionString));
            // To access Application tables
            builder.Services.AddDbContext<QuizDBContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            /* Gp -  Original Identity source code 
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
            */

            // Gp - new identity code - START
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
                //options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddRoles<ApplicationRole>() // Add roles.
                .AddEntityFrameworkStores<QuizIdentityDBContext>();

            // From: https://github.com/dotnet/AspNetCore.Docs/issues/17649
            // Configure identity server to put the role claim into the id token and the access
            // token and prevent the default mapping for roles in the JwtSecurityTokenHandler.
            builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, QuizIdentityDBContext>(options => {
                    options.IdentityResources["openid"].UserClaims.Add("role");
                    options.ApiResources.Single().UserClaims.Add("role");
                });
            // Need to do this as it maps "role" to ClaimTypes.Role and causes issues
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler
                .DefaultInboundClaimTypeMap.Remove("role");
            // Gp - new identity code - END

            builder.Services.AddAuthentication()
                .AddIdentityServerJwt();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddMyServices();

            // TODO: configure form post size
            //builder.Services.Configure<FormOptions>(options => {
            //    options.ValueCountLimit = 32 * 1024;
            //    options.ValueLengthLimit = 16 * 1024 * 1024;
            //    //options.MultipartBodyLengthLimit = int.MaxValue;
            //    //options.MultipartHeadersLengthLimit = int.MaxValue;// 64 * 1024;
            //});
        }

        private static void PreConfigureMiddleware(WebApplicationBuilder builder) {
            // HSTS and automatic forward to HTTPS (see https://aka.ms/aspnetcore-hsts).
            if (!builder.Environment.IsDevelopment()) {
                builder.Services.AddHsts(options => {
                    var section = builder.Configuration.GetSection("Hsts");
                    options.Preload = section?.GetValue<bool?>("Preload") ?? false;
                    options.MaxAge = TimeSpan.FromDays(section?.GetValue<int?>("MaxAge") ?? 60);
                    options.IncludeSubDomains = section?.GetValue<bool?>("IncludeSubDomains") ?? false;
                    //options.ExcludedHosts.Add("example.com");
                    //options.ExcludedHosts.Add("www.example.com");
                });

                builder.Services.AddHttpsRedirection(options => {
                    options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
                    options.HttpsPort = 443;
                });
            }
        }

        private static void ConfigureMiddleware(WebApplication app) {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            // We usually add these after the 'UseRouting', so that the auth/auth middleware knows about the URL being accessed by the User.
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private static void ConfigureEndpoints(WebApplication app) {
            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");
        }

    }
}