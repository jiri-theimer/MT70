using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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

            var lisP31 = GetRecords(guid, pids, prefix);
            SetupTempData(lisP31,guid, p72id,p91id, approvinglevel);

            return View();
        }

        private void SetupTempData(IEnumerable<BO.p31Worksheet> lisP31,string guid,int p72id,int p91id, int approvinglevel)
        {
            int intLastP41ID = 0;int intP41ID = 0;

            foreach (var rec in lisP31)
            {
                intP41ID = rec.p41ID;
                var c = new BO.p31WorksheetApproveInput() { p31ID = rec.pid, p33ID = rec.p33ID, Guid = guid,p31Date=rec.p31Date, p31ApprovingLevel = rec.p31ApprovingLevel };
                if (rec.p71ID == BO.p71IdENUM.Nic)
                {
                    //dosud neprošlo schvalováním
                    c.Rate_Internal_Approved = rec.p31Rate_Internal_Orig;
                    c.p31ApprovingLevel = approvinglevel;

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
                if (p72id>0 && p72id !=4 && (rec.p33ID==BO.p33IdENUM.Cas || rec.p33ID == BO.p33IdENUM.Kusovnik))
                {
                    c.p72id = (BO.p72IdENUM)p72id;
                    c.Value_Approved_Billing = 0;
                }

                intLastP41ID = intP41ID;
            }
        }

        private IEnumerable<BO.p31Worksheet> GetRecords(string guid, string pids, string prefix)
        {
            var lisPIDs = new List<int>();
            var p31ids = new List<int>();
            if (guid != null)
            {
                var lisTemp = Factory.p85TempboxBL.GetList(guid, true, prefix);
                lisPIDs = lisTemp.Select(p => p.p85DataPID).ToList();
            }
            else
            {
                lisPIDs = BO.BAS.ConvertString2ListInt(pids);
            }
            var mq = new BO.myQueryP31() { MyRecordsDisponible = true, p31statequery = 3 }; //nevyúčtované            
            switch (prefix)
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
                        BO.Reflexe.SetPropertyValue(mq, prefix = "id", pid);
                        p31ids.InsertRange(0, Factory.p31WorksheetBL.GetList(mq).Select(p => p.pid));
                    }
                    break;

            }

            mq = new BO.myQueryP31() { MyRecordsDisponible = true, p31statequery = 3, pids = p31ids,explicit_orderby = "a.p41ID,a.p31ID"};

            return Factory.p31WorksheetBL.GetList(mq);
        }
    }
}
