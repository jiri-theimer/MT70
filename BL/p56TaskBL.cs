using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip56TaskBL
    {
        public BO.p56Task Load(int pid);
        public IEnumerable<BO.p56Task> GetList(BO.myQuery mq);
        public int Save(BO.p56Task rec);
        public BO.p56TaskSum LoadSumRow(int pid);
        public BO.p56RecDisposition InhaleRecDisposition(BO.p56Task rec);

    }
    class p56TaskBL : BaseBL, Ip56TaskBL
    {
        public p56TaskBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.p41ID,a.p57ID,a.j02ID_Owner,a.b02ID,a.p56Name,a.p56NameShort,a.p56Code,a.p56Description,a.p56Ordinary,a.p56PlanFrom,a.p56PlanUntil,a.p56Plan_Hours,a.p56Plan_Expenses,a.p56RatingValue,a.p56CompletePercent,a.p56ExternalPID,a.p56IsPlan_Hours_Ceiling,a.p56IsPlan_Expenses_Ceiling,a.p56IsHtml,a.p56IsNoNotify");
            sb(",p28client.p28Name as Client,p57.p57Name,isnull(p41.p41NameShort,p41.p41Name) as p41Name,p41.p41Code,b02.b02Name,b02.b02Color,j02owner.j02LastName+' '+j02owner.j02FirstName as Owner,p57.b01ID");
            sb(","+_db.GetSQL1_Ocas("p56"));
            sb(",dbo.p56_getroles_inline(a.p56ID) as ReceiversInLine");
            sb(" FROM p56Task a");
            sb(" INNER JOIN p57TaskType p57 ON a.p57ID=p57.p57ID INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID");
            sb(" LEFT OUTER JOIN p28Contact p28client ON p41.p28ID_Client=p28client.p28ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID");
            sb(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID");
            sb(" LEFT OUTER JOIN p56Task_FreeField p56free ON a.p56ID=p56free.p56ID");
            sb(strAppend);
            return sbret();
        }
        public BO.p56Task Load(int pid)
        {
            return _db.Load<BO.p56Task>(GetSQL1(" WHERE a.p56ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.p56Task> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p56Task>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p56Task rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }

            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddString("p56Name", rec.p56Name);
                p.AddString("p56Code", rec.p56Code);
                p.AddString("p56Description", rec.p56Description);
                p.AddBool("p56IsNoNotify", rec.p56IsNoNotify);
                p.AddInt("p56CompletePercent", rec.p56CompletePercent);

                p.AddDateTime("p56PlanFrom", rec.p56PlanFrom);
                p.AddDateTime("p56PlanUntil", rec.p56PlanUntil);
                p.AddDouble("p56Plan_Hours", rec.p56Plan_Hours);
                p.AddDouble("p56Plan_Expenses", rec.p56Plan_Expenses);
                p.AddBool("p56IsPlan_Hours_Ceiling", rec.p56IsPlan_Hours_Ceiling);
                p.AddBool("p56IsPlan_Expenses_Ceiling", rec.p56IsPlan_Expenses_Ceiling);

                int intPID = _db.SaveRecord("p56Task", p, rec);
                if (intPID > 0)
                {
                    var pars = new Dapper.DynamicParameters();
                    {
                        pars.Add("p56id", intPID, System.Data.DbType.Int32);
                        pars.Add("j03id_sys", _mother.CurrentUser.pid, System.Data.DbType.Int32);
                    }

                    if (_db.RunSp("p56_aftersave", ref pars, false) == "1")
                    {
                        sc.Complete();                        
                    }
                    else
                    {
                        return 0;
                    }
                }
                return intPID;
            }
                            
        }
        private bool ValidateBeforeSave(BO.p56Task rec)
        {
            if (string.IsNullOrEmpty(rec.p56Name))
            {
                this.AddMessage("Chybí vyplnit [Název úkolu]."); return false;
            }
            if (rec.p57ID==0)
            {
                this.AddMessage("Chybí vyplnit [Typ úkolu]."); return false;
            }
            if (rec.p41ID == 0)
            {
                this.AddMessage("Chybí vazba na projekt.");return false;
            }
            if (rec.pid == 0 && rec.j02ID_Owner == 0) rec.j02ID_Owner = _mother.CurrentUser.j02ID;
            if (rec.j02ID_Owner == 0)
            {
                this.AddMessage("Chybí vyplnit [Vlastník záznamu]."); return false;
            }
            if (rec.p56PlanUntil != null)
            {
                if (Convert.ToDateTime(rec.p56PlanUntil).ToString("HH:mm") == "00:00")
                {
                    Convert.ToDateTime(rec.p56PlanUntil).AddDays(1).AddSeconds(-1);
                }
            }
            if (rec.p56PlanUntil !=null && rec.p56PlanFrom != null)
            {
                if (rec.p56PlanUntil < rec.p56PlanFrom)
                {
                    this.AddMessage("Čas plánovaného zahájení úkolu nesmí být větší než Termín úkolu.");return false;
                }
            }
            if (rec.p56IsPlan_Expenses_Ceiling && rec.p56Plan_Expenses<=0)
            {
                this.AddMessage("Chybí zadání plánu peněžních výdajů.");return false;
            }
            if (rec.p56IsPlan_Hours_Ceiling && rec.p56Plan_Hours <= 0)
            {
                this.AddMessage("Chybí zadání plánu hodin."); return false;
            }


            return true;
        }

        public BO.p56TaskSum LoadSumRow(int pid)
        {
            return _db.Load<BO.p56TaskSum>("EXEC dbo.p56_inhale_sumrow @j03id_sys,@pid", new { j03id_sys = _mother.CurrentUser.pid, pid = pid });
        }

        public BO.p56RecDisposition InhaleRecDisposition(BO.p56Task rec)
        {
            var c = new BO.p56RecDisposition();

            if (rec.j02ID_Owner == _mother.CurrentUser.j02ID || _mother.CurrentUser.IsAdmin)
            {
                c.OwnerAccess = true; c.ReadAccess = true;
                return c;
            }
            if (_mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P56_Reader))
            {
                c.ReadAccess = true;
                return c;
            }

            return c;
        }

    }
}
