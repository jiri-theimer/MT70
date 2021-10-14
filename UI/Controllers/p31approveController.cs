using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models.p31approve;
using UI.Models;

namespace UI.Controllers
{
    public class p31approveController : BaseController 
    {
        public IActionResult Index(string guid,string pids,string prefix,int p72id,int p91id,int approvinglevel,string tempguid)
        {
            if (string.IsNullOrEmpty(pids) && string.IsNullOrEmpty(guid) && string.IsNullOrEmpty(tempguid))
            {
                return this.StopPage(true, "pids or guid missing.");
            }
            

            var v = new GatewayViewModel() { tempguid = tempguid, pidsinline=pids,prefix=prefix,p91id=p91id,p72id=(BO.p72IdENUM) p72id,approvinglevel=approvinglevel };

            
            if (v.tempguid == null)
            {
                v.lisP31 = GetRecords(v, guid);
                v.tempguid = BO.BAS.GetGuid();
                BL.bas.p31Support.SetupTempApproving(this.Factory, v.lisP31, v.tempguid, v.approvinglevel, v.DoDefaultApproveState, v.p72id);
            }
            else
            {
                var mq = new BO.myQueryP31() {tempguid = v.tempguid };
                v.lisP31 = Factory.p31WorksheetBL.GetList(mq);
            }
            
            
            


            if (Factory.CBL.LoadUserParamInt("grid-p31-approve-p31statequery", 0) > 0 && Factory.CBL.LoadUserParamValidityMinutes("grid-p31-approve-p31statequery") > 1)
            {
                //nechceme mít na úvod filtrování podle stavu úkonů
                Factory.CBL.SetUserParam("grid-p31-approve-p31statequery", "0");
                Factory.CBL.ClearUserParamsCache();
            }

            v.j72ID = Factory.CBL.LoadUserParamInt("p31approve-j72id");

            RefreshState_Index(v);
            return View(v);

            
            
        }
        

        [HttpPost]
        public IActionResult Index(GatewayViewModel v,string oper)
        {
            RefreshState_Index(v);

            switch (oper)
            {
                case "saveonly":
                case "saveandbilling":                   
                    if (SaveApproving(v))
                    {
                        v.SetJavascript_CallOnLoad(0);
                        return View(v);
                    }
                    else
                    {                        
                        return View(v);
                    }
                case "rate":
                    UpdateTempRate(v.batchpids, v.BatchInvoiceRate, v.tempguid);                    
                    return View(v);
                case "approvinglevel":
                    UpdateTempApprovingLevel(v.batchpids, v.BatchApproveLevel, v.tempguid);
                    return View(v);

            }

                        
            return View(v);
        }

        private void RefreshState_Index(GatewayViewModel v)
        {
            string strMyQuery = "tempguid|string|" + v.tempguid;
            if (v.SelectedTab != null)
            {
                strMyQuery += "|tabquery|string|" + v.SelectedTab;
            }
            v.gridinput = new TheGridInput() { entity = "p31Worksheet",j72id=v.j72ID, myqueryinline = strMyQuery, oncmclick = "", ondblclick = "",master_entity="approve" };
            v.gridinput.query = new BO.InitMyQuery().Load("p31", "app", 0, strMyQuery);
            
            var cTabs = new NavTabsSupport(Factory);
            v.OverGridTabs = cTabs.getOverGridTabs("approve", v.SelectedTab, false);

            v.P31StateQueryAlias = Factory.tra("Stav úkonů");
            v.P31StateQueryCssClass = "btn btn-light dropdown-toggle";

            v.gridinput.query.p31statequery = Factory.CBL.LoadUserParamInt("grid-p31-approve-p31statequery", 0);
            if (v.gridinput.query.p31statequery > 0)
            {
                v.P31StateQueryCssClass = "btn btn-light dropdown-toggle filtered";
                v.P31StateQueryAlias = new UI.Menu.TheMenuSupport(Factory).getP31StateQueryAlias(v.gridinput.query.p31statequery);
            }

        }


        

