﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip90ProformaBL
    {
        public BO.p90Proforma Load(int pid);
        public IEnumerable<BO.p90Proforma> GetList(BO.myQuery mq);
        public int Save(BO.p90Proforma rec, List<BO.FreeFieldInput> lisFFI);

    }
    class p90ProformaBL : BaseBL, Ip90ProformaBL
    {
        public p90ProformaBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*");
            sb(",p82.p82ID as p82ID_First,j27.j27Code,p89.p89Name,p28.p28Name");
            sb(",j02owner.j02LastName+' '+j02owner.j02FirstName as Owner");
            sb(",");
            sb(_db.GetSQL1_Ocas("p90"));
            sb(" FROM p90Proforma a");
            sb(" FROM p90Proforma a INNER JOIN p89ProformaType p89 ON a.p89ID=p89.p89ID");
            sb(" LEFT OUTER JOIN p28Contact p28 ON a.p28ID=p28.p28ID");
            sb(" LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID");
            sb(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID");
            sb(" LEFT OUTER JOIN (select xa.p90ID,min(p82ID) as p82ID FROM p82Proforma_Payment xa GROUP BY xa.p90ID) p82 ON a.p90ID=p82.p90ID");
            sb(strAppend);
            return sbret();
        }
        public BO.p90Proforma Load(int pid)
        {
            return _db.Load<BO.p90Proforma>(GetSQL1(" WHERE a.p90ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.p90Proforma> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p90Proforma>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p90Proforma rec, List<BO.FreeFieldInput> lisFFI)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddBool("p90IsDraft", rec.p90IsDraft);

                p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
                p.AddInt("p89ID", rec.p89ID, true);
                p.AddInt("p28ID", rec.p28ID, true);
                p.AddInt("j27ID", rec.j27ID, true);
                p.AddInt("j19ID", rec.j19ID, true);
                p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);

                p.AddString("p90Code", rec.p90Code);
                p.AddString("p90Text1", rec.p90Text1);
                p.AddString("p90Text2", rec.p90Text2);
                p.AddString("p90TextDPP", rec.p90TextDPP);

                p.AddDouble("p90Amount_WithoutVat", rec.p90Amount_WithoutVat);
                p.AddDouble("p90Amount_Vat", rec.p90Amount_Vat);
                p.AddDouble("p90VatRate", rec.p90VatRate);
                p.AddDouble("p90Amount", rec.p90Amount);

                p.AddDateTime("p90Date", rec.p90Date);
                p.AddDateTime("p90DateMaturity", rec.p90DateMaturity);


                int intPID = _db.SaveRecord("p90Proforma", p, rec);
                if (intPID > 0)
                {
                    if (!DL.BAS.SaveFreeFields(_db, intPID, lisFFI))
                    {
                        return 0;
                    }

                    if (_db.RunSql("exec dbo.p90_aftersave @p90id,@j03id_sys", new { p90id = intPID, j03id_sys = _mother.CurrentUser.pid }))
                    {
                        sc.Complete();
                    }

                }

                return intPID;
            }
                

        }
        private bool ValidateBeforeSave(BO.p90Proforma rec)
        {
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _mother.CurrentUser.j02ID;
            if (rec.p90Amount > 0 && rec.p90Amount_WithoutVat>0)
            {
                if (Math.Abs((rec.p90Amount_WithoutVat + rec.p90Amount_Vat) - rec.p90Amount) > 0.01)
                {
                    this.AddMessage("Částka bez DPH + částka DPH musí souhlasit s celkovou částkou.");return false;
                }
            }
            if (rec.j27ID==0)
            {
                this.AddMessage("Chybí vyplnit [Měna]."); return false;
            }
            if (rec.p89ID==0)
            {
                this.AddMessage("Chybí vyplnit [Typ zálohy]."); return false;
            }
            if (rec.p28ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Klient]."); return false;
            }
            return true;
        }

    }
}
