using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Im62ExchangeRateBL
    {
        public BO.m62ExchangeRate Load(int pid);
        public IEnumerable<BO.m62ExchangeRate> GetList(BO.myQuery mq);
        public int Save(BO.m62ExchangeRate rec);


    }
    class m62ExchangeRateBL : BaseBL, Im62ExchangeRateBL
    {

        public m62ExchangeRateBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,j27master.j27Code as j27Code_Master,j27slave.j27Code as j27Code_Slave," + _db.GetSQL1_Ocas("m62") + " FROM m62ExchangeRate a");
            sb(" INNER JOIN j27Currency j27master ON a.j27ID_Master=j27master.j27ID INNER JOIN j27Currency j27slave ON a.j27ID_Slave=j27slave.j27ID");
            
            sb(strAppend);
            return sbret();
        }

        public BO.m62ExchangeRate Load(int pid)
        {
            return _db.Load<BO.m62ExchangeRate>(GetSQL1(" WHERE a.m62ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.m62ExchangeRate> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.m62ExchangeRate>(fq.FinalSql, fq.Parameters);
        }


        public int Save(BO.m62ExchangeRate rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();

                p.AddInt("pid", rec.pid);
                p.AddInt("j27ID_Master", rec.j27ID_Master, true);
                p.AddInt("j27ID_Slave", rec.j27ID_Slave, true);
                p.AddEnumInt("m62RateType", rec.m62RateType);
                p.AddDateTime("m62Date", rec.m62Date);
                p.AddDouble("m62Rate", rec.m62Rate);
                p.AddInt("m62Units", rec.m62Units);
               
                int intPID = _db.SaveRecord("m62ExchangeRate", p.getDynamicDapperPars(), rec);
                if (intPID > 0)
                {
                    _db.RunSql("exec dbo.m62_aftersave @m62id,@j03id_sys", new { m62id = intPID, j03id_sys = _mother.CurrentUser.pid });
                }
                sc.Complete();
                return intPID;
            }

        }


        private bool ValidateBeforeSave(BO.m62ExchangeRate rec)
        {            

            if (rec.j27ID_Master == 0)
            {
                this.AddMessage("Chybí zdrojová měna."); return false;
            }
            if (rec.j27ID_Master == 0)
            {
                this.AddMessage("Chybí cílová měna."); return false;
            }
            if (rec.j27ID_Master==rec.j27ID_Slave)
            {
                this.AddMessage("Zdrojová a cílová měna se musí lišit."); return false;
            }
            if (rec.m62Rate <= 0)
            {
                this.AddMessage("Hodnota kurzu musí být větší než nula."); return false;
            }
            if (rec.m62Units <= 0)
            {
                this.AddMessage("Množství musí být větší než nula."); return false;
            }

            return true;
        }
    }
}
