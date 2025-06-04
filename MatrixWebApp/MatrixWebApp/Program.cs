using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace MatrixWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var cultureInfo = new CultureInfo("nl-NL");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddHttpClient("MatrixApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7113/");
            });

            var app = builder.Build();

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("nl-NL"),
                SupportedCultures = new[] { cultureInfo },
                SupportedUICultures = new[] { cultureInfo }
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
