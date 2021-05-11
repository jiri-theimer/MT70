using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class PeriodViewModel:BaseViewModel
    {
        public string prefix { get; set; }
        public string masterentity { get; set; }
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


        public void InhaleUserPeriodSetting(BL.ThePeriodProvider pp, BL.Factory f,string prefix,string masterentity)
        {
            int x = f.CBL.LoadUserParamInt(get_param_key("grid-period-value-" + prefix, masterentity));  //podformuláře filtrují období za sebe a nikoliv globálně jako flatview/masterview
            switch (x)
            {
                case 0: //nefiltrovat období
                    this.PeriodValue = 0;
                    break;
                case 1:     //ručně zadaný interval d1-d2
                    var r1 = pp.ByPid(1);
                    this.PeriodValue = r1.pid;                    
                    this.d1 = f.CBL.LoadUserParamDate(get_param_key("grid-period-d1-" + prefix, masterentity));
                    this.d2 = f.CBL.LoadUserParamDate(get_param_key("grid-period-d2-" + prefix, masterentity));
                    this.PeriodField = f.CBL.LoadUserParam(get_param_key("grid-period-field-" + prefix, masterentity));
                    break;
                default:    //pojmenované období
                    var r = pp.ByPid(x);                    
                    this.PeriodValue = r.pid;                    
                    this.d1 = r.d1;
                    this.d2 = r.d2;
                    this.PeriodField = f.CBL.LoadUserParam(get_param_key("grid-period-field-" + prefix, masterentity));
                    break;
            }
        }

        private string get_param_key(string strKey, string strMasterEntity)
        {
            if (strMasterEntity != null)
            {
                return (strKey += "-" + strMasterEntity);
            }
            else
            {
                return strKey;
            }
        }
    }
}
