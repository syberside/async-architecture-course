using aTES.Billing.Controllers;
using aTES.Billing.DataAccess;
using aTES.Billing.Services;
using aTES.SchemaRegistry;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace aTES.Billing
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
            services.AddDbContext<BillingDbContext>();

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

                   options.ClientId = "billing";
                   options.ClientSecret = "secret";
                   options.ResponseType = "code";

                   options.SaveTokens = true;
                   options.Scope.Add("PopugRole");
                   options.ClaimActions.Clear();
                   options.ClaimActions.MapJsonKey("PopugRole", "PopugRole");
               });

            services
                .AddTransient<UserService>()
                .AddTransient<AccountingService>()
                .AddTransient<MessageBus>()
                .AddTransient<TasksService>()
                .AddTransient<MessageSerializer>()
                .AddSingleton(sp => new Random(DateTime.Now.Millisecond))
                .AddHostedService<BillingCycleConsumer>()
                .AddHostedService<TasksWorkflowConsumer>()
                .AddHostedService<AccountsStreamConsumer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BillingDbContext dbContext)
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
