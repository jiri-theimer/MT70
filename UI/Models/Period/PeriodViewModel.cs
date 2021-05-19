using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class PeriodViewModel:BaseViewModel
    {
        public string prefix { get; set; }
        
        public string UserParamKey { get; set; } = "grid-period";
        public string PeriodField { get; set; }
        public DateTime? d1 { get; set; }
        public DateTime? d2 { get; set; }
        

        public int PeriodValue { get; set; }
        
        public bool IsShowButtonRefresh { get; set; }

        public string d1_iso
        {
            get
            {
                if (d1 == null) return "2000-01-01";
                return Convert.ToDateTime(d1).ToString("o").Substring(0,10);
            }
        }
        public string d2_iso
        {
            get
            {
                if (d2 == null) return "3000-01-01";
                return Convert.ToDateTime(d2).ToString("o").Substring(0,10);
            }
        }


        public void InhaleUserPeriodSetting(BL.ThePeriodProvider pp, BL.Factory f)
        {
            int x = f.CBL.LoadUserParamInt(this.UserParamKey+"-value");  //podformuláře filtrují období za sebe a nikoliv globálně jako flatview/masterview
            if (x > 0)
            {
                this.PeriodField = f.CBL.LoadUserParam(this.UserParamKey+"-field");
            }
            
            if (x > 60)
            {
                //uživatelem definované období
                var rec = f.FBL.LoadX21(x);
                this.PeriodValue = rec.pid;                
                this.d1 = rec.x21ValidFrom;
                this.d2 = rec.x21ValidUntil;
                return;
            }
            switch (x)
            {
                case 0: //nefiltrovat období
                    this.PeriodValue = 0;
                    break;
                case 1:     //ručně zadaný interval d1-d2
                    var r1 = pp.ByPid(1);
                    this.PeriodValue = r1.pid;                    
                    this.d1 = f.CBL.LoadUserParamDate(this.UserParamKey+"-d1");
                    this.d2 = f.CBL.LoadUserParamDate(this.UserParamKey+"-d2");                    
                    break;
                default:    //pojmenované období
                    var r = pp.ByPid(x);                    
                    this.PeriodValue = r.pid;                    
                    this.d1 = r.d1;
                    this.d2 = r.d2;                    
                    break;
            }
        }

     
    }
}
