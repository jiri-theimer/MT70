using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BL
{
    public class TheReportOnFly
    {
        public IConfiguration Configuration { get; private set; }
        private BL.RunningApp _app;

        public TheReportOnFly(BL.RunningApp app)
        {
            _app = app;

            var config = new ConfigurationBuilder().AddJsonFile(_app.AppRootFolder + "\\appsettings.json", true).Build();

            this.Configuration = config;
        }
    }
}
