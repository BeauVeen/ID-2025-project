using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MatrixWebApp.Services;

namespace MatrixWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Session configuratie voor de shoppingcart 
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var cultureInfo = new CultureInfo("nl-NL");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            builder.Services.AddRazorPages();

            // Authentication setup met Cookie + JWT
            builder.Services.AddAuthentication(options =>
            {
                // Default scheme op Cookie zetten, want webapp gebruikt cookies voor login
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                // Je kan hier nog extra cookie opties zetten, zoals ExpireTimeSpan, SlidingExpiration, etc.
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "MatrixApi",
                    ValidAudience = "MatrixWebApp",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EenLangSterkeSecretKey1234567890!"))
                };
            });

            builder.Services.AddScoped<ShoppingCartService>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHttpClient("MatrixApi", client =>
            {
                client.BaseAddress = new Uri("http://20.86.128.95");
            });
            //https://localhost:7113/

            var app = builder.Build();

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("nl-NL"),
                SupportedCultures = new[] { cultureInfo },
                SupportedUICultures = new[] { cultureInfo }
            });

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapRazorPages();

            app.Run();
        }
    }
}
