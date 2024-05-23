using Editor.Client.Components;
using Editor.Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Editor.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddScoped<UserManager>();
            builder.Services.AddBlazorBootstrap();

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            builder.Services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<HttpContextAccessor>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped(sp =>
                 new HttpClient
                 {
                     BaseAddress = new Uri(builder.Configuration["Api:Url"]??throw new Exception("Api url not set"))
                 });

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
            builder.Services.AddAuthentication();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode();
            app.Run();
        }
    }
}
