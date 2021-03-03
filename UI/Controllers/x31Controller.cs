using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using System.IO.Packaging;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using UI.Models;
using UI.Models.Record;
using System.Data;

namespace UI.Controllers
{
    public class x31Controller : BaseController
    {
        private readonly BL.ThePeriodProvider _pp;
        public x31Controller(BL.ThePeriodProvider pp)
        {            
            _pp = pp;
        }
        public IActionResult ReportNoContextFramework(int x31id,string code)
        {
            var v = new ReportNoContextFrameworkViewModel();

            if (!Factory.CurrentUser.j04IsMenu_Report)
            {            
                return this.StopPage(false, "Nemáte oprávnění pro tuto stránku.");
            }
            v.lisX31 = Factory.x31ReportBL.GetList(new BO.myQuery("x31")).Where(p => p.x29ID == BO.x29IdEnum._NotSpecified);
            var qry = v.lisX31.Select(p => new { p.j25ID, p.j25Name, p.j25Ordinary }).Distinct().OrderBy(p => p.j25Ordinary);
            return View(v);

        }
        public IActionResult ReportNoContext(int x31id,string code)
        {
            var v = new ReportNoContextViewModel();
            v.LangIndex = Factory.CurrentUser.j03LangIndex;

            if (x31id == 0 && !string.IsNullOrEmpty(code))
            {
                x31id = Factory.x31ReportBL.LoadByCode(code, 0).pid;
            }
            v.SelectedX31ID = x31id;
            RefreshStateReportNoContext(v);


            return View(v);
            
        }
      
        [HttpPost]
        public IActionResult ReportNoContext(ReportNoContextViewModel v, string oper)
        {
            
            RefreshStateReportNoContext(v);

            
            return View(v);
        }
        private void RefreshStateReportNoContext(ReportNoContextViewModel v)
        {
            if (v.SelectedX31ID > 0)
            {
                v.RecX31 = Factory.x31ReportBL.Load(v.SelectedX31ID);
                v.SelectedReport = v.RecX31.x31Name;

                var recO27 = Factory.x31ReportBL.LoadReportDoc(v.SelectedX31ID);
                               
                if (recO27 !=null)
                {
                    v.ReportFileName = recO27.o27ArchiveFileName;
                    if (!System.IO.File.Exists(Factory.x35GlobalParamBL.ReportFolder() + "\\" + v.ReportFileName))
                    {
                        v.ReportFileName = recO27.o27OriginalFileName;
                    }
                    if (System.IO.File.Exists(Factory.x35GlobalParamBL.ReportFolder() + "\\" + v.ReportFileName))
                    {
                        var xmlReportSource = new Telerik.Reporting.XmlReportSource();
                        var strXmlContent = System.IO.File.ReadAllText(Factory.x35GlobalParamBL.ReportFolder() + "\\" + v.ReportFileName);
                        if (strXmlContent.Contains("datFrom") && strXmlContent.Contains("datUntil"))
                        {
                            v.IsPeriodFilter = true;
                            v.PeriodFilter = new PeriodViewModel();
                            v.PeriodFilter.IsShowButtonRefresh = true;
                            var per = InhalePeriodFilter();
                            v.PeriodFilter.PeriodValue = per.pid;
                            v.PeriodFilter.d1 = per.d1;
                            v.PeriodFilter.d2 = per.d2;
                        }
                        else
                        {
                            v.IsPeriodFilter = false;
                        }
                        if (strXmlContent.Contains("1=1"))
                        {
                            v.lisJ72 = Factory.j72TheGridTemplateBL.GetList("a01Event", Factory.CurrentUser.pid, null).Where(p => p.j72HashJ73Query == true);
                            foreach(var c in v.lisJ72.Where(p => p.j72IsSystem == true))
                            {
                                c.j72Name = Factory.tra("Výchozí GRID");
                            }
                        }
                    }
                    
                }
                else
                {
                    this.AddMessage("Na serveru nelze dohledat soubor šablony zvolené tiskové sestavy.");                    
                }
            }

        }

        
        public IActionResult ReportContext(int pid, string prefix,int x31id,string x31pid)
        {
            var v = new ReportContextViewModel() { rec_pid = pid, rec_prefix = prefix };
            v.LangIndex = Factory.CurrentUser.j03LangIndex;
            if (string.IsNullOrEmpty(v.rec_prefix)==true || v.rec_pid == 0)
            {
                return StopPage(true, "pid or prefix missing");
            }
            if (x31id == 0 && string.IsNullOrEmpty(x31pid))
            {
                v.UserParamKey = "ReportContext-" + prefix + "-x31id";
                x31id = Factory.CBL.LoadUserParamInt(v.UserParamKey);

            }
            if (x31id==0 && !string.IsNullOrEmpty(x31pid))
            {
                x31id = Factory.x31ReportBL.LoadByCode(x31pid, 0).pid;
            }
            v.SelectedX31ID = x31id;
            
                        
            RefreshStateReportContext(v);
            return View(v);
        }
        [HttpPost]
        public IActionResult ReportContext(ReportContextViewModel v,string oper)
        {
            RefreshStateReportContext(v);

            if (oper == "change_x31id" && v.SelectedX31ID>0)
            {
                Factory.CBL.SetUserParam(v.UserParamKey, v.SelectedX31ID.ToString());
                v.GeneratedTempFileName = "";
            }
            if (oper== "generate_docx")
            {
                v.AllGeneratedTempFileNames=Handle_DocMailMerge(v);
                
                if (v.AllGeneratedTempFileNames !=null)
                {
                    v.GeneratedTempFileName = v.AllGeneratedTempFileNames[0];
                    this.AddMessage("Dokument byl vygenerován.", "info");
                }
            }
            
            return View(v);
        }

