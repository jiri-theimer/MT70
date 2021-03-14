using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip34ActivityGroupBL
    {
        public BO.p34ActivityGroup Load(int pid);
        public IEnumerable<BO.p34ActivityGroup> GetList(BO.myQueryP34 mq);
        public int Save(BO.p34ActivityGroup rec, int intP32ID_SystemDefault = 0);
        public IEnumerable<BO.p34ActivityGroup> GetList_WorksheetEntryInProject(int p41id, int p42id, int j02id);

    }
    class p34ActivityGroupBL : BaseBL, Ip34ActivityGroupBL
    {

        public p34ActivityGroupBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,p33.p33Name,p33.p33Code," + _db.GetSQL1_Ocas("p34"));
            sb(" FROM p34ActivityGroup a INNER JOIN p33ActivityInputType p33 ON a.p33ID=p33.p33ID");
            sb(strAppend);
            return sbret();
        }

        public BO.p34ActivityGroup Load(int pid)
        {
            return _db.Load<BO.p34ActivityGroup>(GetSQL1(" WHERE a.p34ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.p34ActivityGroup> GetList(BO.myQueryP34 mq)
        {
            if (mq.explicit_orderby == null) mq.explicit_orderby = "a.p34Ordinary";
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p34ActivityGroup>(fq.FinalSql, fq.Parameters);
        }

        public IEnumerable<BO.p34ActivityGroup> GetList_WorksheetEntryInProject(int p41id,int p42id,int j02id)
        {
            string strEntryFlags = "1,2";
            if (j02id != _mother.CurrentUser.j02ID)
            {
                strEntryFlags = "1,2,4";
            }
            string s = "INNER JOIN p43ProjectType_Workload p43 ON a.p34ID=p43.p34ID";
            s += " WHERE p43.p42ID=@p42id AND a.p34ValidFrom<=getdate() AND a.p34ValidUntil>=getdate()";
            if (_mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P31_Creator))
            {
                //právo vykazovat do všech projektů    
                s += " ORDER BY a.p34Ordinary,a.p34Name";
                return _db.GetList<BO.p34ActivityGroup>(GetSQL1(s), new { p42id = p42id });
            }
            s += " AND (a.p34ID IN (";
            s += "SELECT p34ID FROM x72ProjectRole_Permission_Grid WHERE p41ID=@p41id AND x72IsValid=1 AND o28EntryFlag IN (" + strEntryFlags + ") AND (j02ID=@j02id OR j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))";
            s += ")";
            s += " ORDER BY a.p34Ordinary,a.p34Name";

            return _db.GetList<BO.p34ActivityGroup>(GetSQL1(s), new { p42id = p42id,p41id=p41id,j02id=j02id });
        }

        public int Save(BO.p34ActivityGroup rec,int intP32ID_SystemDefault=0)
        {
            if (!ValidateBeforeSave(rec, intP32ID_SystemDefault))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())     //ukládání v transakci
            {
                var p = new DL.Params4Dapper();

                p.AddInt("pid", rec.pid);
                p.AddInt("p34Ordinary", rec.p34Ordinary);
                p.AddString("p34Name", rec.p34Name);
                p.AddString("p34Code", rec.p34Code);
                p.AddEnumInt("p33ID", rec.p33ID);
                p.AddEnumInt("p34ActivityEntryFlag", rec.p34ActivityEntryFlag);
                p.AddEnumInt("p34IncomeStatementFlag", rec.p34IncomeStatementFlag);
                p.AddInt("p34ValueOffFlag", rec.p34ValueOffFlag);
                p.AddString("p34Name_BillingLang1", rec.p34Name_BillingLang1);
                p.AddString("p34Name_BillingLang2", rec.p34Name_BillingLang2);
                p.AddString("p34Name_BillingLang3", rec.p34Name_BillingLang3);
                p.AddString("p34Name_BillingLang4", rec.p34Name_BillingLang4);

                int intPID = _db.SaveRecord("p34ActivityGroup", p, rec);

                if (intPID > 0)
                {
                    if (rec.p34ActivityEntryFlag == BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava)
                    {
                        _db.RunSql("UPDATE p32Activity SET p32IsSystemDefault=0 WHERE p34ID=@pid", new { pid = intPID });
                        if (intP32ID_SystemDefault > 0)
                        {
                            _db.RunSql("UPDATE p32Activity SET p32IsSystemDefault=1 WHERE p32ID=@p32id", new { p32id = intP32ID_SystemDefault });
                        }
                    }
                    if (rec.pid == 0)   //nový sešit
                    {
                        _db.RunSql("INSERT INTO o28ProjectRole_Workload(p34ID,x67ID,o28EntryFlag,o28PermFlag) select @p34id,x67ID,1,case when x67ID=3 THEN 3 else 0 end FROM x67EntityRole WHERE x29ID=141 AND getdate() BETWEEN x67ValidFrom AND x67ValidUntil", new { p34id = intPID });
                        _db.RunSql("INSERT INTO p43ProjectType_Workload(p42ID,p34ID) SELECT p42ID,@p34id FROM p42ProjectType WHERE getdate() BETWEEN p42ValidFrom AND p42ValidUntil", new { p34id = intPID });
                    }
                    sc.Complete();
                }


                return intPID;
            }
                
        }


        private bool ValidateBeforeSave(BO.p34ActivityGroup rec,int intP32ID_SystemDefault)
        {

            if (string.IsNullOrEmpty(rec.p34Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.p34ActivityEntryFlag == BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava && rec.pid == 0)
            {
                this.AddMessage("Technicky vkládanou aktivitu lze nastavit až po uložení sešitu."); return false;
            }
            if (rec.p34ActivityEntryFlag==BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava && intP32ID_SystemDefault==0)
            {
                this.AddMessage("Chybí vazba na technicky vkládanou aktivitu."); return false;
            }
            

            return true;
        }
    }
}
