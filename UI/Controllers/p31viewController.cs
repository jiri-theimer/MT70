using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.p31view;

namespace UI.Controllers
{
    public class p31viewController : BaseController
    {
        //DAYLINE rozhraní
        public IActionResult dayline(string d)
        {            
            var v = new daylineViewModel() { d0 = DateTime.Today };
            if (!string.IsNullOrEmpty(d))
            {
                v.d0 = BO.BAS.String2Date(d);
            }
            v.d1 = new DateTime(v.d0.Year, v.d0.Month, 1);
            v.d2 = v.d1.AddMonths(1).AddDays(-1);

            dayline_handle_defaults(v);



            var mqJ02 = new BO.myQueryJ02() { j02isintraperson = true,explicit_orderby="a.j02LastName,a.j02FirstName" };
            v.lisJ02 = Factory.j02PersonBL.GetList(mqJ02);

            v.lisSums = Factory.p31WorksheetBL.GetList_TimelineDays(getJ02IDs(v), v.d1, v.d2,0);
            var mq = new BO.myQueryP31() { global_d1 = v.d1, global_d2 = v.d2 };           
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq);

            return View(v);
        }

        private void dayline_handle_defaults(daylineViewModel v)
        {
            v.j02IDs = Factory.CBL.LoadUserParam("dayline-j02ids");
            if (v.j02IDs != null)
            {
                var lis = Factory.j02PersonBL.GetList(new BO.myQueryJ02() { pids = BO.BAS.ConvertString2ListInt(v.j02IDs) });
                v.SelectedPersons = string.Join(",", lis.Select(p => p.FullNameDesc));
            }
            v.j07IDs = Factory.CBL.LoadUserParam("dayline-j07ids");
            if (v.j07IDs != null)
            {
                var lis = Factory.j07PersonPositionBL.GetList(new BO.myQuery("j07") { pids = BO.BAS.ConvertString2ListInt(v.j07IDs) });
                v.SelectedPositions = string.Join(",", lis.Select(p => p.j07Name));
            }
            v.j11IDs = Factory.CBL.LoadUserParam("dayline-j11ids");
            if (v.j11IDs != null)
            {
                var lis = Factory.j11TeamBL.GetList(new BO.myQueryJ11() { pids = BO.BAS.ConvertString2ListInt(v.j11IDs) });
                v.SelectedTeams = string.Join(",", lis.Select(p => p.j11Name));
            }
        }


        public List<int> getJ02IDs(daylineViewModel v)
        {
            return null;
        }
    }
}
