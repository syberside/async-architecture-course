using aTES.Analytics.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
// Add services to the container.
var services = builder.Services;
services.AddControllersWithViews();
services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
   .AddCookie("Cookies")
   .AddOpenIdConnect("oidc", options =>
   {
       options.Authority = "https://localhost:5001";

       options.ClientId = "analytics";
       options.ClientSecret = "secret";
       options.ResponseType = "code";

       options.SaveTokens = true;
       options.Scope.Add("PopugRole");
       options.ClaimActions.Clear();
       options.ClaimActions.MapJsonKey("PopugRole", "PopugRole");
   });
services.AddDbContext<AnalyticsDbContext>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
    dbContext.Database.Migrate();
}

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .RequireAuthorization();
app.Run();

namespace aTES.Analytics
{
    public static class Startup
    {
        public static bool DataBaseIdReady { get; private set; }
    }
}
