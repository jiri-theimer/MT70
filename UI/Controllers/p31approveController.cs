using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace UI.Controllers
{
    public class p31approveController : BaseController 
    {
        public IActionResult Index(string guid,string pids,string prefix)
        {
            if (string.IsNullOrEmpty(pids) && string.IsNullOrEmpty(guid))
            {
                return this.StopPage(true, "pids or guid missing.");
            }

            var lisPIDs = new List<int>();
            var p31ids = new List<int>();
            if (guid != null)
            {
                var lisTemp=Factory.p85TempboxBL.GetList(guid, true, prefix);
                lisPIDs = lisTemp.Select(p => p.p85DataPID).ToList();
            }
            else
            {
                lisPIDs = BO.BAS.ConvertString2ListInt(pids);
            }
            var mq = new BO.myQueryP31() { p31statequery = 3 }; //nevyúčtované
            
            switch (prefix)
            {
                case "p31":
                    p31ids = lisPIDs;
                    break;
                case "j02":
                case "p28":
                case "p56":
                case "p41":
                    foreach(int pid in lisPIDs)
                    {
                        BO.Reflexe.SetPropertyValue(mq, prefix = "id", pid);                        
                        p31ids.InsertRange(0,Factory.p31WorksheetBL.GetList(mq).Select(p => p.pid));
                    }
                    break;
                
            }

            mq = new BO.myQueryP31() { p31statequery = 3,pids=p31ids };
            var lisP31 = Factory.p31WorksheetBL.GetList(mq);

            return View();
        }
    }
}
