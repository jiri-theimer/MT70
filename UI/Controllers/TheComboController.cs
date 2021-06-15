using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class TheComboController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;

        public TheComboController(BL.TheColumnsProvider cp)
        {
            _colsProvider = cp;
        }

        //public mysearchSetting LoadMySearchSetting()
        //{
        //    var ret = new mysearchSetting();
        //    ret.TopRecs = Factory.CBL.LoadUserParamInt("mysearch_toprecs", 50);
        //    ret.ClosedRecsClass = Factory.CBL.LoadUserParam("mysearch_closedrecs", "isclosed");

        //    return ret;
        //}

        public string GetHtml4TheCombo(string entity, string tableid, string myqueryinline, string pids, string filterflag, string searchstring, string masterprefix, int masterpid) //Vrací HTML zdroj tabulky pro MyCombo
        {
            var mq = new BO.InitMyQuery().Load(entity,masterprefix,masterpid,myqueryinline);
            mq.SetPids(pids);
            
            var ce = Factory.EProvider.ByPrefix(mq.Prefix);
            if (ce.IsWithoutValidity == false)
            {
                mq.IsRecordValid = true;    //v combo nabídce pouze časově platné záznamy
            }
            

            if (filterflag != "0" && filterflag != "")
            {
                mq.SearchString = searchstring; //filtrování na straně serveru
                mq.TopRecordsOnly = 50; //maximálně prvních 50 záznamů, které vyhovují podmínce
            }
            List<BO.TheGridColumn> cols = null;            
            string strJ72Columns = getComboPalleteFixed(entity,mq); //vrací sloupce pro významnější entity
            if (strJ72Columns != null)
            {
                cols = _colsProvider.ParseTheGridColumns(mq.PrefixDb, strJ72Columns, Factory);
            }
            else
            {
                cols = _colsProvider.getDefaultPallete(true, mq,Factory);   //výchozí paleta sloupců
            }
            


            //switch (mq.Prefix)
            //{
            //    case "a03":
            //        if (masterprefix == "a06" && (masterpid == 2 || masterpid==3 || masterpid==4))      //combo zřizovatele nebo dohledového orgánu-> vyndat redizo sloupec
            //        {
            //            if (cols.Where(p => p.Field == "a03REDIZO").Count() > 0)
            //            {
            //                cols.Remove(cols.Where(p => p.Field == "a03REDIZO").First());
            //                bolZrizovatel = true;
            //            }
            //        }
            //        break;
            //    case "a01":
            //        //mq.a01IsTemporary = false;  //vyloučit temp akce
            //        break;                
            //}
            mq.explicit_columns = cols;

            mq.explicit_orderby = ce.SqlOrderByCombo;

            

            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();

            if (mq.TopRecordsOnly > 0)
            {
                if (intRows >= mq.TopRecordsOnly)
                {
                    s.AppendLine(string.Format("<small style='margin-left:10px;'>{0} {1} {2}. {3}</small>",Factory.tra("Zobrazeno prvních"), intRows,Factory.tra("záznamů"),Factory.tra("Zpřesněte filtrovací podmínku.")));
                }
                else
                {
                    s.AppendLine(string.Format("<small style='margin-left:10px;'>{0}: {1}.</small>",Factory.tra("Počet záznamů"), intRows));
                }

            }

            s.Append(string.Format("<table id='{0}' class='table table-thecombo'>", tableid));

            s.Append("<thead><tr style='font-weight:normal;'>");
            foreach (var col in cols)
            {
                switch (Factory.CurrentUser.j03LangIndex)
                {
                    case 1:
                        s.Append(string.Format("<th>{0}</th>", col.TranslateLang1));
                        break;
                    case 2:
                        s.Append(string.Format("<th>{0}</th>", col.TranslateLang2));
                        break;
                    default:
                        s.Append(string.Format("<th>{0}</th>", col.Header));
                        break;
                }
                
            }
            s.Append(string.Format("</tr></thead><tbody id='{0}_tbody'>", tableid));
            string strTrClass = "";
            for (int i = 0; i < intRows; i++)
            {
                strTrClass = "txz";
                if (Convert.ToBoolean(dt.Rows[i]["isclosed"]) == true)
                {

                    strTrClass += " isclosed";
                }
                if (mq.Prefix == "p32")
                {
                    if (Convert.ToBoolean(dt.Rows[i]["p32IsBillable"]))
                    {
                        strTrClass += " fa";
                    }
                    else
                    {
                        strTrClass += " nefa";
                    }                    
                }
                s.Append(string.Format("<tr class='{0}' data-v='{1}'", strTrClass, dt.Rows[i]["pid"]));
                
                

                s.Append(">");
                foreach (var col in cols)
                {
                    if (col.NormalizedTypeName == "num")
                    {
                        s.Append(string.Format("<td style='text-align:right;'>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
                    }
                    else
                    {
                        s.Append(string.Format("<td>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
                    }


                }
                s.Append("</tr>");
            }
            s.Append("</tbody></table>");

            return s.ToString();
        }



        //zdroj checkboxů pro taghelper mycombochecklist:
        public string GetHtml4Checkboxlist(string controlid, string entity, string selectedvalues, string masterprefix, int masterpid, string myqueryinline) //Vrací HTML seznam checkboxů pro taghelper: mycombochecklist
        {            
            var mq = new BO.InitMyQuery().Load(entity,masterprefix,masterpid,myqueryinline);
            
            mq.explicit_columns = _colsProvider.getDefaultPallete(false, mq,Factory);
            var ce = Factory.EProvider.ByPrefix(mq.Prefix);
            if (ce.IsWithoutValidity == false)
            {
                mq.IsRecordValid = true;    //v combo nabídce pouze časově platné záznamy
            }
            mq.explicit_orderby = ce.SqlOrderByCombo;

           
            List<int> selpids = null;
            if (String.IsNullOrEmpty(selectedvalues) == false)
            {
                selpids = BO.BAS.ConvertString2ListInt(selectedvalues);
            }

            string strTextField = "a__" + entity + "__" + mq.Prefix + "Name";
            switch (mq.Prefix)
            {
                case "j02":
                    strTextField = "a__j02Person__fullname_desc";
                    break;
                case "j27":
                    strTextField = "a__j27Currency__j27code";
                    break;
            }
            
            string strGroupField = null;
            string strLastGroup = null;
            string strGroup = null;
            string strChecked = "";
            int intValue = 0;
            string strText = "";
            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var sb = new System.Text.StringBuilder();
            //sb.AppendLine("<div class='row' style='padding:0px;margin:0px;'>");
            //sb.AppendLine("<div class='col-8'>");
            sb.AppendLine("<ul style='list-style:none;padding-left:0px;'>");


            for (int i = 0; i < intRows; i++)
            {
                intValue = Convert.ToInt32(dt.Rows[i]["pid"]);
                strText = Convert.ToString(dt.Rows[i][strTextField]);
                strChecked = "";
                if (strGroupField != null)
                {
                    if (dt.Rows[i][strGroupField] == null)
                    {
                        strGroup = null;
                    }
                    else
                    {
                        strGroup = Convert.ToString(dt.Rows[i][strGroupField]);
                    }
                    if (strGroup != strLastGroup)
                    {
                        sb.AppendLine("<li>");
                        sb.AppendLine("<div style='font-weight:bold;background-color:#ADD8E6;'><span style='padding-left:10px;'>" + strGroup + "</span></div>");
                        sb.AppendLine("</li>");
                    }

                }
                if (selpids != null && selpids.Where(p => p == intValue).Count() > 0)
                {
                    strChecked = "checked";
                }

                sb.AppendLine("<li>");
                sb.Append(string.Format("<input type='checkbox' id='chk{0}_{1}' name='chk{0}' value='{1}' {2} />", controlid, intValue, strChecked));
                sb.Append(string.Format("<label style='min-width:60%;' for='chk{0}_{1}'>{2}</label>", controlid, intValue, strText));

                sb.AppendLine("</li>");
            }



            sb.AppendLine("</ul>");
            //sb.AppendLine("</div>");
            //sb.AppendLine("<div class='col-4'>");
            sb.AppendLine(string.Format("<button type='button' id='cmdCheckAll{0}' class='btn btn-light'>"+Factory.tra("Zaškrtnout vše")+"</button>", controlid));
            sb.AppendLine(string.Format("<button type='button' id='cmdUnCheckAll{0}' class='btn btn-light'>"+Factory.tra("Odškrtnout vše")+"</button>", controlid));
            //sb.AppendLine("</div>");
            //sb.AppendLine("</div>");
            return sb.ToString();
        }

        //public string GetHtml4SearchSetting(string controlid, string entity)
        //{
        //    var setting = LoadMySearchSetting();
        //    var sb = new System.Text.StringBuilder();

        //    sb.AppendLine("<div style='padding:20px;'>");
        //    sb.Append("<h5>"+Factory.tra("FULLTEXT nastavení")+"</h5>");
        //    sb.Append("<hr>");
        //    if (entity.Substring(0, 3) == "a01")
        //    {
        //        sb.Append(Factory.tra("U hledání akcí systém pracuje s poli:")+"<b>"+"[Signatura], [Kód spisu], [Jméno vedoucího akce], [Jméno člena akce], [Název školy], [REDIZO]"+"</b>.");
        //    }
        //    if (entity.Substring(0, 3) == "a03")
        //    {
        //        sb.Append(Factory.tra("U hledání institucí systém pracuje s poli:")+ "<b>"+Factory.tra("[Název školy], [REDIZO],[IZO činnosti školy] [IČ], [Obec], [Ulice]")+"</b>.");
        //    }
        //    if (entity.Substring(0, 3) == "j02")
        //    {
        //        sb.Append(Factory.tra("U hledání osoby systém pracuje s poli:")+" <b>"+Factory.tra("[Jméno], [Příjmení], [E-mail], [Osobní číslo]")+"</b>.");
        //    }
        //    sb.Append("<br>");
        //    sb.Append("<span class='text-danger'>"+Factory.tra("Fulltext hledání funguje na principu shody celých slov!")+"</span>");
        //    sb.Append("<br><span class='text-success'>"+Factory.tra("Mezera mezi slovy je operátor 'and', můžete psát mezi slovy i přímo 'or'.")+"</span>");
        //    sb.Append("<br><span class='text-info'>"+Factory.tra("Narozdíl od GRID filtrování, kde se pracuje s částečnou shodou (LIKE).")+"</span>");
        //    sb.Append("<hr>");
        //    sb.Append("<label>"+Factory.tra("Zobrazovat maximálně")+" </label><select id='mysearch_toprecs'>");
        //    sb.Append(UI.bas.render_select_option("20", "20", setting.TopRecs.ToString()));
        //    sb.Append(UI.bas.render_select_option("50", "50", setting.TopRecs.ToString()));
        //    sb.Append(UI.bas.render_select_option("100", "100", setting.TopRecs.ToString()));
        //    sb.Append("</select> "+Factory.tra("záznamů")+".<br>");

        //    sb.Append("<label>"+Factory.tra("Uzavřené záznamy odlišovat:")+" </label><select id='mysearch_closedrecs'>");
        //    sb.Append(basUI.render_select_option("isclosed", Factory.tra("Přeškrtlé"), setting.ClosedRecsClass));
        //    sb.Append(basUI.render_select_option("isclosed_by_color", Factory.tra("Barvou"), setting.ClosedRecsClass));
        //    sb.Append("</select>.");
        //    sb.AppendLine("<button type='button' class='btn btn-primary' onclick='mysearch_save_setting()'>"+Factory.tra("Uložit nastavení")+"</button>");


        //    sb.AppendLine("</div>");
        //    return sb.ToString();
        //}


        //public string GetHtml4Search(string entity, string searchstring) //Vrací HTML zdroj tabulky pro MySearch
        //{
        //    var setting = LoadMySearchSetting();
        //    var mq = new BO.InitMyQuery().Load(entity);
        //    //mq.SearchImplementation = Factory.App.Implementation;
        //    //mq.IsRecordValid = true;    //v combo nabídce pouze časově platné záznamy


        //    mq.SearchString = searchstring; //filtrování na straně serveru
        //    mq.TopRecordsOnly = setting.TopRecs; //maximálně prvních 50 záznamů, které vyhovují podmínce
        //    //mq.a01IsTemporary = false;  //vyloučit temp akce
        //    mq.MyRecordsDisponible = true;
        //    mq.CurrentUser = Factory.CurrentUser;

        //    var cols = _colsProvider.getDefaultPallete(true, mq);
        //    mq.explicit_columns = cols;

        //    mq.explicit_orderby = Factory.EProvider.ByPrefix(mq.Prefix).SqlOrderByCombo;

        //    var dt = Factory.gridBL.GetList(mq);
        //    var intRows = dt.Rows.Count;

        //    var s = new System.Text.StringBuilder();

        //    if (searchstring == null || searchstring.Length <= 3)
        //    {
        //        s.AppendLine("<small style='margin-left:10px;'>"+Factory.tra("Musíte zadat minimálně 4 znaky.")+"</small>");
        //        return s.ToString();
        //    }

        //    if (mq.TopRecordsOnly > 0)
        //    {
        //        if (intRows >= mq.TopRecordsOnly)
        //        {
        //            s.AppendLine(string.Format("<small style='margin-left:10px;'>"+Factory.tra("Zobrazeno prvních {0} záznamů. Zpřesněte filtrovací podmínku.")+"</small>", intRows));
        //        }
        //        else
        //        {
        //            s.AppendLine(string.Format("<small style='margin-left:10px;'>"+Factory.tra("Počet záznamů: {0}.")+"</small>", intRows));
        //        }

        //    }

        //    s.Append(string.Format("<table id='{0}' class='table table-hover'>", "hovado"));

        //    s.Append("<thead><tr>");
        //    foreach (var col in cols)
        //    {

        //        switch (Factory.CurrentUser.j03LangIndex)
        //        {
        //            case 1:
        //                s.Append(string.Format("<th>{0}</th>", col.TranslateLang1));
        //                break;
        //            case 2:
        //                s.Append(string.Format("<th>{0}</th>", col.TranslateLang2));
        //                break;
        //            default:
        //                s.Append(string.Format("<th>{0}</th>", col.Header));
        //                break;
        //        }

        //    }
        //    s.Append(string.Format("</tr></thead><tbody id='{0}_tbody'>", "hovado"));
        //    string strTrClass = "";
        //    for (int i = 0; i < intRows; i++)
        //    {
        //        strTrClass = "txs";
        //        if (Convert.ToBoolean(dt.Rows[i]["isclosed"]) == true)
        //        {
        //            strTrClass += " " + setting.ClosedRecsClass;
        //        }

        //        s.Append(string.Format("<tr class='{0}' data-v='{1}'", strTrClass, dt.Rows[i]["pid"]));


        //        s.Append(">");
        //        foreach (var col in cols)
        //        {
        //            if (col.NormalizedTypeName == "num")
        //            {
        //                s.Append(string.Format("<td style='text-align:right;'>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
        //            }
        //            else
        //            {
        //                s.Append(string.Format("<td>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
        //            }


        //        }
        //        s.Append("</tr>");
        //    }
        //    s.Append("</tbody></table>");

        //    return s.ToString();
        //}
        
        public string GetAutoCompleteHtmlItems(int o15flag, string tableid) //Vrací options pro datalist v rámci autocomplete pole
        {
            var mq = new BO.myQuery("o15");
            var lis = Factory.o15AutoCompleteBL.GetList(mq).Where(p => (int)p.o15Flag == o15flag);
            return string.Join("|", lis.Select(p => p.o15Value));
            
        }

        public string GetMySelectHtmlOptions(string entity,string textfield,string orderfield)
        {
            var sb = new System.Text.StringBuilder();
            var mq = new BO.myQuery(entity) { IsRecordValid = true };
            textfield = System.Web.HttpUtility.UrlDecode(textfield).Replace("##", "'");
            mq.explicit_selectsql =textfield + " AS combotext";
            if (!string.IsNullOrEmpty(orderfield))
            {               
                orderfield = System.Web.HttpUtility.UrlDecode(orderfield).Replace("##", "'");
                mq.explicit_orderby = orderfield;
            }
            
            var dt = Factory.gridBL.GetList(mq);
            foreach(DataRow dbRow in dt.Rows)
            {
                
                sb.Append(string.Format("<option value='{0}'>{1}</option>", dbRow["pid"].ToString(), dbRow["combotext"]));
            }
            

            return sb.ToString();
        }



        private string getComboPalleteFixed(string entity, BO.baseQuery mq)  //vrací paletu sloupců pro COMBO: pouze pro významné entity
        {
            string s = null;
            switch (mq.Prefix)
            {
                case "j02":
                    s = "a__j02Person__fullname_desc,a__j02Person__j02Email,j02_j07__j07PersonPosition__j07Name";
                    break;
                case "p31":
                    s = "a__p31Worksheet__p31Date,p31_j02__j02Person__fullname_desc,p31_p41_p28__p28Contact__p28Name,p31_p41__p41Project__p41Name,p31_p32__p32Activity__p32Name,a__p31Worksheet__p31Hours_Orig,a__p31Worksheet__p31Amount_WithoutVat_Orig,a__p31Worksheet__p31Text";
                    break;
                case "p28":
                    s = "a__p28Contact__p28Name,a__p28Contact__p28RegID,p28_address_primary__view_PrimaryAddress__FullAddress";
                    break;
                case "le1":
                case "le2":
                case "le3":
                case "le4":
                case "le5":                
                    s = "p41_p28client__p28Contact__p28Name,a__p41Project__p41Name,a__p41Project__p41Code,p41_p42__p42ProjectType__p42Name";
                    break;
                case "p41":
                    s = "p41_p28client__p28Contact__p28Name,a__p41Project__p41Name,p41_p42__p42ProjectType__p42Name,a__p41Project__AllFreeTags_p41";
                    break;
                case "p56":
                    s = "p56_b02__b02WorkflowStatus__b02Name,a__p56Task__p56Name,p56_p28__p28Contact__p28Name,p56_p41__p41Project__p41Name,a__p56Task__p56PlanUntil";
                    break;
                case "p90":
                    s = "a__p90Proforma__p90Code,a__p90Proforma__p90Date,p90_p28__p28Contact__p28Name,a__p90Proforma__p90Amount,p90_j27__j27Currency__j27Code,a__p90Proforma__p90Amount_Billed,a__p90Proforma__p90DateMaturity,a__p90Proforma__p91codes,a__p90Proforma__ChybiSparovat,a__p90Proforma__p90Text1";
                    break;
                case "p91":
                    s = "a__p91Invoice__p91Code,a__p91Invoice__p91Client,a__p91Invoice__p91DateSupply,a__p91Invoice__p91Amount_WithoutVat,p91_j27__j27Currency__j27Code,a__p91Invoice__p91Amount_Debt,a__p91Invoice__p91DateMaturity,a__p91Invoice__VomKdyOdeslano";
                    break;
                case "o23":
                    s = "a__o23Doc__o23Name,a__o23Doc__DocType,o23_p28__p28Contact__p28Name,o23_p41__p41Project__p41Name,a__o23Doc__AllFreeTags_o23,a__o23Doc__DateInsert_o23Doc,a__o23Doc__UserInsert_o23Doc";
                    break;
                case "b07":
                    s = "a__b07Comment__b07Date,a__b07Comment__b07Value,b07_p28__p28Contact__p28Name,b07_p41__p41Project__p41Name,a__b07Comment__b07LinkUrl,a__b07Comment__DateInsert_b07Comment,a__b07Comment__UserInsert_b07Comment";
                    break;
                default:
                    return null;
            }

            if (s == null)
            {
                return s;
            }


            List<string> lis = new List<string>();
            var arr = s.Split(",");
            for (int i = 0; i < arr.Count(); i++)   //pokud v definici sloupce chybí určení entity, pak doplnit:
            {
                if (arr[i].Contains("__"))
                {
                    lis.Add(arr[i]);
                }
                else
                {
                    lis.Add("a__" + entity + "__" + arr[i]);
                }
            }



            s = string.Join(",", lis);

            return s;

        }

    }
}