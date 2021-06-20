using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using UI.Models;

namespace UI.Controllers
{
    public class TheGridReportController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;

        public TheGridReportController(BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _colsProvider = cp;
            _pp = pp;
        }

        public IActionResult ReportViewer(string guid)
        {
            var v = new TheGridReportViewModel() { guid = guid };
            v.TrdxRepDestFileName = v.guid + ".trdx";
            if (!System.IO.File.Exists(Factory.x35GlobalParamBL.ReportFolder() + "\\" + v.TrdxRepDestFileName))
            {
                return this.StopPage(true, "Nelze najít soubor: " + v.TrdxRepDestFileName);
                
                
            }

            return View(v);

        }

        public IActionResult Index(int j72id, string pids, string master_prefix, int master_pid,string guid)
        {
            var v = new TheGridReportViewModel() { j72id = j72id, master_prefix = master_prefix, master_pid = master_pid,guid=guid };
            if (string.IsNullOrEmpty(v.guid))
            {
                v.guid = Factory.CurrentUser.j03Login + "_temp_gridreport";
            }
            InhaleDefaults(v);

            var gridState = Factory.j72TheGridTemplateBL.LoadState(j72id, Factory.CurrentUser.pid);

            v.prefix = gridState.j72Entity.Substring(0, 3);

            if (!string.IsNullOrEmpty(pids))
            {
                v.pids = BO.BAS.ConvertString2ListInt(pids);
            }

            v.TrdxRepDestFileName = v.guid + ".trdx";

            v.TrdxRepSourceFileName = FindRepFile(v);
            if (string.IsNullOrEmpty(v.TrdxRepSourceFileName))
            {
                this.AddMessage("V systému nelze najít template rep soubor.");
                return View(v);
            }
            if (!System.IO.File.Exists(Factory.App.WwwRootFolder + "\\templates\\" + v.TrdxRepSourceFileName))
            {
                this.AddMessageTranslated(string.Format("V systému chybí template rep soubor {0}.", Factory.App.WwwRootFolder + "\\templates\\" + v.TrdxRepSourceFileName));
                return View(v);
            }

            
            var mq = InhaleMyQuery(v, gridState, pids);

            var dt = Factory.gridBL.GetList(mq, false);

            
            string strSQL = Factory.gridBL.GetLastFinalSql();

            double dblDPI = 96; double dblWidthComplete_CM = 0; int intColIndex = 0;
            double dblRatio = v.ZoomPercentage / 100.00;

            string strXmlContent = System.IO.File.ReadAllText(Factory.App.WwwRootFolder + "\\templates\\" + v.TrdxRepSourceFileName);            

            var blocks = new List<BO.StringPair>();

            foreach (var col in mq.explicit_columns)
            {
                
                if ((col.Prefix=="p41" || col.Prefix.Substring(0,1)=="l") && col.Header.Substring(0,1) == "L" && col.Header.Length==2)
                {
                    col.Header = Factory.CurrentUser.getP07Level(Convert.ToInt32(col.Header.Substring(1,1)), true);
                }
                double dblWidth_CM = 3.00;
                string strBlock = GenerateTrdx_getTableCell(col, intColIndex, dblWidth_CM, v);

                
                blocks.Add(new BO.StringPair() { Key = "TableCell", Value = strBlock });

                strBlock = $"<Column Width='{dblWidth_CM.ToString()}cm' />";
                blocks.Add(new BO.StringPair() { Key = "Column", Value = strBlock });

                strBlock = GenerateTrdx_getColumnGroup(col, intColIndex, dblWidth_CM, v);


                blocks.Add(new BO.StringPair() { Key = "TableGroup", Value = strBlock });
                

                dblWidthComplete_CM += dblWidth_CM;
                intColIndex += 1;
            }

            

            string strFind = GenerateTrdx_FindBlock(ref strXmlContent, "<Cells>", "</Cells>");
            string strReplace = "<Cells>" + string.Join("", blocks.Where(p => p.Key == "TableCell").Select(p => p.Value)).Replace("'", "\"") + "</Cells>";
            GenerateTrdx_ParseResult(ref strXmlContent, strFind, strReplace);

            strFind = GenerateTrdx_FindBlock(ref strXmlContent, "<Columns>", "</Columns>");
            strReplace = "<Columns>" + string.Join("", blocks.Where(p => p.Key == "Column").Select(p => p.Value)).Replace("'", "\"") + "</Columns>";
            GenerateTrdx_ParseResult(ref strXmlContent, strFind, strReplace);

            strFind = GenerateTrdx_FindBlock(ref strXmlContent, "<ColumnGroups>", "</ColumnGroups>");
            strReplace = "<ColumnGroups>" + string.Join("", blocks.Where(p => p.Key == "TableGroup").Select(p => p.Value)).Replace("'", "\"") + "</ColumnGroups>";
            GenerateTrdx_ParseResult(ref strXmlContent, strFind, strReplace);

            strFind = ("<MarginsU Left='10mm' Right='5mm' Top='5mm' Bottom='5mm' />").Replace("'", "\"");
            strReplace=$"<MarginsU Left='{v.MarginLeft}mm' Right='{v.MarginRight}mm' Top='{v.MarginTop}mm' Bottom='{v.MarginBottom}mm' />";
            GenerateTrdx_ParseResult(ref strXmlContent, strFind, strReplace);

            GenerateTrdx_ParseResult(ref strXmlContent, "#sql#", HtmlEncoder.Default.Encode(strSQL)); //pro telerik report je třeba sql zahešovat

            

            if (v.PageBreakColumn != null)
            {
                var cols = _colsProvider.ParseTheGridColumns(v.prefix, v.PageBreakColumn, Factory);
                var colPageBy = cols[0];
                GenerateTrdx_ParseResult(ref strXmlContent, "#sql_pageby#", HtmlEncoder.Default.Encode(CompletePageBreakBySql(strSQL, colPageBy)));
                strFind = "=Fields.page_record_name";
                strReplace = colPageBy.Header + ": {Fields.page_record_name}";
                GenerateTrdx_ParseResult(ref strXmlContent, strFind, strReplace);
            }
            if (!string.IsNullOrEmpty(v.Header))
            {
                v.Header = v.Header.Replace("&", "#");
            }
            
            GenerateTrdx_ParseResult(ref strXmlContent, "#header#", v.Header);

            if (v.GroupByColumn != null)
            {
                var cols = _colsProvider.ParseTheGridColumns(v.prefix, v.GroupByColumn, Factory);
                var colGroupBy = cols[0];
                strReplace = colGroupBy.UniqueName;

                strXmlContent = strXmlContent.Replace("Fields.groupby_field_select", "Fields." + strReplace);
                strXmlContent = strXmlContent.Replace("Fields.groupby_field_groupby", "Fields." + strReplace);
                strXmlContent = strXmlContent.Replace("Fields.groupby_field_orderby", "Fields." + strReplace);
                strXmlContent = strXmlContent.Replace("groupby_field_alias", colGroupBy.Header);
            }

            

            System.IO.File.WriteAllText(Factory.x35GlobalParamBL.ReportFolder() + "\\" + v.TrdxRepDestFileName, strXmlContent);

            //TheGridInput input = new TheGridInput() { j72id = v.j72id, entity = gridState.j72Entity };

            //var cSup = new UI.TheGridSupport(input, Factory, _colsProvider);

            return View(v);
        }

