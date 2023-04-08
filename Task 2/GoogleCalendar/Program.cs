using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GoogleCalendar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Configuration.AddJsonFile($"appsettings.json",optional:false,reloadOnChange:true).
                AddEnvironmentVariables()
                .Build();
            var con = builder.Configuration.GetSection("Authentication:Google-Auth").Value;
            builder.Services
             .AddAuthentication(o =>
             {
                 o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                 o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                 o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
             })
             .AddCookie()
             .AddGoogleOpenIdConnect(options =>
             {
                 options.ClientId = "893787613838-onhkpi5ug6jndp8roneaehuk5dh2h4cu.apps.googleusercontent.com";
                 options.ClientSecret = "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";
                 options.CallbackPath = new PathString("/signin-oauthprovider");
                 options.SignedOutRedirectUri = "http://localhost:44398";
             });
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromDays(1);//You can set Time   
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}