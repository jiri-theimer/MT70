using System;
using System.Collections.Generic;


namespace BO
{
    public class RunningUser:j03User
    {
        public bool j04IsMenu_Worksheet { get; set; }
        public bool j04IsMenu_Project { get; set; }
        public bool j04IsMenu_Contact { get; set; }
        public bool j04IsMenu_People { get; set; }
        public bool j04IsMenu_Report { get; set; }
        public bool j04IsMenu_Invoice { get; set; }
        public bool j04IsMenu_Proforma { get; set; }
        public bool j04IsMenu_Notepad { get; set; }
        public bool j04IsMenu_Task { get; set; }
        public bool j04IsMenu_MyProfile { get; set; }

        
        public bool IsMasterPerson { get; }         //detekce, zda je nadřízený jiným osobám
        public bool IsApprovingPerson { get; }    // detekce, zda uživatel může potenciálně schvalovat nějaký worksheet
        private string RoleValue { get; }
        private string PersonalPage { get; }
        public string p07NamesInline { get; }
        public string p07PluralsInline { get; }
        
        
        public string AppName { get; }

        private int MessagesCount { get; }  // počet zpráv, na které systém upozorňuje uživatele
        public string j11IDs { get; }          // seznam týmů osoby

        
        public string ExplicitConnectString { get; set; }   // pro předávání jiného db connect stringu - pro FILIP
        public string ExplicitLogsDir { get; set; }   // pro předávání jiného logovacího adresáře - pro FILIP

        
        
        public bool TestPermission(BO.x53PermValEnum oneperm)
        {
            if (this.RoleValue.Substring((int)oneperm - 1, 1) == "1")
                return true;
            else
                return false;
        }
        public bool TestPermission(BO.x53PermValEnum oneperm, BO.x53PermValEnum orperm)
        {
            if (this.RoleValue.Substring((int)oneperm - 1, 1) == "1")
            {
                return true;
            }
            else
            {
                if (this.RoleValue.Substring((int)orperm - 1, 1) == "1")
                {
                    return true;
                }
            }

            return false;
                
        }

        private bool? _IsAdmin;
        public bool IsAdmin
        {
            get
            {
                if (_IsAdmin != null)
                {
                    return Convert.ToBoolean(_IsAdmin);
                }
                if (this.RoleValue.Substring((int)BO.x53PermValEnum.GR_Admin - 1, 1) == "1")
                    _IsAdmin = true;
                else
                    _IsAdmin = false;


                return Convert.ToBoolean(_IsAdmin);
            }
        }

        private bool? _IsRatesAccess;
        public bool IsRatesAccess
        {
            get
            {
                if (_IsRatesAccess != null)
                {
                    return Convert.ToBoolean(_IsRatesAccess);
                }
                if (this.RoleValue.Substring((int)BO.x53PermValEnum.GR_P31_AllowRates - 1, 1) == "1")
                    _IsRatesAccess = true;
                else
                    _IsRatesAccess = false;


                return Convert.ToBoolean(_IsRatesAccess);
            }
        }

        

        private string[] _p07NamesArr { get; set; }
        private string[] _p07PluralsArr { get; set; }
        private int _p07LevelsCount { get; set; }
        public string getP07Level(int levelindex,bool singular)
        {
            Handle_ParseP07Levels();
            if (singular)
            {
                return _p07NamesArr[levelindex - 1];

            }
            else
            {
                return _p07PluralsArr[levelindex - 1];
            }
            
        }
        
        public int p07LevelsCount
        {
            get
            {
                Handle_ParseP07Levels();
                return _p07LevelsCount;
            }
        }

        private void Handle_ParseP07Levels()
        {
            if (_p07NamesArr == null)
            {
                _p07NamesArr = this.p07NamesInline.Split("|");
                _p07PluralsArr = this.p07PluralsInline.Split("|");
                _p07LevelsCount = 0;
                for (int x = 0; x <= 4; x++)
                {
                    if (!string.IsNullOrEmpty(_p07NamesArr[x])){
                        _p07LevelsCount += 1;
                    }
                    else
                    {
                        _p07NamesArr[x] = null;
                        _p07PluralsArr[x] = null;
                    }
                }
            }
        }
        
        

        public string SiteMenuPersonalName
        {
            get
            {
                if (_j02FirstName.Length + _j02LastName.Length > 15)
                    return _j02FirstName.ToUpper();
                else
                    return _j02FirstName.ToUpper() + "_" + _j02LastName.ToUpper();
            }
        }

        public string LangName
        {
            get
            {
                switch (this.j03LangIndex)
                {
                    case 1:
                        return "EN";
                    case 2:
                        return "DE";
                    case 4:
                        return "SK";
                    default:
                        return "CZ";
                }
            }
        }

        public string getFontSizeCss()
        {
            
            switch (this.j03GlobalCssFlag)
            {
                case 1:
                    return "fontsize1.css";     //menší písmo               
                case 2:
                    return "fontsize2.css";       //výchozí písmo                
                case 3:
                    return "fontsize3.css";        //větší písmo                                                        
                default:
                    return "fontsize2.css";
            }
            //switch (this.j03GlobalCssFlag)
            //{
            //    case 1:
            //        return "font: 0.6875rem/1.0 var(--font-family-sans-serif)";
            //    case 2:
            //        return "font: 0.75rem/1.0 var(--font-family-sans-serif)";
            //    case 3:
            //        return "font: 0.85rem/1.0 var(--font-family-sans-serif)";
            //    case 4:
            //        return "font: 1rem/1.2 var(--font-family-sans-serif);";
            //    default:
            //        return "font: 0.75rem/1.0 var(--font-family-sans-serif)";
            //}
        }



        public List<BO.StringPair> Messages4Notify { get; set; }
        public void AddMessage(string strMessage, string strTemplate = "error")
        {
            if (Messages4Notify == null) { Messages4Notify = new List<BO.StringPair>(); };
            strMessage = strMessage.Replace("\"", "").Replace("\r\n","<hr>");
            Messages4Notify.Add(new BO.StringPair() { Key = strTemplate, Value = strMessage }); ;
        }


    }
}
