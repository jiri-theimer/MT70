using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace BL
{
    public class RunningApp
    {
        public RunningApp()
        {
            _AppRootFolder = System.IO.Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder().AddJsonFile(_AppRootFolder + "\\appsettings.json", true).Build();

            this.Configuration = config;

            _ConnectString = config.GetSection("ConnectionStrings")["AppConnection"];
            _AppName = Configuration.GetSection("App")["Name"];
            _TranslatorMode = Configuration.GetSection("App")["TranslatorMode"];
            _LogFolder = Configuration.GetSection("Folders")["Log"];
            if (string.IsNullOrEmpty(_LogFolder))
            {
                _LogFolder = System.IO.Directory.GetCurrentDirectory() + "\\Logs";
            }
        }
        public IConfiguration Configuration { get; private set; }

        private string _ConnectString { get; set; }
        private string _AppName { get; set; }
        private string _AppRootFolder { get; set; }
        private string _TranslatorMode { get; set; }
        private string _LogFolder { get; set; }

        public string ConnectString {
            get
            {
                return _ConnectString;
            }
        }
        public string LogFolder { get {
                return _LogFolder;
            }
        }
        public bool RobotOnBehind { get {
                return Configuration.GetSection("App").GetValue<Boolean>("RobotOnBehind");
            } 
        }
        public string RobotHostUrl { get {
                return Configuration.GetSection("App")["RobotHostUrl"];
            }
        }
        public string AppRootFolder { get
            {
                return _AppRootFolder;
            }
        }
        public string WwwRootFolder { get; set; }       //předává z venku Startup přes IWebHostEnvironment
        public string AppName { get
            {
                return _AppName;
            }
        }
        public string AppVersion {
            get
            {
                return Configuration.GetSection("App")["Version"];
            }
        }
        public string AppBuild
        {
            get
            {
                var execAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                var versionTime = new System.IO.FileInfo(execAssembly.Location).LastWriteTime;

                return "build: " + BO.BAS.ObjectDateTime2String(versionTime);
            }
        }
        public string TranslatorMode {
            get
            {
                return _TranslatorMode;
            }
        }
        public int DefaultLangIndex { get {
                return BO.BAS.InInt(Configuration.GetSection("App")["DefaultLangIndex"]);
            }
        }
        public string Implementation { get {
                return Configuration.GetSection("App")["Implementation"];
            }
        }
        public string CssCustomSkin { get {
                return Configuration.GetSection("App")["CssCustomSkin"];
            }
        }
        

        public bool IsCloud
        {
            get
            {
                if (this.Implementation == "Cloud")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
}
