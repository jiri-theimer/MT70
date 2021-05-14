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
        private string _RoleValue { get; set; }
        private string _PersonalPage { get; set; }
        

        public string AppName { get; }

        private int _MessagesCount { get; set; }  // počet zpráv, na které systém upozorňuje uživatele
        public string j11IDs { get; }          // seznam týmů osoby

        
        public string ExplicitConnectString { get; set; }   // pro předávání jiného db connect stringu - pro FILIP
        public string ExplicitLogsDir { get; set; }   // pro předávání jiného logovacího adresáře - pro FILIP

        
        
        

        
        
        public string PersonalPage
        {
            get
            {
                return _PersonalPage;
            }
        }


        public string RoleValue
        {
            get
            {
                return _RoleValue;
            }
        }

        public bool TestPermission(BO.x53PermValEnum oneperm)
        {
            if (_RoleValue.Substring((int)oneperm - 1, 1) == "1")
                return true;
            else
                return false;
        }
        public bool TestPermission(BO.x53PermValEnum oneperm, BO.x53PermValEnum orperm)
        {
            if (_RoleValue.Substring((int)oneperm - 1, 1) == "1")
            {
                return true;
            }
            else
            {
                if (_RoleValue.Substring((int)orperm - 1, 1) == "1")
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
                if (_RoleValue.Substring((int)BO.x53PermValEnum.GR_Admin - 1, 1) == "1")
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
                if (_RoleValue.Substring((int)BO.x53PermValEnum.GR_P31_AllowRates - 1, 1) == "1")
                    _IsRatesAccess = true;
                else
                    _IsRatesAccess = false;


                return Convert.ToBoolean(_IsRatesAccess);
            }
        }

        public int MessagesCount
        {
            get
            {
                return _MessagesCount;
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
            Messages4Notify.Add(new BO.StringPair() { Key = strTemplate, Value = strMessage }); ;
        }


    }
}