        private IEnumerable<BO.p31Worksheet> GetRecords(GatewayViewModel v,string pidsguid)
        {
            var lisPIDs = new List<int>();
            var p31ids = new List<int>();
            if (pidsguid != null)
            {                
                lisPIDs = Factory.p85TempboxBL.GetPidsFromTemp(pidsguid);
            }
            else
            {
                lisPIDs = BO.BAS.ConvertString2ListInt(v.pidsinline);
            }
            var mq = new BO.myQueryP31() { MyRecordsDisponible = true, p31statequery = 3 }; //nevyúčtované            
            switch (v.prefix)
            {
                case "p31":
                    p31ids = lisPIDs;
                    break;
                case "j02":
                case "p28":
                case "p56":
                case "p41":
                    foreach (int pid in lisPIDs)
                    {
                        BO.Reflexe.SetPropertyValue(mq, v.prefix + "id", pid);
                        p31ids.InsertRange(0, Factory.p31WorksheetBL.GetList(mq).Select(p => p.pid));
                    }
                    break;

            }

            mq = new BO.myQueryP31() { MyRecordsDisponible = true, p31statequery = 3, pids = p31ids,explicit_orderby = "a.p41ID,a.p31ID"};
            
            return Factory.p31WorksheetBL.GetList(mq);
        }

        public GridRecord LoadGridRecord(int p31id,string guid)
        {
            var rec = this.Factory.p31WorksheetBL.LoadTempRecord(p31id, guid);
            if (rec == null)
            {
                return new GridRecord() { errormessage = "Záznam nelze načíst: rec is null" };
            }
            var recP41 = this.Factory.p41ProjectBL.Load(rec.p41ID);

            var c = new GridRecord() {Datum = BO.BAS.ObjectDate2String(rec.p31Date), Popis = rec.p31Text,Jmeno=rec.Person,Projekt=rec.Project,Aktivita=rec.p32Name+" ("+rec.p34Name+")",p33id=(int)rec.p33ID };
            c.p71id = (int)rec.p71ID;
            c.p72id = (int)rec.p72ID_AfterApprove;            
            c.pl = this.Factory.CurrentUser.getP07Level(recP41.p07Level,true);
            c.uroven = rec.p31ApprovingLevel;
            c.sazba = rec.p31Rate_Billing_Approved;
            c.bezdph = rec.p31Amount_WithoutVat_Approved;
            c.j27code = rec.j27Code_Billing_Orig;
            c.dphsazba = rec.p31VatRate_Approved;
            switch (rec.p33ID)
            {
                case BO.p33IdENUM.Cas:
                    if (this.Factory.CurrentUser.j03DefaultHoursFormat == "T" || rec.IsRecommendedHHMM())
                    {
                        c.vykazano = rec.p31HHMM_Orig;
                        if (rec.p31Hours_Approved_Billing < rec.p31Hours_Orig)
                        {
                            c.vykazano += " (" + BO.basTime.ShowAsHHMM((rec.p31Hours_Approved_Billing-rec.p31Hours_Orig).ToString()) + ")";
                        }
                        if (rec.p31Hours_Approved_Billing > rec.p31Hours_Orig)
                        {
                            c.vykazano += " (+" + BO.basTime.ShowAsHHMM((rec.p31Hours_Approved_Billing - rec.p31Hours_Orig).ToString()) + ")";
                        }

                    }
                    else
                    {
                        c.vykazano = BO.BAS.Number2String(rec.p31Hours_Orig);
                        if (rec.p31Hours_Approved_Billing < rec.p31Hours_Orig)
                        {
                            c.vykazano += " (" + (rec.p31Hours_Approved_Billing- rec.p31Hours_Orig).ToString() + ")";
                        }
                        if (rec.p31Hours_Approved_Billing > rec.p31Hours_Orig)
                        {
                            c.vykazano += " (+" + (rec.p31Hours_Approved_Billing - rec.p31Hours_Orig).ToString() + ")";
                        }
                    }
                    
                    c.vykazano_sazba = BO.BAS.Number2String(rec.p31Rate_Billing_Orig) + " " + rec.j27Code_Billing_Orig;
                    break;
                case BO.p33IdENUM.Kusovnik:
                    c.vykazano = BO.BAS.Number2String(rec.p31Value_Orig);
                    c.vykazano_sazba = BO.BAS.Number2String(rec.p31Rate_Billing_Orig) + " " + rec.j27Code_Billing_Orig;
                    break;
                default:
                    c.vykazano = BO.BAS.Number2String(rec.p31Amount_WithoutVat_Orig) + " " + rec.j27Code_Billing_Orig;
                    break;

            }
            if (this.Factory.CurrentUser.j03DefaultHoursFormat == "T" || rec.IsRecommendedHHMM_Approved())
            {
                c.hodiny = BO.basTime.ShowAsHHMM(rec.p31Hours_Approved_Billing.ToString());
                
            }
            else
            {
                c.hodiny = rec.p31Hours_Approved_Billing.ToString();
                
            }
            
            if (this.Factory.CurrentUser.j03DefaultHoursFormat == "T" || rec.p31Value_FixPrice.ToString().Length > 5)
            {
                c.hodinypausal = BO.basTime.ShowAsHHMM(rec.p31Value_FixPrice.ToString());
            }
            else
            {
                c.hodinypausal = rec.p31Value_FixPrice.ToString();
            }
            if (this.Factory.CurrentUser.j03DefaultHoursFormat == "T" || rec.p31Hours_Approved_Internal.ToString().Length > 5)
            {
                c.hodinyinterni = BO.basTime.ShowAsHHMM(rec.p31Hours_Approved_Internal.ToString());
            }
            else
            {
                c.hodinyinterni = rec.p31Hours_Approved_Internal.ToString();
            }

            return c;
        }

        