        private string GenerateTrdx_FindBlock(ref string strContent, string strStartElement, string strEndElement)
        {
            int x = strContent.IndexOf(strStartElement);
            int y = strContent.IndexOf(strEndElement, x + 1);
            if (x == -1 || y == -1)
            {
                return "???##not-merged: " + strStartElement;
            }
            return strContent.Substring(x, strEndElement.Length + y - x);
        }
        private string GenerateTrdx_getColumnGroup(BO.TheGridColumn col, int intColIndex,double dblWidth_CM, TheGridReportViewModel v)
        {
            var s = new System.Text.StringBuilder();
            string strW = dblWidth_CM.ToString() + "cm";string strBgColor = "242, 242, 242";
            if (v.GroupByColumn != null)
            {
                strBgColor = "217, 217, 217";
            }
            s.AppendLine($"<TableGroup Name='{col.UniqueName + "_group" + intColIndex.ToString()}'>");
            s.AppendLine("<ReportItem>");
            s.AppendLine($"<TextBox Width='{strW}' Height='0.5cm' Left='0cm' Top='0cm' Value='{col.Header}' Name='textBox_header{intColIndex}' StyleName='Office.TableHeader'>");
            s.AppendLine($"<Style BackgroundColor='{strBgColor}' TextAlign='Center'>");
            s.AppendLine("<Font Size='8pt' Bold='True' />");
            s.AppendLine("</Style>");
            s.AppendLine("</TextBox>");
            s.AppendLine("</ReportItem>");
            s.AppendLine("</TableGroup>");

            return s.ToString();
        }

