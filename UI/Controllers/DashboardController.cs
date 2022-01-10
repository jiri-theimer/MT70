using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;


namespace UI.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly BL.ThePeriodProvider _pp;
        private readonly BL.TheColumnsProvider _colsProvider;
        public DashboardController(BL.ThePeriodProvider pp, BL.TheColumnsProvider cp)
        {
            _pp = pp;
            _colsProvider = cp;
        }







        public IActionResult Index()
        {

            if (HttpContext.Request.Path.Value.Length <= 1)
            {
                //úvodní spuštění: otestovat nastavení domovské stránky
                if (!string.IsNullOrEmpty(Factory.CurrentUser.j03HomePageUrl))
                {

                    return Redirect(Factory.CurrentUser.j03HomePageUrl);  //pryč na jinou stránku
                }
            }

            string skin = Factory.CBL.LoadUserParam("Widgets-Skin", "index");
            return Widgets(skin);
        }

        public IActionResult Widgets(string skin)
        {
            if (string.IsNullOrEmpty(skin))
            {
                skin = "index";
            }
            var v = new WidgetsViewModel() { Skin = skin, IsSubform = false };
            if (v.Skin == "inspector" || v.Skin == "school")
            {
                v.IsSubform = true;
            }


            PrepareWidgets(v);

            return View(v);
        }



        public BO.Result ChangeSkin(string skin)
        {
            Factory.CBL.SetUserParam("Widgets-Skin", skin);
            return new BO.Result(false);
        }




        private string parse_badge(int intCount)
        {
            if (intCount > 0)
            {
                return intCount.ToString();
            }
            return null;
        }

        public BO.Result SaveWidgetState(string s, string skin)
        {
            var rec = Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, skin);
            rec.x56DockState = s;
            rec.x56Skin = skin;
            Factory.x55WidgetBL.SaveState(rec);
            return new BO.Result(false);
        }
        public BO.Result RemoveWidget(int x55id, string skin)
        {
            var recX55 = Factory.x55WidgetBL.Load(x55id);
            var recX56 = Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, skin);
            var boxes = BO.BAS.ConvertString2List(recX56.x56Boxes);
            if (boxes.Where(p => p == recX55.x55Code).Count() > 0)
            {
                boxes.Remove(recX55.x55Code);
                recX56.x56Boxes = string.Join(",", boxes);
                Factory.x55WidgetBL.SaveState(recX56);
                return new BO.Result(false);
            }

            return new BO.Result(true, "widget not found");
        }
        public BO.Result InsertWidget(int x55id, string skin)
        {
            var recX55 = Factory.x55WidgetBL.Load(x55id);
            var recX56 = Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, skin);
            var boxes = BO.BAS.ConvertString2List(recX56.x56Boxes);
            if (boxes.Where(p => p == recX55.x55Code).Count() == 0)
            {
                boxes.Add(recX55.x55Code);
                recX56.x56Boxes = string.Join(",", boxes);
                Factory.x55WidgetBL.SaveState(recX56);
                return new BO.Result(false);
            }
            return new BO.Result(true, "widget not found");
        }
        public BO.Result SavePocetSloupcu(int x, string skin)
        {
            Factory.CBL.SetUserParam("Widgets-ColumnsPerPage-" + skin, x.ToString());
            return new BO.Result(false);
        }
        public BO.Result SavePageAutoRefresh(int x,string skin)
        {
            Factory.CBL.SetUserParam("Widgets-PageAutoRefresh-" + skin, x.ToString());
            return new BO.Result(false);
        }

        public BO.Result Clear2FactoryState(string skin)    //vyčistí plochu do továrního nastavení
        {
            Factory.x55WidgetBL.Clear2FactoryState(Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, skin));
            return new BO.Result(false);
        }




        //rozvržení prostoru na ploše
        private void PrepareWidgets(WidgetsViewModel v)
        {
            var mq = new BO.myQuery("x55") { IsRecordValid = true, MyRecordsDisponible = true, CurrentUser = Factory.CurrentUser };
            var hodnoty = new List<string>() { null, v.Skin };
            if (v.Skin == "inspector" || v.Skin == "index")
            {
                hodnoty.Add("inspector_index");
            }
            v.lisAllWidgets = Factory.x55WidgetBL.GetList(mq).Where(p => hodnoty.Contains(p.x55Skin));



            v.lisUserWidgets = new List<BO.x55Widget>();
            v.ColumnsPerPage = Factory.CBL.LoadUserParamInt("Widgets-ColumnsPerPage-" + v.Skin, 2);
            v.PageAutoRefreshPerSeconds = Factory.CBL.LoadUserParamInt("Widgets-PageAutoRefresh-" + v.Skin, 0);
            v.recX56 = Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, v.Skin);
            v.DockStructure = new WidgetsEnvironment(v.recX56.x56DockState);

            if (Factory.CurrentUser.j03LangIndex == 2)
            {
                v.DataTables_Localisation = "/lib/datatables/localisation/uk_UA.json";
            }
            else
            {
                v.DataTables_Localisation = "/lib/datatables/localisation/cs_CZ.json";
            }

            if (v.recX56 == null || v.recX56.x56Boxes == null)
            {
                return; //uživatel nemá na ploše žádný widget, dál není třeba pokračovat
            }

            var boxes = BO.BAS.ConvertString2List(v.recX56.x56Boxes);
            foreach (string s in boxes)
            {
                if (v.lisAllWidgets.Where(p => p.x55Code == s).Count() > 0)
                {
                    v.lisUserWidgets.Add(v.lisAllWidgets.Where(p => p.x55Code == s).First());
                }
            }


            foreach (var onestate in v.DockStructure.States)
            {
                if (v.lisUserWidgets.Where(p => p.pid.ToString() == onestate.Value).Count() > 0)
                {
                    var c = v.lisUserWidgets.Where(p => p.pid.ToString() == onestate.Value).First();
                    switch (onestate.Key)
                    {
                        case "2":
                            if (v.ColumnsPerPage >= 2) v.DockStructure.Col2.Add(c);
                            break;
                        case "3":
                            if (v.ColumnsPerPage >= 3) v.DockStructure.Col3.Add(c);
                            break;
                        default:
                            v.DockStructure.Col1.Add(c);
                            break;
                    }
                }
            }
            foreach (var c in v.lisUserWidgets)
            {
                if ((v.DockStructure.Col1.Contains(c) || v.DockStructure.Col2.Contains(c) || v.DockStructure.Col3.Contains(c)) == false)
                {
                    switch (v.ColumnsPerPage)
                    {
                        case 2 when (v.DockStructure.Col1.Count() >= 2):
                            v.DockStructure.Col2.Add(c);
                            break;
                        case 3 when (v.DockStructure.Col1.Count() >= 2 && v.DockStructure.Col2.Count() >= 2):
                            v.DockStructure.Col3.Add(c);
                            break;
                        case 3 when (v.DockStructure.Col1.Count() >= 2 && v.DockStructure.Col2.Count() < 2):
                            v.DockStructure.Col2.Add(c);
                            break;
                        default:
                            v.DockStructure.Col1.Add(c);
                            break;
                    }

                }

            }
            if (v.lisUserWidgets.Exists(p => p.x55TableSql != null && p.x55TableColHeaders != null))
            {
                v.IsDataTables = true;
            }
            if (v.IsDataTables)
            {
                if (v.lisUserWidgets.Exists(p => p.x55DataTablesButtons > BO.x55DataTablesBtns.None))
                {
                    v.IsExportButtons = true;   //zobrazovat tlačítka XLS/CSV/COPY
                }
                if (v.lisUserWidgets.Exists(p => p.x55DataTablesButtons == BO.x55DataTablesBtns.ExportPrintPdf))
                {
                    v.IsPdfButtons = true;      //zobrazovat i tlačítko PDF
                }
                if (v.IsPdfButtons || v.lisUserWidgets.Exists(p => p.x55DataTablesButtons == BO.x55DataTablesBtns.ExportPrint))
                {
                    v.IsPrintButton = true;      //zobrazovat i tlačítko PDF
                }
            }
            if (v.lisUserWidgets.Exists(p => p.x55ChartSql != null && p.x55ChartHeaders != null))
            {
                v.IsCharts = true;
            }

            switch (v.ColumnsPerPage)
            {
                case 1:
                    v.BoxColCss = "col-12";
                    break;
                case 2:
                    v.BoxColCss = "col-lg-6";
                    break;
                case 3:
                    v.BoxColCss = "col-sm-6 col-lg-4";
                    break;
            }
        }


        //načtení html obsahu jednoho boxu
        public BO.x55Widget GetWidgetHtmlContent(int x55id, int columnsperpage)
        {
            var rec = Factory.x55WidgetBL.Load(x55id);
            string strHtml = rec.x55Content;

            if (rec.x55ChartSql != null && rec.x55ChartHeaders != null)
            {
                string s = rec.x55ChartSql;
                s = DL.BAS.ParseMergeSQL(s, Factory.CurrentUser.j02ID.ToString()).Replace("@j04id", Factory.CurrentUser.j04ID.ToString().Replace("@j03id", Factory.CurrentUser.pid.ToString()));
                var dt = Factory.gridBL.GetListFromPureSql(s);
                var cGen = new BO.CLS.Datatable2Chart();
                strHtml += cGen.CreateGoogleChartHtml(dt, rec.x55ChartType, rec.x55ChartHeaders);
            }
            if (rec.x55TableSql != null && rec.x55TableColHeaders != null)
            {
                string s = rec.x55TableSql;
                s = DL.BAS.ParseMergeSQL(s, Factory.CurrentUser.j02ID.ToString()).Replace("@j04id", Factory.CurrentUser.j04ID.ToString().Replace("@j03id", Factory.CurrentUser.pid.ToString()));
                var dt = Factory.gridBL.GetListFromPureSql(s);
                if (dt.Rows.Count >= rec.x55DataTablesLimit && rec.x55DataTablesLimit > 0)
                {
                    rec.IsUseDatatables = true; //splněna podmínka pro zobrazení tabulky přes plugin DataTables

                }
                var cGen = new BO.CLS.Datatable2Html(new BO.CLS.Datatable2HtmlDef() { ColHeaders = rec.x55TableColHeaders, ColTypes = rec.x55TableColTypes, ClientID = rec.x55Code, IsUseDatatables = rec.IsUseDatatables });
                strHtml += cGen.CreateHtmlTable(dt, 1000);

            }

            switch (rec.x55Code.ToLower())
            {
                case "pandulak":
                    var strPandulakDir = Factory.App.AppRootFolder + "\\wwwroot\\images\\pandulak";
                    strHtml = string.Format("<img src='/images/pandulak/{0}'/>", basUI.getPandulakImage(strPandulakDir));
                    if (columnsperpage <= 2)
                    {
                        strHtml += string.Format("<img src='/images/pandulak/{0}'/>", basUI.getPandulakImage(strPandulakDir));
                    }
                    break;
            }
            rec.x55Content = strHtml;
            return rec;



        }
    }
}