        public BO.Result SaveTempBatch(string pids,int p71id,int p72id,string guid)
        {
            var ret = new BO.Result(false);
            var p31ids = BO.BAS.ConvertString2ListInt(pids);
            int x = 0;
            var errs = new List<string>();

            foreach(int p31id in p31ids)
            {
                x += 1;
                var rec = Factory.p31WorksheetBL.Load(p31id);
                var recTemp = Factory.p31WorksheetBL.LoadTempRecord(p31id, guid);
                var c = new BO.p31WorksheetApproveInput() { p31ID = p31id, Guid = guid, p33ID = recTemp.p33ID, p31Date= recTemp.p31Date };
                c.p71id = (BO.p71IdENUM)p71id;
                c.p31ApprovingLevel = recTemp.p31ApprovingLevel;

                switch (c.p71id)
                {
                    case BO.p71IdENUM.Nic:
                        if (!Factory.p31WorksheetBL.Save_Approving(c, true,true))
                        {
                            errs.Add("#"+x.ToString()+": "+Factory.GetFirstNotifyMessage());
                        }
                        break;
                    case BO.p71IdENUM.Neschvaleno:
                        if (!Factory.p31WorksheetBL.Save_Approving(c, true,true))
                        {
                            errs.Add("#" + x.ToString() + ": " + Factory.GetFirstNotifyMessage());
                        }
                        break;
                    case BO.p71IdENUM.Schvaleno:
                        c.p72id = (BO.p72IdENUM)p72id;
                        if ((c.p72id==BO.p72IdENUM.Fakturovat || c.p72id == BO.p72IdENUM.FakturovatPozdeji))
                        {
                            c.Rate_Billing_Approved = recTemp.p31Rate_Billing_Orig;
                            c.Value_Approved_Billing = recTemp.p31Value_Orig;
                            c.VatRate_Approved = recTemp.p31VatRate_Approved;

                            if (c.Rate_Billing_Approved==0) c.Rate_Billing_Approved = rec.p31Rate_Billing_Orig;
                            if (c.Value_Approved_Billing==0) c.Value_Approved_Billing = rec.p31Value_Orig;
                            
                        }
                        if (!Factory.p31WorksheetBL.Save_Approving(c, true,true))
                        {
                            errs.Add("#" + x.ToString() + ": " + Factory.GetFirstNotifyMessage());
                        }
                        break;
                }
            }

            if (errs.Count() > 0)
            {
                ret.Message = string.Join("<hr>", errs);                
            }

            return ret;
        }
        
