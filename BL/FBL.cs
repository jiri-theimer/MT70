using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BL
{
    public interface IFBL
    {       
        public IEnumerable<BO.SysDbObject> GetList_SysObjects();
        public void GenerateCreateUpdateScript(IEnumerable<BO.SysDbObject> lis);
        public IEnumerable<BO.x53Permission> GetListX53();
        public IEnumerable<BO.p87BillingLanguage> GetListP87();
        public BO.j27Currency LoadCurrencyByCode(string j27code);
        public BO.j27Currency LoadCurrencyByID(int j27id);
        public IEnumerable<BO.j27Currency> GetListCurrency();
        public int SaveP87(BO.p87BillingLanguage rec);

    }
    class FBL : BaseBL, IFBL
    {
        public FBL(BL.Factory mother) : base(mother)
        {

        }

        public IEnumerable<BO.x53Permission> GetListX53()
        {
            return _db.GetList<BO.x53Permission>("SELECT * FROM x53Permission WHERE GETDATE() between x53ValidFrom AND x53ValidUntil ORDER BY x53Ordinary");
        }
        public IEnumerable<BO.p87BillingLanguage> GetListP87()
        {
            return _db.GetList<BO.p87BillingLanguage>("SELECT "+ _db.GetSQL1_Ocas("p87")+ ",a.* FROM p87BillingLanguage a");
        }
        public int SaveP87(BO.p87BillingLanguage rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("p87Name", rec.p87Name);
            p.AddString("p87Icon", rec.p87Icon);

            return _db.SaveRecord("p87BillingLanguage", p.getDynamicDapperPars(),rec);
        }
        public BO.j27Currency LoadCurrencyByCode(string j27code)
        {
            return _db.Load<BO.j27Currency>("select *,j27ID as pid FROM j27Currency WHERE j27Code LIKE @j27code", new { j27code = j27code });
        }
        public BO.j27Currency LoadCurrencyByID(int j27id)
        {
            return _db.Load<BO.j27Currency>("select * FROM j27Currency WHERE j27ID = @j27id", new { j27id = j27id });
        }
        public IEnumerable<BO.j27Currency> GetListCurrency()
        {
            return _db.GetList<BO.j27Currency>("SELECT * FROM j27Currency a WHERE GETDATE() BETWEEN j27ValidFrom AND j27ValidUntil");
        }

        public IEnumerable<BO.SysDbObject> GetList_SysObjects()
        {
            string s = "SELECT ID,name,xtype,schema_ver as version,convert(text,null) as content FROM sysobjects WHERE rtrim(xtype) IN ('V','FN','P','TR','IF') AND name not like 'dt_%' and name not like 'zzz%' and (name not like 'sys%' or name not like 'system_%') order by xtype,name";
            var lis = _db.GetList<BO.SysDbObject>(s);
            foreach (var c in lis)
            {
                string strContent = "";
                var dt = _db.GetDataTable("select colid,text FROM syscomments where id=" + c.ID.ToString() + " order by colid");
                foreach (DataRow dbrow in dt.Rows)
                {
                    strContent += dbrow["text"];
                }
                c.Content = strContent;
                c.xType = c.xType.Trim();
            }
            return lis;
        }

        public void GenerateCreateUpdateScript(IEnumerable<BO.SysDbObject> lis)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var c in lis)
            {
                sb.AppendLine("if exists(select 1 from sysobjects where id = object_id('" + c.Name + "') and type = '" + c.xType + "')");
                switch (c.xType)
                {
                    case "P":
                        sb.AppendLine(" drop procedure " + c.Name);
                        break;
                    case "FN":
                    case "IF":
                        sb.AppendLine(" drop function " + c.Name);
                        break;
                    case "V":
                        sb.AppendLine(" drop view " + c.Name);
                        break;
                }
                sb.AppendLine("GO");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine(c.Content);
                sb.AppendLine("");
                sb.AppendLine("GO");

                System.IO.File.WriteAllText(_mother.x35GlobalParamBL.TempFolder() + "\\sql_sp_funct_views.sql", sb.ToString());
            }
        }

       

    }
}
