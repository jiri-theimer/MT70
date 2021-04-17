using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip90ProformaBL
    {
        public BO.p90Proforma Load(int pid);
        public BO.p90Proforma LoadMyLastCreated();
        public IEnumerable<BO.p90Proforma> GetList(BO.myQuery mq);
        public int Save(BO.p90Proforma rec, List<BO.FreeFieldInput> lisFFI);
        public BO.p90RecDisposition InhaleRecDisposition(BO.p90Proforma rec);

    }
    class p90ProformaBL : BaseBL, Ip90ProformaBL
    {
        public p90ProformaBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null,int toprec=0)
        {
            if (toprec == 0)
            {
                sb("SELECT a.*");
            }
            else
            {
                sb($"SELECT TOP {toprec} a.*");
            }            
            //sb(",p82.p82ID as p82ID_First,j27.j27Code,p89.p89Name,p28.p28Name");
            sb(",j27.j27Code,p89.p89Name,p28.p28Name");
            sb(",j02owner.j02LastName+' '+j02owner.j02FirstName as Owner");
            sb(",p89.x38ID,");
            sb(_db.GetSQL1_Ocas("p90"));
            sb(" FROM p90Proforma a");
            sb(" INNER JOIN p89ProformaType p89 ON a.p89ID=p89.p89ID");
            sb(" LEFT OUTER JOIN p28Contact p28 ON a.p28ID=p28.p28ID");
            sb(" LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID");
            sb(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID");
            //sb(" LEFT OUTER JOIN (select xa.p90ID,min(p82ID) as p82ID FROM p82Proforma_Payment xa GROUP BY xa.p90ID) p82 ON a.p90ID=p82.p90ID");
            sb(strAppend);
            return sbret();
        }
        public BO.p90Proforma Load(int pid)
        {
            return _db.Load<BO.p90Proforma>(GetSQL1(" WHERE a.p90ID=@pid"), new { pid = pid });
        }
        public BO.p90Proforma LoadMyLastCreated()
        {
            return _db.Load<BO.p90Proforma>(GetSQL1(" WHERE a.j02ID_Owner=@j02id_owner ORDER BY a.p90ID DESC",1), new { j02id_owner = _mother.CurrentUser.j02ID });
        }

        public BO.p90RecDisposition InhaleRecDisposition(BO.p90Proforma rec)
        {
            var c = new BO.p90RecDisposition();

            if (rec.j02ID_Owner == _mother.CurrentUser.j02ID || _mother.CurrentUser.IsAdmin || _mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P90_Owner))
            {
                c.OwnerAccess = true; c.ReadAccess = true;
                return c;
            }
            if (_mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P90_Reader))
            {
                c.ReadAccess = true;
                return c;
            }

            return c;
        }

        public IEnumerable<BO.p90Proforma> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p90Proforma>(fq.FinalSql, fq.Parameters);
        }

        public IEnumerable<BO.p99Invoice_Proforma> GetList_p99(int p90id,int p91id,int p82id)
        {
            sb("SELECT a.*,p90.p90Code,p91.p91Code,p82.p82Code,p82.p90ID,");
            sb(_db.GetSQL1_Ocas("p99",false,false,true));
            sb(" FROM p99Invoice_Proforma a INNER JOIN p82Proforma_Payment p82 ON a.p82ID=p82.p82ID INNER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID INNER JOIN p90Proforma p90 ON p82.p90ID=p90.p90ID");
            object pars = null;

            if (p90id > 0)
            {
                sb(" WHERE p82.p90ID=@p90id");
                pars = new { p90id = p90id };
            }
            if (p91id > 0)
            {
                sb(" WHERE a.p91ID=@p91id");
                pars = new { p91id = p91id };
            }
            if (p82id > 0)
            {
                sb(" WHERE a.p82ID=@p82id");
                pars = new { p82id = p82id };
            }
            return _db.GetList<BO.p99Invoice_Proforma>(sbret(), pars);
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
            if (rec.p90Amount>0 && rec.p90VatRate>0 && rec.p90Amount_WithoutVat == 0 && rec.p90Amount_Vat==0)
            {
                rec.p90Amount_WithoutVat = rec.p90Amount / (1 + rec.p90VatRate / 100);
                rec.p90Amount_Vat = rec.p90Amount - rec.p90Amount_WithoutVat;
            }
            if (rec.p90Amount == 0 && rec.p90VatRate > 0 && rec.p90Amount_WithoutVat > 0 && rec.p90Amount_Vat == 0)
            {                
                rec.p90Amount_Vat = rec.p90Amount_WithoutVat * rec.p90VatRate / 100;
                rec.p90Amount = rec.p90Amount_WithoutVat + rec.p90Amount_Vat;
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
