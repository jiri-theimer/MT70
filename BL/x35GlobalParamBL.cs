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

    }
}
