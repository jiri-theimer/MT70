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
        public IActionResult Index(string guid,string pids,string prefix,int p72id,int p91id,int approvinglevel)
        {
            if (string.IsNullOrEmpty(pids) && string.IsNullOrEmpty(guid))
            {
                return this.StopPage(true, "pids or guid missing.");
            }
            

            var v = new GatewayViewModel() { guid = guid,pidsinline=pids,prefix=prefix,p91id=p91id,p72id=(BO.p72IdENUM) p72id,approvinglevel=approvinglevel };

            v.lisP31 = GetRecords(v);
            SetupTempData(v);

            v.lisP31 = null;v.pidsinline = null;    //z důvodu, aby se nepřenášeli obrovská data přes querystring
            return this.RedirectToAction("Grid",v);
            
        }
        public IActionResult Inline(GatewayViewModel v)
        {
            return View(v);
        }
        public IActionResult Grid(GatewayViewModel v)
        {            
            v.j72ID = Factory.CBL.LoadUserParamInt("p31approve-j72id");
            
            RefreshState_Grid(v);
            return View(v);
        }

        [HttpPost]
        public IActionResult Grid(GatewayViewModel v,string oper)
        {
            RefreshState_Grid(v);
            return View(v);
        }

        private void RefreshState_Grid(GatewayViewModel v)
        {
            string strMyQuery = "tempguid|string|" + v.guid;
            v.gridinput = new TheGridInput() { entity = "p31Worksheet",j72id=v.j72ID, myqueryinline = strMyQuery, oncmclick = "", ondblclick = "",master_entity="approve" };
            v.gridinput.query = new BO.InitMyQuery().Load("p31", "app", 0, strMyQuery);
            
        }


        private void SetupTempData(GatewayViewModel v)
        {
            int intLastP41ID = 0;int intP41ID = 0;int intSpatnaKorekceRows = 0;BO.p41Project recP41 = null;
            if (v.guid == null) v.guid = BO.BAS.GetGuid();
            foreach (var rec in v.lisP31)
            {
                intP41ID = rec.p41ID;
                var c = new BO.p31WorksheetApproveInput() { p31ID = rec.pid, p33ID = rec.p33ID, Guid = v.guid,p31Date=rec.p31Date, p31ApprovingLevel = rec.p31ApprovingLevel };
                if (rec.p71ID == BO.p71IdENUM.Nic)
                {
                    //dosud neprošlo schvalováním
                    c.Rate_Internal_Approved = rec.p31Rate_Internal_Orig;
                    c.p31ApprovingLevel = v.approvinglevel;
                    if (v.DoDefaultApproveState)
                    {
                        c.p71id = BO.p71IdENUM.Schvaleno;
                        if (rec.p32IsBillable)
                        {
                            c.p72id = BO.p72IdENUM.Fakturovat;
                            c.Value_Approved_Billing = rec.p31Value_Orig;
                            c.Value_Approved_Internal = rec.p31Value_Orig;
                            switch (c.p33ID)
                            {
                                case BO.p33IdENUM.Cas:
                                case BO.p33IdENUM.Kusovnik:
                                    c.Rate_Billing_Approved = rec.p31Rate_Billing_Orig;
                                    c.VatRate_Approved = rec.p31VatRate_Orig;
                                    if ((rec.p31Rate_Billing_Orig==0 && rec.p32ManualFeeFlag==0) || (rec.p31Amount_WithoutVat_Orig==0 && rec.p32ManualFeeFlag == 0))
                                    {
                                        c.p72id = BO.p72IdENUM.ZahrnoutDoPausalu;
                                    }
                                    if (rec.p32ManualFeeFlag == 1)
                                    {
                                        c.p32ManualFeeFlag = 1;
                                        c.ManualFee_Approved = rec.p31Amount_WithoutVat_Orig;
                                    }
                                    if (rec.p72ID_AfterTrimming != BO.p72IdENUM._NotSpecified)
                                    {
                                        //uživatel zadal v úkonu výchozí korekci pro schvalování
                                        c.p72id = rec.p72ID_AfterTrimming;
                                        if (c.p72id == BO.p72IdENUM.Fakturovat)
                                        {
                                            c.Value_Approved_Billing = rec.p31Value_Trimmed;
                                            if (c.Rate_Billing_Approved == 0)
                                            {
                                                //korekce zní na status [fakturovat], ale hodinová sazba je nulová -> je třeba nahodit paušál
                                                c.p72id = BO.p72IdENUM.ZahrnoutDoPausalu;
                                                c.Value_Approved_Billing = 0;
                                                if (intSpatnaKorekceRows == 0)
                                                {
                                                    var cTmpErr = new BO.p85Tempbox() { p85GUID = "err-" + v.guid, p85DataPID=rec.pid, p85FreeText01= "Minimálně u jednoho úkonu zapisovač navrhl korekcí status [Fakturovat], ale úkon má nulovou fakturační sazbu.<hr>V takovém případě systém nahazuje status [Zahrnout do paušálu]." };
                                                    this.Factory.p85TempboxBL.Save(cTmpErr);

                                                }
                                                intSpatnaKorekceRows += 1;
                                            }
                                        }
                                        else
                                        {
                                            c.Rate_Billing_Approved = 0;
                                            c.Value_Approved_Billing = 0;
                                        }
                                    }
                                    else
                                    {
                                        //fakturovatelné hodiny mohou být záměrně nulovány do paušálu nebo odpisu
                                        if (intP41ID != intLastP41ID || recP41==null)
                                        {
                                            recP41 = this.Factory.p41ProjectBL.Load(intP41ID);
                                        }
                                        if (recP41.p72ID_BillableHours != BO.p72IdENUM._NotSpecified)
                                        {
                                            c.p72id = recP41.p72ID_BillableHours;   //v projektu je explicitně nastavený fakturační status, kterým se mají nulovat fakturovatelné hodiny
                                        }
                                    }
                                    break;
                                case BO.p33IdENUM.PenizeBezDPH:
                                    if (rec.p31Value_Orig == 0)
                                    {
                                        c.p72id = BO.p72IdENUM.ZahrnoutDoPausalu;
                                    }
                                    break;
                                case BO.p33IdENUM.PenizeVcDPHRozpisu:
                                    c.VatRate_Approved = rec.p31VatRate_Orig;
                                    if (rec.p31Value_Orig == 0)
                                    {
                                        c.p72id = BO.p72IdENUM.ZahrnoutDoPausalu;
                                    }
                                    switch (rec.p72ID_AfterTrimming)
                                    {
                                        case BO.p72IdENUM._NotSpecified:                                            
                                            break;
                                        case BO.p72IdENUM.Fakturovat:
                                            c.p72id = rec.p72ID_AfterTrimming ;
                                            c.Value_Approved_Billing = rec.p31Value_Trimmed;
                                            break;
                                        default:
                                            c.p72id = rec.p72ID_AfterTrimming;
                                            c.Value_Approved_Billing = 0;
                                            break;
                                    }
                                    break;
                            }
                            if (rec.p31MarginHidden !=0 || rec.p31MarginTransparent != 0)
                            {
                                //navýšit částku o expense marži
                                c.Value_Approved_Billing = rec.p31Value_Orig + (rec.p31Value_Orig * rec.p31MarginHidden / 100);
                                c.Value_Approved_Billing = c.Value_Approved_Billing + (c.Value_Approved_Billing * rec.p31MarginTransparent / 100);
                            }
                        }
                        else
                        {
                            c.Value_Approved_Internal = rec.p31Value_Orig;
                            if (rec.p72ID_AfterTrimming ==BO.p72IdENUM._NotSpecified || rec.p72ID_AfterTrimming == BO.p72IdENUM.Fakturovat)
                            {
                                c.p72id = this.Factory.p31WorksheetBL.Get_p72ID_NonBillableWork(c.p31ID);
                            }
                            else
                            {
                                c.p72id = rec.p72ID_AfterTrimming;
                            }
                        }
                    }
                }
                else
                {
                    //již dříve prošlo schvalováním
                    c.p71id = rec.p71ID;
                    c.p72id = rec.p72ID_AfterApprove;
                    c.Value_Approved_Billing = rec.p31Value_Approved_Billing;
                    c.Value_Approved_Internal = rec.p31Value_Approved_Internal;
                    c.VatRate_Approved = rec.p31VatRate_Approved;
                    c.Rate_Billing_Approved = rec.p31Rate_Billing_Approved;
                    c.Rate_Internal_Approved = rec.p31Rate_Internal_Approved;
                    c.p32ManualFeeFlag = rec.p32ManualFeeFlag;
                    if (rec.p32ManualFeeFlag == 1)
                    {
                        c.ManualFee_Approved = rec.p31Amount_WithoutVat_Approved;
                    }
                    if (c.p72id == BO.p72IdENUM.ZahrnoutDoPausalu)
                    {
                        c.p31Value_FixPrice = rec.p31Value_FixPrice;
                    }
                }
                if (v.p72id !=BO.p72IdENUM._NotSpecified && v.p72id !=BO.p72IdENUM.Fakturovat && (rec.p33ID==BO.p33IdENUM.Cas || rec.p33ID == BO.p33IdENUM.Kusovnik))
                {
                    c.p72id = v.p72id;
                    c.Value_Approved_Billing = 0;
                }

                if (!this.Factory.p31WorksheetBL.Save_Approving(c,true))
                {

                }

                intLastP41ID = intP41ID;

            }

        }

        private IEnumerable<BO.p31Worksheet> GetRecords(GatewayViewModel v)
        {
            var lisPIDs = new List<int>();
            var p31ids = new List<int>();
            if (v.guid != null)
            {                
                lisPIDs = Factory.p85TempboxBL.GetPidsFromTemp(v.guid);
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
            var recP41 = this.Factory.p41ProjectBL.Load(rec.p41ID);

            var c = new GridRecord() { Datum = BO.BAS.ObjectDate2String(rec.p31Date), Popis = rec.p31Text,Jmeno=rec.Person,Projekt=rec.Project,Aktivita=rec.p32Name,p33ID=(int)rec.p33ID };
            c.pl = this.Factory.CurrentUser.getP07Level(recP41.p07Level,true);
            c.uroven = rec.p31ApprovingLevel;
            c.sazba = rec.p31Rate_Billing_Approved;
            c.bezdph = rec.p31Amount_WithoutVat_Approved;
            if (this.Factory.CurrentUser.j03DefaultHoursFormat == "T" || rec.IsRecommendedHHMM_Approved())
            {
                c.hodiny = BO.basTime.ShowAsHHMM(rec.p31Hours_Approved_Billing.ToString());
            }
            else
            {
                c.hodiny = rec.p31Hours_Approved_Billing.ToString();
            }
            if (this.Factory.CurrentUser.j03DefaultHoursFormat == "T")
            {
                c.hodinypausal = BO.basTime.ShowAsHHMM(rec.p31Value_FixPrice.ToString());
            }
            else
            {
                c.hodinypausal = rec.p31Value_FixPrice.ToString();
            }

            return c;
        }
    }
}
