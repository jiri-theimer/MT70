using System;
using System.Text;

namespace BO
{

    public enum DeviceTypeFlag
    {
        Desktop = 1,
        Phone = 2
    }


    public class j03User : BaseBO
    {
        public string j03PasswordHash { get; set; } //nový soupec verze 7
        public bool j03IsDebugLog { get; set; }     //nový sloupec verze 7
        public string j03HomePageUrl { get; set; }  //nový sloupec verze 7
        public string j03SiteMenuMyLinksV7 { get; set; }    //nový sloupec verze 7
        public string j03PageSplitterFlagV7 { get; set; }   //nový sloupec verze 7
        public int j03LangIndex { get; set; }
        public string j03Login { get; set; }
        public int j04ID { get; set; }
        public int j02ID { get; set; }
        public int j07ID { get; }
        
        
        public bool j03IsSystemAccount { get; set; }
       
        public DateTime? j03LiveChatTimestamp { get; set; }

        

        public bool j03IsShallReadUpgradeInfo { get; set; }
        public bool j03IsMustChangePassword { get; set; }
        public DateTime? j03PasswordExpiration { get; set; }

        public DateTime? j03Ping_TimeStamp { get; set; }
        

        public DeviceTypeFlag j03Ping_DeviceTypeFlag { get; set; } = DeviceTypeFlag.Desktop;   // nový sloupec ve verzi 6
        public int j03Ping_InnerWidth { get; set; }   // nový sloupec ve verzi 6
        public int j03Ping_InnerHeight { get; set; }  // nový sloupec ve verzi 6


        
        
        public string j04Name { get; }
        protected string _j02LastName { get; set; }
        protected string _j02FirstName { get; set; }
        protected string _j02TitleBeforeName { get; set; }
        public string j02Email { get; }
        protected int _j02WorksheetAccessFlag { get; set; }


        public string j03DefaultHoursFormat { get; set; }     // nový sloupec ve verzi 6        možné hodnoty: N a T
        
        public int j03GridSelectionModeFlag { get; set; } = 0; // používá se pouze pro podporu clipboard v gridu
        public int j03GlobalCssFlag { get; set; }     // zvolená velikost písma
        public int j03FreelanceFlag { get; set; } = 0; // 1 - freelance uživatel a plátce DPH 2 - freelance neplátce DPH
        public string j03Cache_HomeMenu { get; set; }
        public int j03ModalWindowsFlag { get; set; }  // 99: zobrazovat modální okna vždy maximalizovaná
                                                // 'Public Property j03MobileForwardFlag As Integer - nevyužívané pole z verze 5


        public string PersonAsc
        {
            get
            {
                return (_j02TitleBeforeName + " " + _j02FirstName + " " + _j02LastName).Trim();
                
            }
        }
        public string PersonDesc
        {
            get
            {
                return (_j02LastName + " " + _j02FirstName + " " + _j02TitleBeforeName).Trim();
            }
        }
       
        public int j02WorksheetAccessFlag
        {
            get
            {
                return _j02WorksheetAccessFlag;
            }
        }
    }


}
