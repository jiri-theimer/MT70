using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BL
{
    public class Factory
    {
        public BO.RunningUser CurrentUser { get; set; }
        public BL.RunningApp App { get; set; }
        
        public BL.TheEntitiesProvider EProvider { get; set; }
        
        public BL.TheTranslator Translator { get; set; }


        
        private ICBL _cbl;
        private IFBL _fbl;
        private Ij72TheGridTemplateBL _j72;
        private IDataGridBL _grid;
        private Ix35GlobalParamBL _gp;        
        private Ij02PersonBL _j02;        
        private Ij03UserBL _j03;
        private Ij04UserRoleBL _j04;
        private Ij05MasterSlaveBL _j05;
        private Ij07PersonPositionBL _j07;
        private Ij11TeamBL _j11;
        private Ij17CountryBL _j17;
        private Ij18RegionBL _j18;
        private Ij61TextTemplateBL _j61;
        private Io15AutoCompleteBL _o15;
        private Io13AttachmentTypeBL _o13;
        private Io27AttachmentBL _o27;
        private Io58FieldBagBL _o58;
        private Ib65WorkflowMessageBL _b65;
        private Ip07ProjectLevelBL _p07;
        private Ip30Contact_PersonBL _p30;
        private Ip31WorksheetBL _p31;
        private Ip32ActivityBL _p32;
        private Ip34ActivityGroupBL _p34;
        private Ip41ProjectBL _p41;
        private Ip42ProjectTypeBL _p42;
        private Ip29ContactTypeBL _p29;
        private Ip35UnitBL _p35;
        private Ip36LockPeriodBL _p36;
        private Ip38ActivityTagBL _p38;
        private Ip51PriceListBL _p51;
        private Ip61ActivityClusterBL _p61;
        private Ip63OverheadBL _p63;
        private Ip80InvoiceAmountStructureBL _p80;
        private Ip85TempboxBL _p85;
        private Ip86BankAccountBL _p86;
        private Ip92InvoiceTypeBL _p92;
        private Ip93InvoiceHeaderBL _p93;
        private Ip95InvoiceRowBL _p95;
        private Ip98Invoice_Round_Setting_TemplateBL _p98;
        private Ix51HelpCoreBL _x51;
        private Ix55WidgetBL _x55;
        private Ix67EntityRoleBL _x67;
        private Ic21FondCalendarBL _c21;
        private Ic26HolidayBL _c26;
        private Ix27EntityFieldGroupBL _x27;
        private Ix28EntityFieldBL _x28;
        private Ix38CodeLogicBL _x38;
        private Ix97TranslateBL _x97;
        private Io40SmtpAccountBL _o40;

        public Factory(BO.RunningUser ru,BL.RunningApp runningapp,BL.TheEntitiesProvider ep,BL.TheTranslator tt)
        {
           
            this.CurrentUser = ru;
            this.App = runningapp;
            this.EProvider = ep;            
            this.Translator = tt;
           

            if (ru.pid == 0 && string.IsNullOrEmpty(ru.j03Login)==false)
            {
                InhaleUserByLogin(ru.j03Login);
                
            }
            
        }

        public void LogInfo(string message)
        {
            BO.BASFILE.LogInfo(message, CurrentUser.j03Login);
           

        }




        public void InhaleUserByLogin(string strLogin)
        {
            DL.DbHandler db = new DL.DbHandler(this.App.ConnectString, this.CurrentUser,this.App.LogFolder);            
            this.CurrentUser = db.Load<BO.RunningUser>("exec dbo._core_j03user_load_sysuser @login", new { login = strLogin });
           


        }
        //logování přihlášení musí být zde, protože se logují i neńsspěšné pokusy o přihlášení a nešlo by to řešit v j03UserBL
        public void Write2AccessLog(BO.j90LoginAccessLog c) //zápis úspěšných přihlášení i neúspěšných pokusů o přihlášení
        {                                  
            DL.DbHandler db = new DL.DbHandler(this.App.ConnectString, this.CurrentUser, this.App.LogFolder);
            string s = "INSERT INTO j90LoginAccessLog(j03ID,j90Date,j90ClientBrowser,j90BrowserFamily,j90Platform,j90BrowserDeviceType,j90BrowserDeviceFamily,j90ScreenPixelsWidth,j90ScreenPixelsHeight,j90BrowserInnerWidth,j90BrowserInnerHeight,j90LoginMessage,j90LoginName,j90CookieExpiresInHours,j90AppClient,j90UserHostAddress)";
            s += " VALUES(@j03id,GETDATE(),@useragent,@browser,@os,@devicetype,@devicefamily,@aw,@ah,@iw,@ih,@mes,@loginname,@cookieexpire,'7.0',@host)";
            db.RunSql(s,new {j03id=BO.BAS.TestIntAsDbKey(c.j03ID), useragent = c.j90ClientBrowser,browser= c.j90BrowserFamily,os=c.j90Platform, devicetype=c.j90BrowserDeviceType, devicefamily=c.j90BrowserDeviceFamily,aw=c.j90ScreenPixelsWidth,ah=c.j90ScreenPixelsHeight,iw=c.j90BrowserInnerWidth,ih=c.j90BrowserInnerHeight,mes=c.j90LoginMessage, loginname=c.j90LoginName, cookieexpire=c.j90CookieExpiresInHours, host=c.j90UserHostAddress });
        }
        
        

        public string tra(string strExpression)   //lokalizace do ostatních jazyků
        {
            if (this.CurrentUser.j03LangIndex == 0) return strExpression;
            return this.Translator.DoTranslate(strExpression, this.CurrentUser.j03LangIndex);
        }

        public string trawi(string strExpression,int langindex)   //lokalizace do ostatních jazyků
        {
            
            return this.Translator.DoTranslate(strExpression, langindex);
        }

        
        public ICBL CBL
        {
            get
            {
                if (_cbl == null) _cbl = new CBL(this);
                return _cbl;
            }
        }
        public IFBL FBL
        {
            get
            {
                if (_fbl == null) _fbl = new FBL(this);
                return _fbl;
            }
        }
        public Ij72TheGridTemplateBL j72TheGridTemplateBL
        {
            get
            {
                if (_j72 == null) _j72 = new j72TheGridTemplateBL(this);
                return _j72;
            }
        }
        public IDataGridBL gridBL
        {
            get
            {
                if (_grid == null) _grid = new DataGridBL(this);
                return _grid;
            }
        }
        public Ix35GlobalParamBL x35GlobalParamBL
        {
            get
            {
                if (_gp == null) _gp = new x35GlobalParamBL(this);
                return _gp;
            }
        }
        
        public Ij02PersonBL j02PersonBL
        {
            get
            {
                if (_j02 == null) _j02 = new j02PersonBL(this);
                return _j02;
            }
        }
        public Ij03UserBL j03UserBL
        {
            get
            {
                if (_j03 == null) _j03 = new j03UserBL(this);
                return _j03;
            }
        }
        public Ij04UserRoleBL j04UserRoleBL
        {
            get
            {
                if (_j04 == null) _j04 = new j04UserRoleBL(this);
                return _j04;
            }
        }
        public Ij05MasterSlaveBL j05MasterSlaveBL
        {
            get
            {
                if (_j05 == null) _j05 = new j05MasterSlaveBL(this);
                return _j05;
            }
        }
        public Ij07PersonPositionBL j07PersonPositionBL
        {
            get
            {
                if (_j07 == null) _j07 = new j07PersonPositionBL(this);
                return _j07;
            }
        }
        public Ij11TeamBL j11TeamBL
        {
            get
            {
                if (_j11 == null) _j11 = new j11TeamBL(this);
                return _j11;
            }
        }
        public Ij17CountryBL j17CountryBL
        {
            get
            {
                if (_j17 == null) _j17 = new j17CountryBL(this);
                return _j17;
            }
        }
        public Ij18RegionBL j18RegionBL
        {
            get
            {
                if (_j18 == null) _j18 = new j18RegionBL(this);
                return _j18;
            }
        }
        public Ij61TextTemplateBL j61TextTemplateBL
        {
            get
            {
                if (_j61 == null) _j61 = new j61TextTemplateBL(this);
                return _j61;
            }
        }
        public Io15AutoCompleteBL o15AutoCompleteBL
        {
            get
            {
                if (_o15 == null) _o15 = new o15AutoCompleteBL(this);
                return _o15;
            }
        }
        public Io58FieldBagBL o58FieldBagBL
        {
            get
            {
                if (_o58 == null) _o58 = new o58FieldBagBL(this);
                return _o58;
            }
        }
        public Ib65WorkflowMessageBL b65WorkflowMessageBL
        {
            get
            {
                if (_b65 == null) _b65 = new b65WorkflowMessageBL(this);
                return _b65;
            }
        }
        public Ic21FondCalendarBL c21FondCalendarBL
        {
            get
            {
                if (_c21 == null) _c21 = new c21FondCalendarBL(this);
                return _c21;
            }
        }
        public Ic26HolidayBL c26HolidayBL
        {
            get
            {
                if (_c26 == null) _c26 = new c26HolidayBL(this);
                return _c26;
            }
        }
        public Io13AttachmentTypeBL o13AttachmentTypeBL
        {
            get
            {
                if (_o13 == null) _o13 = new o13AttachmentTypeBL(this);
                return _o13;
            }
        }

        public Io27AttachmentBL o27AttachmentBL
        {
            get
            {
                if (_o27 == null) _o27 = new o27AttachmentBL(this);
                return _o27;
            }
        }

        public Ip07ProjectLevelBL p07ProjectLevelBL
        {
            get
            {
                if (_p07 == null) _p07 = new p07ProjectLevelBL(this);
                return _p07;
            }
        }
        public Ip30Contact_PersonBL p30Contact_PersonBL
        {
            get
            {
                if (_p30 == null) _p30 = new p30Contact_PersonBL(this);
                return _p30;
            }
        }
        public Ip31WorksheetBL p31WorksheetBL
        {
            get
            {
                if (_p31 == null) _p31 = new p31WorksheetBL(this);
                return _p31;
            }
        }
        public Ip32ActivityBL p32ActivityBL
        {
            get
            {
                if (_p32 == null) _p32 = new p32ActivityBL(this);
                return _p32;
            }
        }
        public Ip34ActivityGroupBL p34ActivityGroupBL
        {
            get
            {
                if (_p34 == null) _p34 = new p34ActivityGroupBL(this);
                return _p34;
            }
        }

        public Ip41ProjectBL p41ProjectBL
        {
            get
            {
                if (_p41 == null) _p41 = new p41ProjectBL(this);
                return _p41;
            }
        }
        public Ip42ProjectTypeBL p42ProjectTypeBL
        {
            get
            {
                if (_p42 == null) _p42 = new p42ProjectTypeBL(this);
                return _p42;
            }
        }
        public Ip29ContactTypeBL p29ContactTypeBL
        {
            get
            {
                if (_p29 == null) _p29 = new p29ContactTypeBL(this);
                return _p29;
            }
        }
        public Ip35UnitBL p35UnitBL
        {
            get
            {
                if (_p35 == null) _p35 = new p35UnitBL(this);
                return _p35;
            }
        }
        public Ip36LockPeriodBL p36LockPeriodBL
        {
            get
            {
                if (_p36 == null) _p36 = new p36LockPeriodBL(this);
                return _p36;
            }
        }
        public Ip38ActivityTagBL p38ActivityTagBL
        {
            get
            {
                if (_p38 == null) _p38 = new p38ActivityTagBL(this);
                return _p38;
            }
        }
        public Ip51PriceListBL p51PriceListBL
        {
            get
            {
                if (_p51 == null) _p51 = new p51PriceListBL(this);
                return _p51;
            }
        }
        public Ip61ActivityClusterBL p61ActivityClusterBL
        {
            get
            {
                if (_p61 == null) _p61 = new p61ActivityClusterBL(this);
                return _p61;
            }
        }
        public Ip63OverheadBL p63OverheadBL
        {
            get
            {
                if (_p63 == null) _p63 = new p63OverheadBL(this);
                return _p63;
            }
        }
        public Ip80InvoiceAmountStructureBL p80InvoiceAmountStructureBL
        {
            get
            {
                if (_p80 == null) _p80 = new p80InvoiceAmountStructureBL(this);
                return _p80;
            }
        }
        public Ip85TempboxBL p85TempboxBL
        {
            get
            {
                if (_p85 == null) _p85 = new p85TempboxBL(this);
                return _p85;
            }
        }
        public Ip86BankAccountBL p86BankAccountBL
        {
            get
            {
                if (_p86 == null) _p86 = new p86BankAccountBL(this);
                return _p86;
            }
        }
        public Ip92InvoiceTypeBL p92InvoiceTypeBL
        {
            get
            {
                if (_p92 == null) _p92 = new p92InvoiceTypeBL(this);
                return _p92;
            }
        }
        public Ip93InvoiceHeaderBL p93InvoiceHeaderBL
        {
            get
            {
                if (_p93 == null) _p93 = new p93InvoiceHeaderBL(this);
                return _p93;
            }
        }
        public Ip95InvoiceRowBL p95InvoiceRowBL
        {
            get
            {
                if (_p95 == null) _p95 = new p95InvoiceRowBL(this);
                return _p95;
            }
        }
        public Ip98Invoice_Round_Setting_TemplateBL p98Invoice_Round_Setting_TemplateBL
        {
            get
            {
                if (_p98 == null) _p98 = new p98Invoice_Round_Setting_TemplateBL(this);
                return _p98;
            }
        }
        public Ix27EntityFieldGroupBL x27EntityFieldGroupBL
        {
            get
            {
                if (_x27 == null) _x27 = new x27EntityFieldGroupBL(this);
                return _x27;
            }
        }
        public Ix28EntityFieldBL x28EntityFieldBL
        {
            get
            {
                if (_x28 == null) _x28 = new x28EntityFieldBL(this);
                return _x28;
            }
        }
        public Ix38CodeLogicBL x38CodeLogicBL
        {
            get
            {
                if (_x38 == null) _x38 = new x38CodeLogicBL(this);
                return _x38;
            }
        }
        public Ix51HelpCoreBL x51HelpCoreBL
        {
            get
            {
                if (_x51 == null) _x51 = new x51HelpCoreBL(this);
                return _x51;
            }
        }
        public Ix55WidgetBL x55WidgetBL
        {
            get
            {
                if (_x55 == null) _x55 = new x55WidgetBL(this);
                return _x55;
            }
        }
        public Ix67EntityRoleBL x67EntityRoleBL
        {
            get
            {
                if (_x67 == null) _x67 = new x67EntityRoleBL(this);
                return _x67;
            }
        }
        public Ix97TranslateBL x97TranslateBL
        {
            get
            {
                if (_x97 == null) _x97 = new x97TranslateBL(this);
                return _x97;
            }
        }

        public Io40SmtpAccountBL o40SmtpAccountBL
        {
            get
            {
                if (_o40 == null) _o40 = new o40SmtpAccountBL(this);
                return _o40;
            }
        }

        
    }
}