        private void GenerateTrdx_ParseResult(ref string strContent,string strFind,string strReplace)
        {
            //strContent = strContent.Replace(strFind, strReplace,StringComparison.OrdinalIgnoreCase);
            strContent = strContent.Replace(strFind, strReplace);
        }
        private string GenerateTrdx_getTableCell(BO.TheGridColumn col, int intColIndex, double dblWidth_CM, TheGridReportViewModel v)
        {
            var s = new System.Text.StringBuilder();
            string strW = dblWidth_CM.ToString() + "cm"; string strValue = "Fields." + col.UniqueName; string strFormat = null; string strAlign = "Left";
            switch (col.FieldType)
            {
                case "num":
                    strFormat = "Format='{0:N2}'";
                    strAlign = "Right";
                    break;
                case "num0":
                    strFormat = "Format='{0:N0}'";
                    strAlign = "Right";
                    break;
                case "date":
                    strFormat = "Format='{0:d}'";
                    break;
            }

            s.AppendLine($"<TableCell RowIndex='0' ColumnIndex='{intColIndex}' RowSpan='1' ColumnSpan='1'>");
            s.AppendLine("<ReportItem>");
            s.AppendLine($"<TextBox Width='{strW}' Height='0.5cm' Left='0cm' Top='0cm' Value='= {strValue}' {strFormat} Name='textBox_col{intColIndex}' StyleName='Office.TableBody'>");
            s.AppendLine($"<Style TextAlign='{strAlign}' VerticalAlign='Middle'>");
            s.AppendLine("<Font Size='8pt' />");
            s.AppendLine("<Padding Left='0.1cm' Right='0.1cm' Top='0.1cm' />");
            s.AppendLine("</Style>");
            s.AppendLine("</TextBox>");
            s.AppendLine("</ReportItem>");
            s.AppendLine("</TableCell>");

            if (col.IsShowTotals)
            {
                strValue = "Value='=sum(Fields." + col.UniqueName + ")'";
            }
            else
            {
                strValue = "";
            }

            s.AppendLine($"<TableCell RowIndex='1' ColumnIndex='{intColIndex}' RowSpan='1' ColumnSpan='1'>");
            s.AppendLine("<ReportItem>");
            s.AppendLine($"<TextBox Width='{strW}' Height='0.5cm' Left='0cm' Top='0cm' {strValue} {strFormat} Name='textBox_sum_col{intColIndex}' StyleName='Office.TableBody'>");
            s.AppendLine($"<Style BackgroundColor='242, 242, 242' TextAlign='{strAlign}' VerticalAlign='Middle'>");
            s.AppendLine("<Font Size='8pt' Bold='True' />");
            s.AppendLine("<Padding Left='0.1cm' Right='0.1cm' Top='0.1cm' />");
            s.AppendLine("</Style>");
            s.AppendLine("</TextBox>");
            s.AppendLine("</ReportItem>");
            s.AppendLine("</TableCell>");

            if (v.GroupByColumn != null)
            {
                s.AppendLine($"<TableCell RowIndex='2' ColumnIndex='{intColIndex}' RowSpan='1' ColumnSpan='1'>");
                s.AppendLine("<ReportItem>");
                s.AppendLine($"<TextBox Width='{strW}' Height='0.5cm' Left='0cm' Top='0cm' {strValue} {strFormat} Name='textBox_groupsum_col{intColIndex}' StyleName='Office.TableBody'>");
                s.AppendLine($"<Style BackgroundColor='217, 217, 217' TextAlign='{strAlign}' VerticalAlign='Middle'>");
                s.AppendLine("<Font Size='8pt' Bold='True' />");
                s.AppendLine("<Padding Left='0.1cm' Right='0.1cm' Top='0.1cm' />");
                s.AppendLine("</Style>");
                s.AppendLine("</TextBox>");
                s.AppendLine("</ReportItem>");
                s.AppendLine("</TableCell>");
            }

            return s.ToString();
        }

