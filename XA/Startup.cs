using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace XA
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
            services.AddHttpClient();       //kvùli httpclient
            services.AddControllersWithViews();

           
            services.AddSingleton<BL.RunningApp>(new BL.RunningApp());

            services.AddSingleton<BL.TheEntitiesProvider>();
            services.AddSingleton<BL.TheTranslator>();
            services.AddSingleton<BL.TheColumnsProvider>();
            
            services.AddScoped<BO.RunningUser, BO.RunningUser>();
            services.AddScoped<BL.Factory, BL.Factory>();

            services.AddHostedService<XA.TheRobot>();   //pohon pro robota na pozadí

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();

           
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();


            var strCultureCode = Configuration.GetSection("App")["CultureCode"];
            if (string.IsNullOrEmpty(strCultureCode)) strCultureCode = "cs-CZ";
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(strCultureCode);
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(strCultureCode);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });



            loggerFactory.AddFile("Logs/info-{Date}.log", LogLevel.Information);
            loggerFactory.AddFile("Logs/debug-{Date}.log", LogLevel.Debug);
            loggerFactory.AddFile("Logs/error-{Date}.log", LogLevel.Error);


        }
    }
}