        public BO.Result UpdateTempText(int p31id, string s, string guid)
        {
            if (Factory.p31WorksheetBL.UpdateTempText(p31id,s, guid))
            {
                return new BO.Result(false);
            }
            else
            {
                return new BO.Result(true, Factory.GetFirstNotifyMessage());
            }            
        }
        public BO.Result SaveTempRecord(GridRecord rec,int p31id,string guid)
        {
            var ret = new BO.Result(false);
            var c = new BO.p31WorksheetApproveInput() { p31ID = p31id,Guid=guid,p33ID=(BO.p33IdENUM) rec.p33id };
            var recTemp = Factory.p31WorksheetBL.LoadTempRecord(c.p31ID,c.Guid);

            c.p71id = (BO.p71IdENUM)rec.p71id;
            c.p31Date = recTemp.p31Date;

            switch (c.p71id)
            {
                case BO.p71IdENUM.Nic:
                    //rozpracováno
                    if (!Factory.p31WorksheetBL.Save_Approving(c, true,true))
                    {
                        ret.Message = Factory.GetFirstNotifyMessage();
                        ret.Flag = BO.ResultEnum.Failed;
                        return ret;
                    }
                    else
                    {
                        return ret;
                    }                    
                case BO.p71IdENUM.Neschvaleno:
                    //neschváleno
                    if (Factory.p31WorksheetBL.Save_Approving(c, true,true))
                    {
                        return ret;
                    }
                    else
                    {
                        ret.Message = Factory.GetFirstNotifyMessage();
                        ret.Flag = BO.ResultEnum.Failed;
                        return ret;
                    }
                case BO.p71IdENUM.Schvaleno:
                    //schváleno
                    c.p72id = (BO.p72IdENUM)rec.p72id;
                    c.p31Text = rec.Popis;
                    c.p31ApprovingLevel = rec.uroven;
                    break;
            }

            
            
           
            switch (c.p33ID)
            {
                case BO.p33IdENUM.Cas:
                    if (c.p72id == BO.p72IdENUM.Fakturovat || c.p72id == BO.p72IdENUM.FakturovatPozdeji)
                    {
                        c.Value_Approved_Billing = BO.basTime.ShowAsDec(rec.hodiny);
                        c.Rate_Billing_Approved = rec.sazba;
                        if (c.Value_Approved_Billing == 0) c.Value_Approved_Billing = recTemp.p31Value_Orig;
                    }
                    if (c.p72id == BO.p72IdENUM.ZahrnoutDoPausalu)
                    {
                        c.p31Value_FixPrice = BO.basTime.ShowAsDec(rec.hodinypausal);
                    }

                    c.Value_Approved_Internal = BO.basTime.ShowAsDec(rec.hodinyinterni);
                    c.Rate_Internal_Approved = recTemp.p31Rate_Internal_Approved;
                    
                    break;
                case BO.p33IdENUM.Kusovnik:
                    if (c.p72id == BO.p72IdENUM.Fakturovat || c.p72id == BO.p72IdENUM.FakturovatPozdeji)
                    {
                        c.Value_Approved_Billing = BO.basTime.ShowAsDec(rec.hodiny);
                        if (c.Value_Approved_Billing == 0) c.Value_Approved_Billing = recTemp.p31Value_Orig;

                        c.Rate_Billing_Approved = rec.sazba;
                    }
                    
                    
                    break;
                case BO.p33IdENUM.PenizeBezDPH:
                case BO.p33IdENUM.PenizeVcDPHRozpisu:
                    if (c.p72id == BO.p72IdENUM.Fakturovat || c.p72id == BO.p72IdENUM.FakturovatPozdeji)
                    {
                        c.VatRate_Approved = rec.dphsazba;
                        if (c.VatRate_Approved == 0) c.VatRate_Approved = recTemp.p31VatRate_Orig;
                        c.Value_Approved_Billing = rec.bezdph;
                        if (c.Value_Approved_Billing == 0) c.Value_Approved_Billing = recTemp.p31Value_Orig;
                    }
                    

                    break;
            }
            if (!Factory.p31WorksheetBL.Save_Approving(c, true,true))
            {
                ret.Message = Factory.GetFirstNotifyMessage();
                ret.Flag = BO.ResultEnum.Failed;                
            }
            
            return ret;

        }

