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
        private Ij07PersonPositionBL _j07;
        private Ij11TeamBL _j11;
        private Io15AutoCompleteBL _o15;
        private Io13AttachmentTypeBL _o13;
        private Io27AttachmentBL _o27;
        private Io58FieldBagBL _o58;
        private Ib65WorkflowMessageBL _b65;
        private Ip07ProjectLevelBL _p07;
        private Ip31WorksheetBL _p31;
        private Ip32ActivityBL _p32;
        private Ip34ActivityGroupBL _p34;
        private Ip41ProjectBL _p41;
        private Ip42ProjectTypeBL _p42;
        private Ip29ContactTypeBL _p29;
        private Ip85TempboxBL _p85;
        private Ix55WidgetBL _x55;
        private Ix67EntityRoleBL _x67;
        private Ic21FondCalendarBL _c21;
        private Ic26HolidayBL _c26;
        private Ix27EntityFieldGroupBL _x27;
        private Ix28EntityFieldBL _x28;
        private Ix38CodeLogicBL _x38;

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

        public Ip85TempboxBL p85TempboxBL
        {
            get
            {
                if (_p85 == null) _p85 = new p85TempboxBL(this);
                return _p85;
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
    }
}
