using aTES.Analytics.DataAccess;
using aTES.Analytics.Services;
using aTES.SchemaRegistry;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace aTES.Analytics
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AnalyticsDbContext>();

            services.AddControllersWithViews();

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
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

            services
                .AddTransient<UsersService>()
                .AddTransient<MessageSerializer>()
                .AddHostedService<AccountsStreamConsumer>()
                .AddHostedService<TasksBillingConsumer>()
                .AddHostedService<AccountBalanceUpdatesConsumer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AnalyticsDbContext dbContext)
        {
            dbContext.Database.Migrate();
            DataBaseIdReady = true;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                .RequireAuthorization();
            });
        }
        public static bool DataBaseIdReady { get; private set; }

    }
}
