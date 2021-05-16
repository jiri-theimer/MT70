using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.p31view;

namespace UI.Controllers
{
    public class p31daylineController : BaseController
    {        
        public IActionResult Index(string d)
        {
            var v = new daylineViewModel() { d0 = DateTime.Today, GroupBy = daylineGroupBy.None };
            if (!string.IsNullOrEmpty(d))
            {
                try
                {
                    v.d0 = BO.BAS.String2Date(d);
                }
                catch
                {
                    v.d0 = DateTime.Today;
                }
            }

            var mqJ02 = new BO.myQueryJ02() { j02isintraperson = true, explicit_orderby = "a.j02LastName,a.j02FirstName" };

            dayline_handle_defaults(v, mqJ02);

            v.lisJ02 = Factory.j02PersonBL.GetList(mqJ02);

            v.lisSums = Factory.p31WorksheetBL.GetList_TimelineDays(v.lisJ02.Select(p => p.pid).ToList(), v.d1, v.d2, 0).ToList();
            var mq = new BO.myQueryP31() { global_d1 = v.d1, global_d2 = v.d2 };            
            v.lisC26 = Factory.c26HolidayBL.GetList(new BO.myQueryC26() { global_d1 = v.d1,global_d2 = v.d2 });

            if (v.GroupBy != daylineGroupBy.None)
            {
                v.lisP31 = Factory.p31WorksheetBL.GetList(mq);
            }
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
                    if (qry.Count() > 0)
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
                        if (rec.Hours_Billable > 0 && rec.Hours_NonBillable == 0)
                        {
                            rec.CssStyle = "color:green;";
                        }
                        else
                        {
                            if (rec.Hours_NonBillable > 0 && rec.Hours_Billable == 0) rec.CssStyle = "color:red;";
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



        //zobrazení ZOOM okna pro vybranou osobu a den
        public IActionResult Zoom(int j02id,string d,int m,int y,int p28id,int p41id,int p32id,bool? p32isbillable,int p70id,bool? iswip,bool? isapproved_and_wait4invoice,string d1,string d2)
        {
            var v = new daylineZoomViewModel() { j02ID=j02id,p28ID=p28id,p41ID=p41id,p32ID=p32id,p32IsBillable= p32isbillable,IsWip=iswip,p70ID=p70id, IsApproved_And_Wait4Invoice=isapproved_and_wait4invoice };
            if (m>0 && y > 0)
            {
                v.SelectedDate1 = new DateTime(y, m, 1);
                v.SelectedDate2 = v.SelectedDate1.AddMonths(1).AddDays(-1);
            }
            else
            {
                if (d1 !=null && d2 != null)
                {
                    v.SelectedDate1 = BO.BAS.String2Date(d1);
                    v.SelectedDate2 = BO.BAS.String2Date(d2);
                }
                else
                {
                    v.SelectedDate1 = BO.BAS.String2Date(d);
                    v.SelectedDate2 = v.SelectedDate1;
                }
                
            }

            if (v.j02ID==0 || v.SelectedDate1.Year<2000)
            {
                return this.StopPage(true, "Na vstupu chybí osoba nebo datum.");
            }
            v.RecJ02 = Factory.j02PersonBL.Load(v.j02ID);

            string strMyQueryInline = "j02id|int|" + v.j02ID.ToString()+ "|global_d1|date|" + BO.BAS.ObjectDate2String(v.SelectedDate1,"dd.MM.yyyy") + "|global_d2|date|" + BO.BAS.ObjectDate2String(v.SelectedDate2, "dd.MM.yyyy");
            if (v.p28ID > 0)
            {
                strMyQueryInline += "|p28id|int|" + v.p28ID.ToString();
            }
            if (v.p41ID > 0)
            {
                strMyQueryInline += "|p41id|int|" + v.p41ID.ToString();
            }
            if (v.p32ID > 0)
            {
                strMyQueryInline += "|p32id|int|" + v.p32ID.ToString();
            }
            if (v.p70ID > 0)
            {
                strMyQueryInline += "|p70id|int|" + v.p70ID.ToString();
            }
            if (v.p32IsBillable != null)
            {
                strMyQueryInline += "|p32isbillable|bool|" + BO.BAS.GB(Convert.ToBoolean(v.p32IsBillable));
            }
            if (v.IsWip != null)
            {
                strMyQueryInline += "|iswip|bool|" + BO.BAS.GB(Convert.ToBoolean(v.IsWip));
            }
            if (v.IsApproved_And_Wait4Invoice != null)
            {
                strMyQueryInline += "|isapproved_and_wait4invoice|bool|" + BO.BAS.GB(Convert.ToBoolean(v.IsApproved_And_Wait4Invoice));
            }

            v.gridinput = new TheGridInput() { entity = "p31Worksheet", master_entity = "inform", myqueryinline = strMyQueryInline,oncmclick= "local_cm(event)" }; //grid má vlastní zdroj kontextového menu
            v.gridinput.query = new BO.InitMyQuery().Load("p31", null, 0, strMyQueryInline);
            
            //v.gridinput.fixedcolumns = "p31Date,p31_j02__j02Person__fullname_desc,p31_p41__p41Project__p41Name,p31_p32__p32Activity__p32Name,p31Rate_Billing_Invoiced,p31Amount_WithoutVat_Invoiced,p31VatRate_Invoiced,p31Text";
            
            return View(v);
        }

    }
}