        private void RefreshStateReportContext(ReportContextViewModel v)
        {
            v.x29ID = Factory.EProvider.ByPrefix(v.rec_prefix).x29ID;
            if (v.SelectedX31ID > 0)
            {
                v.RecX31 = Factory.x31ReportBL.Load(v.SelectedX31ID);
                v.SelectedReport = v.RecX31.x31Name;

                var recO27 = Factory.x31ReportBL.LoadReportDoc(v.SelectedX31ID);
               
                if (recO27 !=null)
                {
                    v.ReportFileName = recO27.o27ArchiveFileName;
                    if (!System.IO.File.Exists(Factory.x35GlobalParamBL.ReportFolder()+"\\"+ v.ReportFileName))
                    {
                        v.ReportFileName = recO27.o27OriginalFileName;
                    }
                }
                else
                {
                    this.AddMessage("Na serveru nelze dohledat soubor šablony zvolené tiskové sestavy.");
                }
            }            

        }

        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x31Record() { rec_pid = pid, rec_entity = "x31",UploadGuid=BO.BAS.GetGuid() };
            v.Rec = new BO.x31Report();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x31ReportBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboJ25Name = v.Rec.j25Name;

               

            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            RefreshStateRecord(v);
            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(x31Record v,string oper)
        {
            RefreshStateRecord(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.x31Report c = new BO.x31Report();
                if (v.rec_pid > 0) c = Factory.x31ReportBL.Load(v.rec_pid);
                c.x31FormatFlag = v.Rec.x31FormatFlag;
                c.x29ID = v.Rec.x29ID;
                c.x31Name = v.Rec.x31Name;
                c.x31Code = v.Rec.x31Code;
                c.x31Description = v.Rec.x31Description;
                c.x31IsPeriodRequired = v.Rec.x31IsPeriodRequired;

                c.x31ExportFileNameMask = v.Rec.x31ExportFileNameMask;
                
                c.x31DocSqlSource = v.Rec.x31DocSqlSource;
                c.x31LangIndex = v.Rec.x31LangIndex;

                c.x31IsScheduling = v.Rec.x31IsScheduling;
                c.x31IsRunInDay1 = v.Rec.x31IsRunInDay1;
                c.x31IsRunInDay2 = v.Rec.x31IsRunInDay2;
                c.x31IsRunInDay3 = v.Rec.x31IsRunInDay3;
                c.x31IsRunInDay4 = v.Rec.x31IsRunInDay4;
                c.x31IsRunInDay5 = v.Rec.x31IsRunInDay5;
                c.x31IsRunInDay6 = v.Rec.x31IsRunInDay6;
                c.x31IsRunInDay7 = v.Rec.x31IsRunInDay7;


                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
               
                c.pid = Factory.x31ReportBL.Save(c);
                if (c.pid > 0)
                {
                    if (Factory.o27AttachmentBL.SaveSingleUpload(v.UploadGuid, 931, c.pid))
                    {
                        v.SetJavascript_CallOnLoad(c.pid);
                        return View(v);
                    }

                    
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshStateRecord(x31Record v)
        {
            if (v.rec_pid > 0)
            {
                v.RecO27 = Factory.x31ReportBL.LoadReportDoc(v.rec_pid);               
            }
            
        }


        private BO.ThePeriod InhalePeriodFilter()
        {
            var ret = _pp.ByPid(0);
            int x = Factory.CBL.LoadUserParamInt("report-period-value");
            if (x > 0)
            {
                ret = _pp.ByPid(x);
            }
            else
            {
                ret.d1 = Factory.CBL.LoadUserParamDate("report-period-d1");
                ret.d2 = Factory.CBL.LoadUserParamDate("report-period-d2");

            }
            
            return ret;
        }

        private void handle_merge_value(Text item,DataTable dt, DataRow dr)
        {
            string strVal = "";            

            foreach (DataColumn col in dt.Columns)
            {

                if (item.Text.Contains("«" + col.ColumnName + "»", StringComparison.OrdinalIgnoreCase) || item.Text.Contains("<" + col.ColumnName + ">", StringComparison.OrdinalIgnoreCase))
                {
                    if (dr[col] == null)
                    {
                        strVal = "";
                    }
                    else
                    {
                        switch (col.DataType.Name.ToString())
                        {
                            case "DateTime":
                                strVal = BO.BAS.ObjectDate2String(dr[col]);
                                break;
                            case "Decimal":
                            case "Double":
                                strVal = BO.BAS.Number2String(Convert.ToDouble(dr[col]));
                                break;
                            default:
                                strVal = dr[col].ToString();
                                break;
                        }
                        
                    }
                    item.Text = item.Text.Replace("<" + col.ColumnName + ">", strVal, StringComparison.OrdinalIgnoreCase).Replace("«" + col.ColumnName + "»", strVal, StringComparison.OrdinalIgnoreCase);

                }
            }
        }
        private List<string> Handle_DocMailMerge(ReportContextViewModel v)
        {
            var recO27 = Factory.x31ReportBL.LoadReportDoc(v.RecX31.pid);
            if (recO27 == null)
            {
                this.AddMessage("Na serveru nelze dohledat soubor šablony zvolené tiskové sestavy.");return null;
            }
            DataTable dt = null;
            if (string.IsNullOrEmpty(v.RecX31.x31DocSqlSource))
            {
                dt = Factory.gridBL.GetList4MailMerge(v.rec_prefix, v.rec_pid);
            }
            else
            {
                dt = Factory.gridBL.GetList4MailMerge(v.rec_pid,v.RecX31.x31DocSqlSource);  //sql zdroj na míru
            }
            
            if (dt.Rows.Count == 0)
            {
                this.AddMessage("Na vstupu chybí záznam.");return null;
            }

            var strFileName = BO.BAS.GetGuid();
            DataRow dr0 = dt.Rows[0];
            switch (v.rec_prefix)
            {
                case "a01":
                    strFileName = dr0["a01Signature"] + "_" + BO.BAS.GetGuid();
                    break;
                case "a03":
                    strFileName = dr0["a03REDIZO"] + "_" + BO.BAS.GetGuid();
                    break;
                case "j02":
                    strFileName = dr0["j02LastName"] + "_" + dr0["j02FirstName"] + "_" + BO.BAS.GetGuid();
                    break;
            }

            var filenames = new List<string>();
            int x = 0;
            foreach(DataRow dr in dt.Rows)
            {
                x += 1;
                filenames.Add(strFileName+"_"+x.ToString()+".docx");                
                GenerateOneDoc(recO27, dt, dr, filenames.Last());
            }

            if (dt.Rows.Count > 1)  //join dokumentů, pokud má zdroj více záznamů
            {                
                System.IO.File.Copy(Factory.x35GlobalParamBL.TempFolder() + "\\" + filenames[0], Factory.x35GlobalParamBL.TempFolder() + "\\" + strFileName+".docx", true);   //výsledek pro záznam
                
                for (int xx = 1;xx < filenames.Count;xx++)  //join druhého a dalšího záznamu do dokumentu prvního záznamu
                {
                    using (WordprocessingDocument myDoc = WordprocessingDocument.Open(Factory.x35GlobalParamBL.TempFolder() + "\\" + strFileName + ".docx", true))
                    {
                        MainDocumentPart mainPart = myDoc.MainDocumentPart;
                        
                        string altChunkId = "AltChunkId" + xx.ToString();
                        AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
                            AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                        using (FileStream fileStream = System.IO.File.Open(Factory.x35GlobalParamBL.TempFolder() + "\\" + filenames[xx], FileMode.Open))
                        {
                            chunk.FeedData(fileStream);
                        }
                        Paragraph pb1 = new Paragraph(new Run((new Break() { Type = BreakValues.Page })));
                        mainPart.Document.Body.InsertAfter(pb1, mainPart.Document.Body.LastChild);

                        AltChunk altChunk = new AltChunk();
                        altChunk.Id = altChunkId;
                        mainPart.Document.Body.InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());
                        
                        mainPart.Document.Save();
                        myDoc.Close();
                    }
                }
                filenames.Insert(0, strFileName + ".docx");
            }

            return filenames;
        }

        private void GenerateOneDoc(BO.o27Attachment recO27, DataTable dt, DataRow dr,string strTempFileName)
        {
            string strTempPath = Factory.x35GlobalParamBL.TempFolder() + "\\" + strTempFileName;
            System.IO.File.Copy(Factory.x35GlobalParamBL.ReportFolder()+"\\" + recO27.o27ArchiveFileName, strTempPath, true);
            Package wordPackage = Package.Open(strTempPath, FileMode.Open, FileAccess.ReadWrite);

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(wordPackage))
            {
                var body = wordDocument.MainDocumentPart.Document.Body;
                var allParas = wordDocument.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>();

                foreach (var item in allParas)
                {
                    handle_merge_value(item, dt, dr);


                }
                foreach (HeaderPart headerPart in wordDocument.MainDocumentPart.HeaderParts)
                {
                    Header header = headerPart.Header;
                    var allHeaderParas = header.Descendants<Text>();
                    foreach (var item in allHeaderParas)
                    {
                        handle_merge_value(item, dt, dr);

                    }

                }

                foreach (FooterPart footerPart in wordDocument.MainDocumentPart.FooterParts)
                {
                    Footer footer = footerPart.Footer;
                    var allFooterParas = footer.Descendants<Text>();
                    foreach (var item in allFooterParas)
                    {
                        handle_merge_value(item, dt, dr);

                    }

                }


                wordDocument.MainDocumentPart.Document.Save();

            }
        }

    }
}