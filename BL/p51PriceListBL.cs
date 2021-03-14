using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip51PriceListBL
    {
        public BO.p51PriceList Load(int pid);
        
        public IEnumerable<BO.p51PriceList> GetList(BO.myQuery mq);
        public int Save(BO.p51PriceList rec, List<BO.p52PriceList_Item> lisP52);

        public IEnumerable<BO.p52PriceList_Item> GetList_p52(int p51id);
    }
    class p51PriceListBL : BaseBL, Ip51PriceListBL
    {
        public p51PriceListBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,master.p51Name as p51Name_Master,j27.j27Code,");
            sb(_db.GetSQL1_Ocas("p51"));
            sb(" FROM p51PriceList a LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID");
            sb(" LEFT OUTER JOIN p51PriceList master ON a.p51ID_Master=master.p51ID");
            
            sb(strAppend);
            return sbret();
        }
        public BO.p51PriceList Load(int pid)
        {
            return _db.Load<BO.p51PriceList>(GetSQL1(" WHERE a.p51ID=@pid"), new { pid = pid });
        }
        

        public IEnumerable<BO.p51PriceList> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p51PriceList>(fq.FinalSql, fq.Parameters);
        }

        public IEnumerable<BO.p52PriceList_Item> GetList_p52(int p51id)
        {
            sb("select a.*,j02.j02LastName+' '+j02.j02FirstName as Person,j07.j07Name,p34.p34Name,p32.p32Name,");
            sb(_db.GetSQL1_Ocas("p52",false,false,true));
            sb(" FROM p52PriceList_Item a LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID");
            sb(" LEFT OUTER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID");
            sb(" LEFT OUTER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID");
            sb(" LEFT OUTER JOIN p32Activity p32 ON a.p32ID=p32.p32ID");
            sb(" WHERE a.p51ID=@p51id");
            return _db.GetList<BO.p52PriceList_Item>(sbret(),new { p51id = p51id });
        }


        public int Save(BO.p51PriceList rec,List<BO.p52PriceList_Item> lisP52)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())     //ukládání podléhá transakci{
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddEnumInt("p51TypeFlag", rec.p51TypeFlag);
                p.AddInt("j27ID", rec.j27ID, true);
                p.AddInt("p51ID_Master", rec.p51ID_Master, true);
                p.AddBool("p51IsMasterPriceList", rec.p51IsMasterPriceList);
                p.AddBool("p51IsCustomTailor", rec.p51IsCustomTailor);
                p.AddString("p51Name", rec.p51Name);
                p.AddString("p51Code", rec.p51Code);
                p.AddDouble("p51DefaultRateT", rec.p51DefaultRateT);
                p.AddInt("p51Ordinary", rec.p51Ordinary);
                

                int intPID = _db.SaveRecord("p51PriceList", p, rec);
                if (intPID > 0)
                {
                    if (lisP52 != null)
                    {
                        if (rec.pid > 0)
                        {
                            _db.RunSql("DELETE FROM p52PriceList_Item WHERE p51ID = @pid", new { pid = intPID });
                        }
                        foreach(var c in lisP52)
                        {
                            p = new DL.Params4Dapper();
                            p.AddInt("p51ID", intPID, true);
                            p.AddInt("j02ID", c.j02ID, true);
                            p.AddInt("j07ID", c.j07ID, true);
                            p.AddInt("p34ID", c.p34ID, true);
                            p.AddInt("p32ID", c.p32ID, true);
                            p.AddString("p52Name", c.p52Name);
                            p.AddDouble("p52Rate", c.p52Rate);
                            p.AddBool("p52IsMaster", c.p52IsMaster);
                            p.AddBool("p52IsPlusAllTimeSheets", c.p52IsPlusAllTimeSheets);

                            _db.SaveRecord("p52PriceList_Item", p, c,false,true);
                        }
                    }
                    var pars = new Dapper.DynamicParameters();
                    {
                        pars.Add("p51id", intPID, System.Data.DbType.Int32);
                        pars.Add("j03id_sys", _mother.CurrentUser.pid, System.Data.DbType.Int32);
                    }
                    if (_db.RunSp("p51_aftersave", ref pars, false) == "1")
                    {
                        sc.Complete();
                        return intPID;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return intPID;
            }


        }
        private bool ValidateBeforeSave(BO.p51PriceList rec)
        {
            
            if (string.IsNullOrEmpty(rec.p51Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.j27ID == 0)
            {
                this.AddMessage("Chybí měna ceníku."); return false;
            }
            if (rec.p51IsMasterPriceList && rec.pid>0)
            {
                if (GetList(new BO.myQuery("p51")).Where(p=>p.p51ID_Master==rec.pid && p.j27ID != rec.j27ID).Count() > 0)
                {
                    this.AddMessage("Měna MASTER ceníku musí být shodná s měnou jeho podřízených ceníků!");return false;
                }
            }
            else
            {
                if (rec.p51ID_Master > 0)
                {
                    var recMaster = Load(rec.p51ID_Master);
                    if (recMaster == null)
                    {
                        rec.p51ID_Master = 0;
                    }
                    else
                    {
                        if (recMaster.j27ID != rec.j27ID)
                        {
                            this.AddMessage(String.Format("Měna ceníku musí být shodná s měnou jeho MASTER ceníku {0}!", recMaster.j27Code));
                        }
                    }
                }
            }

            if (rec.pid > 0 && rec.p51TypeFlag==BO.p51TypeFlagENUM.RootBillingRates)
            {
                var lisP41 = _mother.p41ProjectBL.GetList(new BO.myQueryP41() { p51id = rec.pid });
                if (lisP41.Count() > 0)
                {
                    this.AddMessage(String.Format("Minimálně jeden projekt ({0}) je svázán s tímto ceníkem. Kořenový (ROOT) ceník nemůže být přímo svázán s projektem!", lisP41.First().FullName));
                }

            }

            return true;
        }

    }
}
