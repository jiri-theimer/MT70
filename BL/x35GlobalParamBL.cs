using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ix35GlobalParamBL
    {
        public string LoadParam(string strKey, string strDefault = null);
        public int LoadParamInt(string strKey, int intDefault);
        public DateTime? LoadParamDate(string strKey);
        public string TempFolder();
        public string UploadFolder();
        public string CompanyLogoFile();

        public BO.x35GlobalParam Load(int pid); //pracuje napřímo s databází
        public BO.x35GlobalParam LoadByKey(string x35key, bool ifnullnew);  //pracuje napřímo s databází   
        public int Save(BO.x35GlobalParam rec); //pracuje napřímo s databází
        public int Save(string key, string value);  //pracuje napřímo s databází



    }

    class x35GlobalParamBL: BaseBL, Ix35GlobalParamBL
    {
        private IEnumerable<BO.x35GlobalParam> _lis;

        public x35GlobalParamBL(BL.Factory mother) : base(mother)
        {
            _lis = _db.GetList<BO.x35GlobalParam>("select a.*," + _db.GetSQL1_Ocas("x35") + " from x35GlobalParam a");
        }

        

        public string TempFolder()
        {
            return LoadParam("Upload_Folder") + "\\TEMP";
        }
        public string UploadFolder()
        {
            return LoadParam("Upload_Folder");
        }
        public string CompanyLogoFile()
        {
            string logofilename = "company_logo";
            if (_mother.App.IsCloud)
            {
                logofilename = BO.BAS.ParseDbNameFromCloudLogin(_mother.CurrentUser.j03Login) + "_logo";
            }
            if (test_ifexist_logofile(logofilename + ".png")) return logofilename + ".png";
            if (test_ifexist_logofile(logofilename+".jpg")) return logofilename+".jpg";            
            if (test_ifexist_logofile(logofilename + ".gif")) return logofilename + ".gif";


            return "company_logo_default.png";
        }
        private bool test_ifexist_logofile(string logofilename)
        {
            if (System.IO.File.Exists(_mother.App.WwwRootFolder + "\\Plugins\\" + logofilename))
            {
                return true;
            }
            return false;
        }
        public string LoadParam(string strKey, string strDefault = null)
        {
            
            if (_lis.Where(p => p.x35Key == strKey).Count()>0)
            {
                return _lis.Where(p => p.x35Key == strKey).First().x35Value;
            }
            else
            {
                return strDefault;
            }            
        }
        public int LoadParamInt(string strKey, int intDefault)
        {
            
            string s = LoadParam(strKey);
            if (s == null)
            {
                return intDefault;
            }
            else
            {
                return BO.BAS.InInt(s);
            }
        }
        public DateTime? LoadParamDate(string strKey)
        {
            
            string s = LoadParam(strKey);
            if (String.IsNullOrEmpty(s) == true)
            {
                return null;
            }
            else
            {
                return BO.BAS.String2Date(s);
            }
        }

        //níže jsou metody pracující napřímo s databází:------------------------------------------------------------------------
        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("x35"));
            sb(" FROM x35GlobalParam a");
            sb(strAppend);
            return sbret();
        }
        public BO.x35GlobalParam Load(int pid)  
        {
            return _db.Load<BO.x35GlobalParam>(GetSQL1(" WHERE a.x35ID=@pid"), new { pid = pid });
        }
        public BO.x35GlobalParam LoadByKey(string x35key,bool ifnullnew)
        {
            BO.x35GlobalParam rec = _db.Load<BO.x35GlobalParam>(GetSQL1(" WHERE a.x35Key LIKE @key"), new { key = x35key });
            if (ifnullnew && rec == null)
            {
                rec = new BO.x35GlobalParam() { x35Key = x35key };
            }
            return rec;
        }
        public int Save(BO.x35GlobalParam rec)
        {            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("x35Key", rec.x35Key);
            p.AddString("x35Value", rec.x35Value);
            
            return _db.SaveRecord("x35GlobalParam", p.getDynamicDapperPars(), rec);
        }

        public int Save(string x35key,string value)
        {
            var rec = LoadByKey(x35key, true);
            rec.x35Value = value;
            return Save(rec);
        }

        

    }
}
