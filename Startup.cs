using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using react_crash_2021.Data;
using react_crash_2021.Data.Entities;
using react_crash_2021.Data.RepositoryFiles;
using System.Reflection;

namespace react_crash_2021
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

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            //add auto mapper
            //it needs a profile
            //says go look for profile classes on startup that derive from profile
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddDbContext<ReactCrashAppContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AppContext")));

            //identity docs: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-5.0
            //Identity with the default UI
            services.AddDefaultIdentity<reactCrashUser>()
                .AddEntityFrameworkStores<ReactCrashAppContext>();
            //IdentityServer with an additional AddApiAuthorization helper method that sets up some 
            //default ASP.NET Core conventions on top of IdentityServer:
            services.AddIdentityServer()
                .AddApiAuthorization<reactCrashUser, ReactCrashAppContext>();
            //Authentication with an additional AddIdentityServerJwt helper method 
            //that configures the app to 
            //validate JWT tokens produced by IdentityServer:
            services.AddAuthentication()
                .AddIdentityServerJwt();
            //scoped makes it available for the whole http request
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddTransient<AspNetUserManager<reactCrashUser>>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ReactCrashAppContext appContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            appContext.Database.EnsureCreated(); 
            //The authentication middleware that is responsible for 
            //validating the request credentials and setting the user
            //on the request context:
            app.UseAuthentication();
            //The IdentityServer middleware that exposes the OpenID 
            //Connect endpoints:
            app.UseIdentityServer();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
