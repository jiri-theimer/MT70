using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;
using Microsoft.AspNetCore.DataProtection;

namespace UI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private string _wwwrootfolder;

        public Startup(IWebHostEnvironment env)
        {
            _wwwrootfolder = env.WebRootPath;
            
            Configuration = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", false, true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                 .Build();
        }

       

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(Configuration.GetSection("Authentication")["KeyPath"]))
                .SetApplicationName(Configuration.GetSection("Authentication")["AppName"]);

            services.AddAuthentication("Identity.Application")
                 .AddCookie("Identity.Application", config =>
                 {
                     config.Cookie.HttpOnly = true;
                     config.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                     config.Cookie.SameSite = SameSiteMode.Lax;
                     config.SlidingExpiration = true;
                     config.ExpireTimeSpan = TimeSpan.FromHours(24);
                     config.Cookie.Name = Configuration.GetSection("Authentication")["CookieName"];
                     config.Cookie.Path = "/";
                     config.Cookie.Domain = Configuration.GetSection("Authentication")["Domain"];
                     config.ReturnUrlParameter = "returnurl";
                     config.LoginPath = "/Login/UserLogin";
                 });

            //aby se do html zapisovali origin�l unicode znaky:
            services.Configure<Microsoft.Extensions.WebEncoders.WebEncoderOptions>(options => { options.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(System.Text.Unicode.UnicodeRanges.All); });

            services.AddMvc(options => options.EnableEndpointRouting = false);  //nutn� kv�li podpo�e routingu Areas: Mobile
            services.AddControllers();      //kv�li telerik reporting
            services.AddControllersWithViews();
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;      //kv�li telerik reporting
            });

            services.AddHttpClient();       //kv�li pr�ci s httpclient v r�mci IHttpClientFactory
            //services.AddRazorPages();

            services.AddRazorPages().AddNewtonsoftJson();   //kv�li telerik reporting


            services.AddSingleton<BL.RunningApp>(p => new BL.RunningApp()
            {                              
                WwwRootFolder = _wwwrootfolder               
            });

            

            services.AddSingleton<BL.TheEntitiesProvider>();
            services.AddSingleton<BL.TheTranslator>();
            services.AddSingleton<BL.TheColumnsProvider>();
            services.AddSingleton<BL.ThePeriodProvider>();
          

            //Slu�ba pro TELERIK REPORTING:
            services.TryAddSingleton<IReportServiceConfiguration>(sp =>
            new ReportServiceConfiguration
            {
                ReportingEngineConfiguration = ConfigurationHelper.ResolveConfiguration(sp.GetService<IWebHostEnvironment>()),
                HostAppId = "ReportViewer" + Configuration.GetSection("App")["Name"],
                Storage = new Telerik.Reporting.Cache.File.FileStorage()                
            });


            services.AddScoped<BO.RunningUser, BO.RunningUser>();
            services.AddScoped<BL.Factory, BL.Factory>();

            if (Configuration.GetSection("App").GetValue<Boolean>("RobotOnBehind"))
            {
                services.AddHostedService<UI.TheRobotUI>();   //pohon pro robota na pozad�
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            app.UseDeveloperExceptionPage();    //zjt: v r�mci v�voje

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();    //user-identity funguje pouze s app.UseAuthentication
            app.UseAuthorization();

            app.UseRequestLocalization();

            var strCultureCode = Configuration.GetSection("App")["CultureCode"];
            if (string.IsNullOrEmpty(strCultureCode)) strCultureCode = "cs-CZ";
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(strCultureCode);
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(strCultureCode);




            //app.UseMvc(routes => {

            //    routes.MapRoute(
            //      name: "Mobile",
            //      template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //    routes.MapRoute(
            //       name: "default",
            //       template: "{controller=Dashboard}/{action=Index}/{id?}");
            //}
            //);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); //kv�li teleri reporting

                endpoints.MapAreaControllerRoute(
                  name: "Mobile",
                  areaName: "Mobile",
                  pattern: "Mobile/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Index}/{id?}");


                endpoints.MapRazorPages();

            });


            



            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});




            loggerFactory.AddFile("Logs/info-{Date}.log", LogLevel.Information);
            loggerFactory.AddFile("Logs/debug-{Date}.log", LogLevel.Debug);
            loggerFactory.AddFile("Logs/error-{Date}.log", LogLevel.Error);

            
            
        }
    }
}
