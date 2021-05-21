using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BL
{
    public interface IFBL
    {       
        public IEnumerable<BO.SysDbObject> GetList_SysObjects();
        public void GenerateCreateUpdateScript(IEnumerable<BO.SysDbObject> lis);
        public IEnumerable<BO.x53Permission> GetListX53();
        public IEnumerable<BO.p87BillingLanguage> GetListP87();
        public BO.p87BillingLanguage LoadP87(int p87id);
        public BO.j27Currency LoadCurrencyByCode(string j27code);
        public BO.j27Currency LoadCurrencyByID(int j27id);
        public IEnumerable<BO.j27Currency> GetListCurrency();
        public int SaveP87(BO.p87BillingLanguage rec);

        public int AppendRobotLog(BO.j91RobotLog rec);  //uložení jetí robota na pozadí
        public BO.j91RobotLog GetLastRobotRun(BO.j91RobotTaskFlag flag); //vrátí poslední jetí pro zadaný flag
        public IEnumerable<BO.j91RobotLog> GetListRobotLast20();
        public IEnumerable<BO.j19PaymentType> GetListJ19();
        public BO.j19PaymentType LoadJ19(int j19id);
        public BO.x21DatePeriod LoadX21(int x21id);
        public IEnumerable<BO.x21DatePeriod> GetListX21(int j03id);
        public bool SaveX21Batch(List<BO.x21DatePeriod> lisX21);
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
        public BO.p87BillingLanguage LoadP87(int p87id)
        {
            return _db.Load<BO.p87BillingLanguage>("SELECT " + _db.GetSQL1_Ocas("p87") + ",a.* FROM p87BillingLanguage a WHERE a.p87ID=@pid", new { pid = p87id });
        }
        public int SaveP87(BO.p87BillingLanguage rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("p87Name", rec.p87Name);
            p.AddString("p87Icon", rec.p87Icon);

            return _db.SaveRecord("p87BillingLanguage", p,rec);
        }
        public IEnumerable<BO.j19PaymentType> GetListJ19()
        {
            return _db.GetList<BO.j19PaymentType>("SELECT " + _db.GetSQL1_Ocas("j19") + ",a.* FROM j19PaymentType a");
        }
        public BO.j19PaymentType LoadJ19(int j19id)
        {
            return _db.Load<BO.j19PaymentType>("SELECT " + _db.GetSQL1_Ocas("j19") + ",a.* FROM j19PaymentType a WHERE a.j19ID=@pid", new { pid = j19id });
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
            return _db.GetList<BO.j27Currency>("SELECT a.*,a.j27ID as pid FROM j27Currency a WHERE GETDATE() BETWEEN a.j27ValidFrom AND a.j27ValidUntil");
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

            return _db.SaveRecord("j91RobotLog", p, rec,false,false);
        }

        public BO.j91RobotLog GetLastRobotRun(BO.j91RobotTaskFlag flag)
        {
            return _db.Load<BO.j91RobotLog>("select TOP 1 * FROM j91RobotLog WHERE j91TaskFlag=@flg ORDER BY j91ID DESC", new { flg = (int) flag });
        }

        public IEnumerable<BO.j91RobotLog> GetListRobotLast20()
        {
            return _db.GetList<BO.j91RobotLog>("select TOP 20 * FROM j91RobotLog ORDER BY j91ID DESC");
        }

        public BO.x21DatePeriod LoadX21(int x21id)
        {
            return _db.Load<BO.x21DatePeriod>("SELECT " + _db.GetSQL1_Ocas("x21",false,false,false) + ",a.* FROM x21DatePeriod a WHERE a.x21ID=@pid", new { pid = x21id });
        }
        public IEnumerable<BO.x21DatePeriod> GetListX21(int j03id)
        {
            return _db.GetList<BO.x21DatePeriod>("SELECT " + _db.GetSQL1_Ocas("x21",false,false,false) + ",a.* FROM x21DatePeriod a WHERE a.x21Ordinary=@j03id ORDER BY a.x21ValidFrom,a.x21ValidUntil",new { j03id = j03id });
        }

        public bool SaveX21Batch(List<BO.x21DatePeriod> lisX21)
        {
            if (lisX21.Any(p => string.IsNullOrEmpty(p.x21Name)))
            {
                this.AddMessage("V pojmenovaném období chybí název.");return false;
            }
            if (lisX21.Any(p => p.x21ValidFrom.Year<=1900 || p.x21ValidUntil.Year <= 1900))
            {
                this.AddMessage("V pojmenovaném období chybí rok."); return false;
            }

            using (var sc = new System.Transactions.TransactionScope())
            {
                foreach (var c in lisX21)
                {
                    if (c.IsTempDeleted)
                    {
                        if (c.pid > 0)
                        {
                            _db.RunSql("DELETE FROM x21DatePeriod WHERE x21ID=@pid", new { pid = c.pid });
                        }
                    }
                    else
                    {
                        if (c.pid == 0)
                        {
                            int intMax = _db.GetIntegerFromSql("select max(x21ID) FROM x21DatePeriod") + 1;
                            if (intMax <= 60) intMax = 61;
                            _db.RunSql("SET IDENTITY_INSERT x21DatePeriod ON; INSERT INTO x21DatePeriod(x21ID,x21Name,x21ValidFrom,x21ValidUntil,x21Ordinary) VALUES(@x21id,@x21name,@d1,@d2,@j03id); SET IDENTITY_INSERT x21DatePeriod OFF", new { x21id = intMax, x21name = c.x21Name, d1 = c.x21ValidFrom, d2 = c.x21ValidUntil, j03id = _mother.CurrentUser.pid });
                        }
                        else
                        {
                            _db.RunSql("UPDATE x21DatePeriod SET x21Ordinary=@j03id,x21Name=@x21name,x21ValidFrom=@d1,x21ValidUntil=@d2 WHERE x21ID=@pid", new { pid = c.pid, x21name = c.x21Name, d1 = c.x21ValidFrom, d2 = c.x21ValidUntil, j03id = _mother.CurrentUser.pid });
                        }

                    }

                }
                sc.Complete();
            }
                

            return true;
        }
    }
}