        private bool SaveApproving(GatewayViewModel v)
        {
            
            var mq = new BO.myQueryP31() { tempguid = v.tempguid };
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq);
            if (v.lisP31.Count() == 0)
            {
                this.AddMessage("Žádné úkony ke schválení.");
                return false;
            }

            for(int pokus = 1; pokus <= 2; pokus++)
            {
                //pokus=1:kontrola, pokus=2:uložení
                var errs = new List<string>(); int x = 1;
                if (pokus==2 && errs.Count > 0)
                {
                    this.AddMessageTranslated(string.Join("<hr>", errs));
                    return false;
                }
                foreach (var rec in v.lisP31)
                {
                    var c = BL.bas.p31Support.InhaleApprovingInput(rec);

                    if (pokus == 1)
                    {
                        if (!Factory.p31WorksheetBL.Validate_Before_Save_Approving(c,false))
                        {
                            errs.Add("#" + x.ToString() + ": " + Factory.GetFirstNotifyMessage());
                        }
                    }
                    else
                    {
                        Factory.p31WorksheetBL.Save_Approving(c, false, true);
                    }
                    
                    x += 1;
                }
            }



            return true;
        }


        private void UpdateTempRate(string pids, double newrate, string guid)
        {
            
            if (newrate <= 0)
            {
                this.AddMessage("Fakturační sazba musí být větší než NULA.");
                return;
            }
            var p31ids = BO.BAS.ConvertString2ListInt(pids);
            int x = 1;
      
            foreach (int p31id in p31ids)
            {
                var rec = Factory.p31WorksheetBL.LoadTempRecord(p31id, guid);
                var c = BL.bas.p31Support.InhaleApprovingInput(rec);
                c.Guid = guid;
                c.Rate_Billing_Approved = newrate;
                if (rec.p33ID !=BO.p33IdENUM.Cas && rec.p33ID != BO.p33IdENUM.Kusovnik)
                {
                    this.AddMessageTranslated("#" + x.ToString() + ": " + Factory.tra("Sazbu lze měnit pouze pro časové nebo kusovníkové úkony."));
                }
                else
                {
                    Factory.p31WorksheetBL.Save_Approving(c, true, true);
                }
                
                x += 1;
            }

            

        }

        private void UpdateTempApprovingLevel(string pids, int newlevel, string guid)
        {

           
            var p31ids = BO.BAS.ConvertString2ListInt(pids);
            foreach (int p31id in p31ids)
            {
                var rec = Factory.p31WorksheetBL.LoadTempRecord(p31id, guid);
                var c = BL.bas.p31Support.InhaleApprovingInput(rec);
                c.Guid = guid;
                c.p31ApprovingLevel = newlevel;
             
                Factory.p31WorksheetBL.Save_Approving(c, true, true);

            }



        }
    }
}
