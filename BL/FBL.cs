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

        public int AppendRobotLog(BO.j91RobotLog rec);  //uložení jetí robota na pozadí
        public BO.j91RobotLog GetLastRobotRun(BO.j91RobotTaskFlag flag); //vrátí poslední jetí pro zadaný flag
        public IEnumerable<BO.j91RobotLog> GetListRobotLast20();
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


        public int AppendRobotLog(BO.j91RobotLog rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddDateTime("j91Date", DateTime.Now);
            p.AddString("j91BatchGuid", rec.j91BatchGuid);
            p.AddEnumInt("j91TaskFlag", rec.j91TaskFlag);
            p.AddString("j91InfoMessage", rec.j91InfoMessage);
            p.AddString("j91ErrorMessage", rec.j91ErrorMessage);
            p.AddString("j91Account", rec.j91Account);

            return _db.SaveRecord("j91RobotLog", p.getDynamicDapperPars(), rec,false,false);
        }

        public BO.j91RobotLog GetLastRobotRun(BO.j91RobotTaskFlag flag)
        {
            return _db.Load<BO.j91RobotLog>("select TOP 1 * FROM j91RobotLog WHERE j91TaskFlag=@flg ORDER BY j91ID DESC", new { flg = (int) flag });
        }

        public IEnumerable<BO.j91RobotLog> GetListRobotLast20()
        {
            return _db.GetList<BO.j91RobotLog>("select TOP 20 * FROM j91RobotLog ORDER BY j91ID DESC");
        }
    }
}
