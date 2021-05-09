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
            var v = new daylineViewModel() { d0 = DateTime.Today, GroupBy = daylineGroupBy.None };
            if (!string.IsNullOrEmpty(d))
            {
                v.d0 = BO.BAS.String2Date(d);
            }

            var mqJ02 = new BO.myQueryJ02() { j02isintraperson = true, explicit_orderby = "a.j02LastName,a.j02FirstName" };

            dayline_handle_defaults(v, mqJ02);

            v.lisJ02 = Factory.j02PersonBL.GetList(mqJ02);

            v.lisSums = Factory.p31WorksheetBL.GetList_TimelineDays(v.lisJ02.Select(p=>p.pid).ToList(), v.d1, v.d2, 0).ToList();
            var mq = new BO.myQueryP31() { global_d1 = v.d1, global_d2 = v.d2 };
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq);

            dayline_format_hours(v);

            return View(v);
        }

        private void dayline_format_hours(daylineViewModel v)
        {
            v.ShowHHMM = false;
            if (Factory.CurrentUser.j03DefaultHoursFormat == "T") v.ShowHHMM = true;


            foreach (var recJ02 in v.lisJ02)
            {
                for (DateTime d = v.d1; d <= v.d2; d = d.AddDays(1))
                {
                    var qry = v.lisSums.Where(p => p.j02ID == recJ02.pid && p.p31Date == d);
                    if (qry.Count()>0)
                    {
                        var rec = qry.First();
                       if (v.ShowHHMM)
                        {
                            rec.HoursFormatted = BO.basTime.ShowAsHHMM(rec.Hours.ToString());
                        }
                        else
                        {
                            rec.HoursFormatted = BO.BAS.Number2String(rec.Hours);
                        }
                        
                    }
                }
                    
            }
        }
        private void dayline_handle_defaults(daylineViewModel v, BO.myQueryJ02 mqJ02)
        {
            v.d1 = new DateTime(v.d0.Year, v.d0.Month, 1);
            v.d2 = v.d1.AddMonths(1).AddDays(-1);

            string strGroupBy = Factory.CBL.LoadUserParam("dayline-groupby", "None");
            v.GroupBy = (daylineGroupBy)Enum.Parse(typeof(daylineGroupBy), strGroupBy);

            v.j02IDs = Factory.CBL.LoadUserParam("dayline-j02ids");
            if (v.j02IDs != null)
            {
                var j02ids = BO.BAS.ConvertString2ListInt(v.j02IDs);
                var lis = Factory.j02PersonBL.GetList(new BO.myQueryJ02() { pids = j02ids });
                v.SelectedPersons = string.Join(",", lis.Select(p => p.FullNameDesc));
                mqJ02.pids = BO.BAS.ConvertString2ListInt(v.j02IDs);
            }
            v.j07IDs = Factory.CBL.LoadUserParam("dayline-j07ids");
            if (v.j07IDs != null)
            {
                var j07ids = BO.BAS.ConvertString2ListInt(v.j07IDs);
                var lis = Factory.j07PersonPositionBL.GetList(new BO.myQuery("j07") { pids = j07ids });
                v.SelectedPositions = string.Join(",", lis.Select(p => p.j07Name));
                mqJ02.j07ids = j07ids;
            }
            v.j11IDs = Factory.CBL.LoadUserParam("dayline-j11ids");
            if (v.j11IDs != null)
            {
                var j11ids = BO.BAS.ConvertString2ListInt(v.j11IDs);
                var lis = Factory.j11TeamBL.GetList(new BO.myQueryJ11() { pids = j11ids });
                v.SelectedTeams = string.Join(",", lis.Select(p => p.j11Name));
                mqJ02.j11ids = j11ids;
            }
        }


       
    }
}
