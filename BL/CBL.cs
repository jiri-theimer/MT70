using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface ICBL
    {
        public string DeleteRecord(string entity, int pid);
        public string LoadUserParam(string strKey,string strDefault=null);
        public int LoadUserParamInt(string strKey, int intDefault =0);
        public bool LoadUserParamBool(string strKey, bool bolDefault);
        public DateTime? LoadUserParamDate(string strKey);
        public bool SetUserParam(string strKey, string strValue);
        public string EstimateRecordCode(string entity);
        public string GetRecordAlias(string entity, int pid);
        public void ClearUserParamsCache();
        public int SaveRecordCode(string codevalue, string prefix, int pid);
        public string GetCurrentRecordCode(string prefix, int pid);
        public System.Data.DataTable GetList_Last10RecordCode(string prefix);
    }
    class CBL :BaseBL, ICBL
    {

        public CBL(BL.Factory mother):base(mother)
        {
           
        }
      
        private IEnumerable<BO.StringPair> _userparams = null;
        public string DeleteRecord(string entity, int pid)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("pid", pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
            string strSP = entity.Substring(0, 3) + "_delete";
           
                  
            switch (entity)
            {
                case "":                    
                    break;
                
                default:
                    return _db.RunSp(strSP, ref pars);                    
            }
            
            return "";
        }
        public string EstimateRecordCode(string entity)
        {
            BO.GetString c = _db.Load<BO.GetString>("select dbo.getRecordCode(@ent,@j03id) as Value",new { ent = entity,j03id=_mother.CurrentUser.pid });
            return c.Value;
        }
        public string GetCurrentRecordCode(string prefix,int pid)
        {
            string strTab = BO.BASX29.GetEntity(prefix);

            BO.GetString c = _db.Load<BO.GetString>($"select {prefix}Code as Value FROM {strTab} WHERE {prefix}ID=@pid", new { pid = pid });
            return c.Value;
        }
        public int SaveRecordCode(string codevalue,string prefix,int pid)
        {
            if (string.IsNullOrEmpty(codevalue))
            {
                this.AddMessage("Kód záznamu nesmí být prázdný.");return 0;
            }
            string strTab = BO.BASX29.GetEntity(prefix);
            if (_db.RunSql($"UPDATE {strTab} SET {prefix}Code=@code WHERE {prefix}ID=@pid", new {code=codevalue, pid = pid }))
            {
                return pid;
            }
            else
            {
                return 0;
            }
        }
        public System.Data.DataTable GetList_Last10RecordCode(string prefix)
        {
            string strTab = BO.BASX29.GetEntity(prefix);
            string s = null;
            switch (prefix)
            {
                case "p28":
                    s = "SELECT TOP 10 a.p28Code, a.p28Name,b.p29Name,p28UserInsert,p28DateInsert FROM p28Contact a LEFT OUTER JOIN p29ContactType b ON a.p29ID=b.p29ID ORDER BY a.p28ID DESC"; break;
                case "p41":
                    s = "select top 10 p41Code,p41Name,p42Name,p41UserInsert,p41DateInsert FROM p41Project a LEFT OUTER JOIN p42ProjectType b ON a.p42ID=b.p42ID WHERE a.p41IsDraft=0 ORDER BY p41ID DESC"; break;
                case "p91":
                    s = "select top 10 p91Code,p91Client,p92Name,p91UserInsert,p91DateInsert FROM p91Invoice a LEFT OUTER JOIN p92InvoiceType b ON a.p92ID=b.p92ID WHERE a.p91IsDraft=0 ORDER BY p91ID DESC"; break;
                case "p90":
                    s = "select top 10 p90Code,null,p89Name,p90UserInsert,p90DateInsert FROM p90Proforma a LEFT OUTER JOIN p89ProformaType b ON a.p89ID=b.p89ID WHERE a.p90IsDraft=0 ORDER BY p90ID DESC";break;
                case "p82":
                    s = "select top 10 p82Code,NULL,NULL,p82UserInsert,p82DateInsert FROM p82Proforma_Payment ORDER BY p82ID DESC";break;
                case "o23":
                    s = "select top 10 o23Code,o23Name,x18Name,o23UserInsert,o23DateInsert FROM o23Doc a INNER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID INNER JOIN x18EntityCategory x18 ON x23.x23ID=x18.x23ID WHERE a.o23IsDraft=0 ORDER BY a.o23ID DESC";break;
            }

            return _db.GetDataTable(s);

        }
        public string GetRecordAlias(string entity,int pid)
        {
            BO.GetString c;
            string strPrefix = entity.Substring(0, 3);
            switch (strPrefix)
            {
                case "p51":
                    c = _db.Load<BO.GetString>("select p51Code+' - '+p51Name as Value FROM p51Order WHERE p51ID=@pid", new { pid=pid });
                    break;
                case "j02":
                    c = _db.Load<BO.GetString>("select j02LastName+' - '+j02FirstName as Value FROM j02Person WHERE j02ID=@pid", new { pid = pid });
                    break;
                default:
                    c = _db.Load<BO.GetString>(string.Format("select {0}Name+' ['+{0}Code+']' as Value FROM {1} WHERE {0}ID=@pid",strPrefix,_mother.EProvider.ByPrefix(strPrefix).TableName), new { pid = pid });
                    break;
            }
            
            return c.Value;
        }
        public string LoadUserParam(string strKey, string strDefault = null)
        {     
            if (_userparams == null)
            {
                _userparams= _db.GetList<BO.StringPair>("SELECT x36Key as [Key],x36Value as [Value] FROM x36UserParam WHERE j03ID=@j03id", new { j03id = _db.CurrentUser.pid });
            }

            if (_userparams.Any(p => p.Key == strKey))
            {
                return _userparams.Where(p => p.Key == strKey).First().Value;
            }
            else
            {
                return strDefault;
            }            
        }
        public int LoadUserParamInt(string strKey, int intDefault)
        {
            string s = LoadUserParam(strKey);
            if (s==null)
            {
                return intDefault;
            }
            else
            {
                return BO.BAS.InInt(s);
            }
        }
        public bool LoadUserParamBool(string strKey, bool bolDefault)
        {
            string s = LoadUserParam(strKey);
            if (s == null)
            {
                return bolDefault;
            }
            else
            {
                if (s == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public DateTime? LoadUserParamDate(string strKey)
        {
            string s = LoadUserParam(strKey);
            if (String.IsNullOrEmpty(s) == true)
            {
                return null;
            }
            else
            {
                return BO.BAS.String2Date(s);
            }
        }
        public bool SetUserParam(string strKey,string strValue)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("x36key", strKey, System.Data.DbType.String);
            pars.Add("x36value", strValue, System.Data.DbType.String);
            
            if (_db.RunSp("x36userparam_save", ref pars,false) == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }        
       
        public void ClearUserParamsCache()
        {
            _userparams = null;
        }
       
    }
}