        private void InhaleDefaults(TheGridReportViewModel v)
        {
            v.ZoomPercentage = Factory.CBL.LoadUserParamInt("thegridreport-zoom", 70);
            v.MaxTopRecs = Factory.CBL.LoadUserParamInt("thegridreport-toprecs", 2000);
            v.PageOrientation = Factory.CBL.LoadUserParamInt("thegridreport-orientation", 1);
            v.MarginLeft = Factory.CBL.LoadUserParamInt("thegridreport-marginleft", 10);
            v.MarginRight = Factory.CBL.LoadUserParamInt("thegridreport-marginright", 0);
            v.MarginTop = Factory.CBL.LoadUserParamInt("thegridreport-margintop", 0);
            v.MarginBottom = Factory.CBL.LoadUserParamInt("thegridreport-marginbottom", 0);
        }

        private BO.baseQuery InhaleMyQuery(TheGridReportViewModel v, BO.TheGridState gridState, string pids)
        {
            string myqueryinline = null;
            if (string.IsNullOrEmpty(v.master_prefix) && (v.prefix == "p31" || v.prefix == "j02"))
            {
                var tab = Factory.CBL.LoadUserParam("overgrid-tab-" + v.prefix, "zero", 1);
                if (tab != null && tab != "zero")
                {
                    myqueryinline = "tabquery|string|" + tab;
                }
            }

            var mq = new BO.InitMyQuery().Load(gridState.j72Entity, v.master_prefix, v.master_pid, myqueryinline);
            mq.TopRecordsOnly = v.MaxTopRecs;
            mq.IsRecordValid = null;    //brát v potaz i archivované záznamy
            mq.SetPids(pids);

            mq.explicit_columns = _colsProvider.ParseTheGridColumns(v.prefix, gridState.j72Columns, Factory);

            if (string.IsNullOrEmpty(v.master_prefix))
            {
                mq.p31statequery = Factory.CBL.LoadUserParamInt("grid-" + v.prefix + "-p31statequery", 0);
            }
            else
            {
                mq.p31statequery = Factory.CBL.LoadUserParamInt("grid-" + v.prefix + "-" + BO.BASX29.GetEntity(v.master_prefix) + "-p31statequery", 0);
            }

            var p1 = new PeriodViewModel() { prefix = v.prefix, IsShowButtonRefresh = true };
            if (string.IsNullOrEmpty(v.master_prefix))
            {
                p1.UserParamKey = $"grid-period-{v.prefix}";
            }
            else
            {
                p1.UserParamKey = $"grid-period-{v.prefix}-{BO.BASX29.GetEntity(v.master_prefix)}";
            }
            p1.InhaleUserPeriodSetting(_pp, Factory);
            mq.period_field = p1.PeriodField;
            mq.global_d1 = p1.d1;
            mq.global_d2 = p1.d2;

            return mq;
        }

        private string FindRepFile(TheGridReportViewModel v)
        {
            string s = "thereport_export_template_portrait.trdx";
            if (!string.IsNullOrEmpty(v.GroupByColumn))
            {
                s = "thereport_export_template_portrait_pageby.trdx";
            }
            if (v.IsLandScape)
            {
                s = "thereport_export_template_landscape.trdx";
                if (v.GroupByColumn != null)
                {
                    s = "thereport_export_template_landscape_pageby.trdx";
                }
            }
            return s;
        }

        
        private string CompletePageBreakBySql(string strSQL, BO.TheGridColumn col)
        {
            
            if (col.SqlSyntaxGroupBy == null) return null;

            int x = strSQL.IndexOf(" FROM ");
            strSQL = BO.BAS.RightString(strSQL, strSQL.Length - x);
            int y = strSQL.IndexOf(" ORDER BY ");
            if (y > 0)
            {
                strSQL = strSQL.Substring(0, y);                    
            }

            string s = "SELECT DISTINCT";
            if (col.SqlSyntax == null)
            {
                s += " " + col.SqlSyntaxGroupBy + " AS page_record_pid," + col.UniqueName + " AS page_record_name";
            }
            else
            {
                s += " " + col.SqlSyntaxGroupBy + " AS page_record_pid," + col.SqlSyntax + " AS page_record_name";
            }
            s += strSQL;

            if (col.SqlSyntax == null)
            {
                s += " ORDER BY " + col.UniqueName;
            }
            else
            {
                s += " ORDER BY " + col.SqlSyntax;
            }

            return s;
        }
    }
}
