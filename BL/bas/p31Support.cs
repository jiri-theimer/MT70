using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.bas
{
    public static class p31Support
    {
        
        public static int SaveOrigRecord(BL.Factory _f, DL.DbHandler _db,BO.p31WorksheetEntryInput rec, BO.p33IdENUM p33ID, List<BO.FreeField> lisFF)
        {
            int intSavedP31ID = 0;

            using (var sc = new System.Transactions.TransactionScope())     // ukládání podléhá transakci
            {
                var p = new DL.Params4Dapper();

                p.Add("pid", rec.PID);
                if (rec.PID == 0)
                {
                    p.AddInt("j02ID_Owner", _f.CurrentUser.j02ID, true);
                    p.AddDouble("p31ExchangeRate_Fixed", 1);
                }
                else
                {
                    p.AddInt("j02ID_Owner", rec.j02ID, true);
                }

                p.AddInt("p41ID", rec.p41ID, true);
                p.AddInt("j02ID", rec.j02ID, true);
                p.AddInt("p56ID", rec.p56ID, true);
                p.AddInt("p32ID", rec.p32ID, true);
                p.AddInt("p72ID_AfterTrimming", (int)rec.p72ID_AfterTrimming, true);
                p.AddInt("p28ID_Supplier", rec.p28ID_Supplier, true);
                p.AddInt("j02ID_ContactPerson", rec.j02ID_ContactPerson, true);

                p.AddString("p31Text", rec.p31Text);
                p.AddEnumInt("p31HoursEntryflag", rec.p31HoursEntryflag);
                p.AddDateTime("p31Date", rec.p31Date);
                p.AddDateTime("p31DateUntil", rec.p31DateUntil);

                p.AddDouble("p31Value_Orig", rec.p31Value_Orig);
                p.AddDouble("p31Value_Trimmed", rec.p31Value_Trimmed);
                p.AddInt("p31Minutes_Orig", rec.p31Minutes_Orig);
                p.AddInt("p31Minutes_Trimmed", rec.p31Minutes_Trimmed);
                p.AddString("p31HHMM_Orig", rec.p31HHMM_Orig);
                p.AddDouble("p31Hours_Orig", rec.p31Hours_Orig);
                p.AddDouble("p31Hours_Trimmed", rec.p31Hours_Trimmed);
                p.AddDateTime("p31DateTimeFrom_Orig", rec.p31DateTimeFrom_Orig);
                p.AddDateTime("p31DateTimeUntil_Orig", rec.p31DateTimeUntil_Orig);

                p.AddString("p31Value_Orig_Entried", BO.BAS.LeftString(rec.Value_Orig_Entried, 20));
                p.AddString("p31ExternalPID", rec.p31ExternalPID);

                p.AddString("p31PostRecipient", rec.p31PostRecipient);
                p.AddString("p31PostCode", rec.p31PostCode);
                p.AddInt("p31PostFlag", rec.p31PostFlag, true);

                if (rec.PID == 0 && rec.p31RecordSourceFlag == 0)
                {
                    p.AddInt("p31RecordSourceFlag", rec.p31RecordSourceFlag);
                }
                p.AddString("p31Code", rec.p31Code);

                if (p33ID == BO.p33IdENUM.PenizeBezDPH || p33ID == BO.p33IdENUM.PenizeVcDPHRozpisu)
                {
                    p.AddDouble("p31Amount_WithoutVat_Orig", rec.p31Amount_WithoutVat_Orig);
                    p.AddDouble("p31VatRate_Orig", rec.VatRate_Orig);
                    p.AddDouble("p31Amount_WithVat_Orig", rec.p31Amount_WithVat_Orig);
                    p.AddDouble("p31Amount_Vat_Orig", rec.p31Amount_Vat_Orig);
                    p.AddInt("j27ID_Billing_Orig", rec.j27ID_Billing_Orig, true);
                    p.AddDouble("p31Calc_Pieces", rec.p31Calc_Pieces);
                    p.AddDouble("p31Calc_PieceAmount", rec.p31Calc_PieceAmount);
                    p.AddInt("p35ID", rec.p35ID, true);
                    p.AddInt("p49ID", rec.p49ID, true);
                    p.AddInt("j19ID", rec.j19ID, true);
                    p.AddDouble("p31MarginHidden", rec.p31MarginHidden);
                    p.AddDouble("p31MarginTransparent", rec.p31MarginTransparent);
                }

                if ((p33ID == BO.p33IdENUM.Cas || p33ID == BO.p33IdENUM.Kusovnik) && rec.ManualFee != 0)
                    p.AddDouble("p31Amount_WithoutVat_Orig", rec.ManualFee);    // pevný honorář

                if (p33ID == BO.p33IdENUM.Cas)
                    p.AddDateTime("p31TimerTimestamp", rec.p31TimerTimestamp);

                if (rec.p31ValidUntil != null)
                    p.AddDateTime("p31ValidUntil", rec.p31ValidUntil);


                p.AddDouble("p31Value_Off", rec.Value_OffBilling);


                intSavedP31ID = _db.SaveRecord("p31Worksheet", p.getDynamicDapperPars(), rec, true, true);

                if (intSavedP31ID > 0)
                {

                    //if (!lisFF == null)
                    //    bas.SaveFreeFields(_cDB, lisFF, "p31Worksheet_FreeField", intSavedP31ID);


                    var pars = new Dapper.DynamicParameters();
                    {

                        pars.Add("p31id", intSavedP31ID, System.Data.DbType.Int32);
                        pars.Add("j03id_sys", _f.CurrentUser.pid, System.Data.DbType.Int32);
                        pars.Add("x45ids", null, System.Data.DbType.String, System.Data.ParameterDirection.Output, 50);
                    }

                    if (_db.RunSp("p31_aftersave", ref pars, false) == "1")
                    {
                        sc.Complete();

                    }
                    else
                    {
                        return 0;
                    }

                }
                else
                {
                    return 0;
                }
            }

            return intSavedP31ID;
        }



        public static BO.p31ValidateBeforeSave ValidateBeforeSaveOrigRecord(BL.Factory _f, DL.DbHandler _db,BO.p31WorksheetEntryInput rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("p31id", rec.PID, true);
            p.AddInt("j03id_sys", _f.CurrentUser.pid, true);
            p.AddInt("j02id_rec", rec.j02ID, true);
            p.AddInt("p41id", rec.p41ID, true);
            p.AddInt("p56id", rec.p56ID, true);
            p.AddDateTime("p31date", rec.p31Date);
            p.AddInt("p32id", rec.p32ID, true);
            p.AddDouble("p31vatrate_orig", rec.VatRate_Orig);
            p.AddInt("j27id_explicit", rec.j27ID_Billing_Orig, true);
            p.AddString("p31text", rec.p31Text);
            p.AddDouble("value_orig", rec.p31Value_Orig);
            p.AddDouble("manualfee", rec.ManualFee);

            var pars = p.getDynamicDapperPars();

            pars.Add("err", null, System.Data.DbType.String, System.Data.ParameterDirection.Output, 500);
            pars.Add("round2minutes", null, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            pars.Add("j27id_domestic", null, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            pars.Add("p33id", null, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            pars.Add("vatrate", null, System.Data.DbType.Double, System.Data.ParameterDirection.Output);


            var ret = new BO.p31ValidateBeforeSave();

            string s = _db.RunSp("p31_test_beforesave", ref pars, false);
            if (s == "1")
            {
                ret.ErrorMessage = pars.Get<string>("err");
            }
            else
            {
                ret.ErrorMessage = s;
            }
            if (string.IsNullOrEmpty(ret.ErrorMessage))
            {
                ret.Round2Minutes = pars.Get<int>("round2minutes");
                ret.j27ID_Domestic = pars.Get<int>("j27id_domestic");
                ret.p33ID = pars.Get<BO.p33IdENUM>("p33id");
                if (rec.x15ID == BO.x15IdEnum.Nic)
                {
                    //dph sazba vychází implicitně z globálního nastavení
                    ret.VatRate = pars.Get<double>("vatrate");
                }
                else
                {
                    ret.VatRate = rec.VatRate_Orig;
                }
            }


            return ret;
        }
    }
}
