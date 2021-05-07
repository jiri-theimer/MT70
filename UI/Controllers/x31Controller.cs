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
using Winnovative.PDFMerge;

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
            if (x31id==0 && !string.IsNullOrEmpty(code))
            {
                x31id = Factory.x31ReportBL.LoadByCode(code,0).pid;
            }
            var v = new ReportNoContextFrameworkViewModel();

            if (!Factory.CurrentUser.j04IsMenu_Report)
            {            
                return this.StopPage(false, "Nemáte oprávnění pro tuto stránku.");
            }
            if (x31id == 0)
            {
                x31id = Factory.CBL.LoadUserParamInt("x31/last-reportnocontext-x31id");
            }
            if (x31id > 0)
            {
                v.SelectedReport = Factory.x31ReportBL.Load(x31id);
            }
            v.lisX31 = Factory.x31ReportBL.GetList(new BO.myQuery("x31")).Where(p => p.x29ID == BO.x29IdEnum._NotSpecified);
            var qry = v.lisX31.Select(p => new { p.j25ID, p.j25Name, p.j25Ordinary }).Distinct();
            v.lisJ25 = new List<BO.j25ReportCategory>();
            foreach (var c in qry)
            {
                var cc = new BO.j25ReportCategory() { pid = c.j25ID, j25Name = c.j25Name, j25Ordinary = c.j25Ordinary,j25Code= "accordion-button collapsed py-2" };
                if (cc.j25Name == null)
                {
                    cc.j25Ordinary = -999999;
                    cc.j25Name = "** "+Factory.tra("Bez kategorie");                    
                }
                if (v.SelectedReport != null && c.j25ID == v.SelectedReport.j25ID)
                {
                    cc.j25Code = "accordion-button py-2";
                }
                cc.j25Name += " (" + v.lisX31.Where(p => p.j25ID == cc.pid).Count().ToString() + ")";
                v.lisJ25.Add(cc);
            }
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

            if (x31id > 0)
            {
                int intLastSaved = Factory.CBL.LoadUserParamInt("x31/last-reportnocontext-x31id");
                if (intLastSaved != x31id)
                {
                    Factory.CBL.SetUserParam("x31/last-reportnocontext-x31id", x31id.ToString());
                }
            }

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
                if (!string.IsNullOrEmpty(v.ReportExportName))
                {
                    var cc = new TheReportSupport();
                    v.ReportExportName = cc.GetReportExportName(Factory, 0, v.RecX31);
                }
                

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
                        
                        if (v.RecX31.x31IsPeriodRequired || (strXmlContent.Contains("datFrom", StringComparison.OrdinalIgnoreCase) && strXmlContent.Contains("datUntil", StringComparison.OrdinalIgnoreCase)))      
                        {
                            v.IsPeriodFilter = true;
                            v.PeriodFilter = new PeriodViewModel();
                            v.PeriodFilter.IsShowButtonRefresh = true;
                            var per =Factory.x31ReportBL.InhalePeriodFilter(_pp);
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

        
        public IActionResult ReportContext(int pid, string prefix,int x31id,string x31code)
        {
            var v = new ReportContextViewModel() { rec_pid = pid, rec_prefix = prefix };
            v.LangIndex = Factory.CurrentUser.j03LangIndex;
            if (string.IsNullOrEmpty(v.rec_prefix) || v.rec_pid == 0)
            {
                return StopPage(true, "pid or prefix missing");
            }
            if (x31id == 0 && string.IsNullOrEmpty(x31code))
            {
                v.UserParamKey = "ReportContext-" + prefix + "-x31id";
                x31id = Factory.CBL.LoadUserParamInt(v.UserParamKey);

            }
            if (x31id==0 && !string.IsNullOrEmpty(x31code))
            {
                x31id = Factory.x31ReportBL.LoadByCode(x31code, 0).pid;
            }
            if (x31id==0 && prefix == "p91")
            {
                var recP91 = Factory.p91InvoiceBL.Load(pid);
                x31id = Factory.p92InvoiceTypeBL.Load(recP91.p92ID).x31ID_Invoice;
            }
            v.SelectedX31ID = x31id;
            try
            {
                v.MergedX31ID_1 = Factory.CBL.LoadUserParamInt("ReportContext-mergedx31id-1-"+v.rec_prefix);
                if (v.MergedX31ID_1 > 0) v.MergedX31Name_1 = Factory.x31ReportBL.Load(v.MergedX31ID_1).x31Name;
                v.MergedX31ID_2 = Factory.CBL.LoadUserParamInt("ReportContext-mergedx31id-2-"+v.rec_prefix);
                if (v.MergedX31ID_2 > 0) v.MergedX31Name_2 = Factory.x31ReportBL.Load(v.MergedX31ID_2).x31Name;
                v.MergedX31ID_3 = Factory.CBL.LoadUserParamInt("ReportContext-mergedx31id-3-"+v.rec_prefix);
                if (v.MergedX31ID_3 > 0) v.MergedX31Name_3 = Factory.x31ReportBL.Load(v.MergedX31ID_3).x31Name;
            }
            catch(Exception ex)
            {
                this.AddMessageTranslated(ex.Message);
            }
            

            RefreshStateReportContext(v);
            if (v.RecX31 !=null && v.RecX31.x31FormatFlag == BO.x31FormatFlagENUM.DOCX)
            {
                ReportContext_GenerateDOCX(v); //automaticky vygenerovat DOCX
            }
            return View(v);
        }
        [HttpPost]
        public IActionResult ReportContext(ReportContextViewModel v,string oper)
        {
            RefreshStateReportContext(v);

            if (oper == "change_x31id" && v.SelectedX31ID>0)
            {
                var cc = new TheReportSupport();
                v.ReportExportName = cc.GetReportExportName(Factory, v.rec_pid, v.RecX31);
                Factory.CBL.SetUserParam(v.UserParamKey, v.SelectedX31ID.ToString());
                v.GeneratedTempFileName = "";
            }
            if (oper== "generate_docx")
            {
                ReportContext_GenerateDOCX(v);
            }
            if (oper == "merge")    //pdf merge
            {
               v.FinalMergedPdfFileName = MergeReports(v,BO.BAS.GetGuid());
            }
            if (oper == "mail") //odeslat sestavu poštou
            {
                var cc = new TheReportSupport();
                string strUploadGuid = BO.BAS.GetGuid();
                cc.GeneratePdfReport(Factory, _pp, v.RecX31, strUploadGuid, v.rec_pid);
                return RedirectToAction("SendMail", "Mail", new { uploadguid = strUploadGuid, recpid = v.rec_pid, x29id = v.x29ID });
            }
            if (oper== "merge_and_mail")
            {
                string strUploadGuid = BO.BAS.GetGuid();                
                if (MergeReports(v, strUploadGuid) != null)
                {
                    return RedirectToAction("SendMail", "Mail", new { uploadguid = strUploadGuid, recpid=v.rec_pid,x29id=v.x29ID });
                }
            }
            
            return View(v);
        }

        private void ReportContext_GenerateDOCX(ReportContextViewModel v)
        {
            v.AllGeneratedTempFileNames = Handle_DocMailMerge(v);

            if (v.AllGeneratedTempFileNames != null)
            {
                v.GeneratedTempFileName = v.AllGeneratedTempFileNames[0];
                this.AddMessage("Dokument byl vygenerován.", "info");
            }
        }

        private void RefreshStateReportContext(ReportContextViewModel v)
        {
            v.x29ID = Factory.EProvider.ByPrefix(v.rec_prefix).x29ID;
            if (v.SelectedX31ID > 0)
            {
                v.RecX31 = Factory.x31ReportBL.Load(v.SelectedX31ID);
                v.SelectedReport = v.RecX31.x31Name;
                if (string.IsNullOrEmpty(v.ReportExportName))
                {
                    var cc = new TheReportSupport();
                    v.ReportExportName = cc.GetReportExportName(Factory, v.rec_pid, v.RecX31);
                }
                
                var recO27 = Factory.x31ReportBL.LoadReportDoc(v.SelectedX31ID);               
                if (recO27 !=null)
                {
                    v.ReportFileName = recO27.o27ArchiveFileName;
                    if (!System.IO.File.Exists(Factory.x35GlobalParamBL.ReportFolder()+"\\"+ v.ReportFileName))
                    {
                        v.ReportFileName = recO27.o27OriginalFileName;
                    }
                    if (System.IO.File.Exists(Factory.x35GlobalParamBL.ReportFolder() + "\\" + v.ReportFileName))
                    {
                        var xmlReportSource = new Telerik.Reporting.XmlReportSource();
                        var strXmlContent = System.IO.File.ReadAllText(Factory.x35GlobalParamBL.ReportFolder() + "\\" + v.ReportFileName);
                        if (v.RecX31.x31IsPeriodRequired || (strXmlContent.Contains("datFrom", StringComparison.OrdinalIgnoreCase) && strXmlContent.Contains("datUntil", StringComparison.OrdinalIgnoreCase)))
                        {
                            v.IsPeriodFilter = true;
                            v.PeriodFilter = new PeriodViewModel();
                            v.PeriodFilter.IsShowButtonRefresh = true;
                            var per = Factory.x31ReportBL.InhalePeriodFilter(_pp);
                            v.PeriodFilter.PeriodValue = per.pid;
                            v.PeriodFilter.d1 = per.d1;
                            v.PeriodFilter.d2 = per.d2;

                        }
                        else
                        {
                            v.IsPeriodFilter = false;
                        }                        
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
                c.x31RunInTime = v.Rec.x31RunInTime;
                c.x31SchedulingReceivers = v.Rec.x31SchedulingReceivers;
                c.x21ID_Scheduling = v.Rec.x21ID_Scheduling;

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
            v.lisPeriodSource = new BL.ThePeriodProvider().getPallete().Where(p => p.pid > 1).ToList();
        }


        //private BO.ThePeriod InhalePeriodFilter()
        //{
        //    var ret = _pp.ByPid(0);
        //    int x = Factory.CBL.LoadUserParamInt("report-period-value");
        //    switch (x)
        //    {
        //        case 0:
        //            ret.d1 = new DateTime(2000,1,1);
        //            ret.d2 = new DateTime(3000, 1, 1);
        //            return ret;
        //        case 1:
        //            ret = _pp.ByPid(x);
        //            ret.d1 = Factory.CBL.LoadUserParamDate("report-period-d1");                    
        //            ret.d2 = Factory.CBL.LoadUserParamDate("report-period-d2");
        //            if (ret.d1 == null) ret.d1 = new DateTime(2000, 1, 1);
        //            if (ret.d2 == null) ret.d2= new DateTime(3000, 1, 1);
        //            break;
        //        default:
        //            ret = _pp.ByPid(x);
        //            break;
        //    }
           
            
        //    return ret;
        //}

        private void handle_merge_value(Text item,DataTable dt, DataRow dr)
        {
            string strVal = "";            

            foreach (DataColumn col in dt.Columns)
            {

                if (item.Text.Contains("«" + col.ColumnName + "»", StringComparison.OrdinalIgnoreCase) || item.Text.Contains("<" + col.ColumnName + ">", StringComparison.OrdinalIgnoreCase))
                {
                    if (dr[col] == System.DBNull.Value || dr[col]==null)
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

        private string MergeReports(ReportContextViewModel v,string strUploadGuid)
        {
            if (v.SelectedX31ID == 0)
            {
                this.AddMessage("Chybí vybrat tiskovou sestavu.");return null;
            }
            if (v.MergedX31ID_1 == 0 && v.MergedX31ID_2==0 && v.MergedX31ID_3==0)
            {
                this.AddMessage("Musíte zvolit minimálně jednu slučovanou sestavu."); return null;
            }
            //PDF merge po rumunsku:
            PdfDocumentOptions pdfDocumentOptions = new PdfDocumentOptions();
            pdfDocumentOptions.PdfCompressionLevel = PDFCompressionLevel.Normal;
            pdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            PDFMerge pdfMerge = new PDFMerge(pdfDocumentOptions);
            
            var cc = new TheReportSupport();
            string s = null;
            if (v.SelectedX31ID > 0)
            {
                s = cc.GeneratePdfReport(Factory, _pp, Factory.x31ReportBL.Load(v.SelectedX31ID), BO.BAS.GetGuid(), v.rec_pid);
                pdfMerge.AppendPDFFile(s);                
            }
            if (v.MergedX31ID_1 > 0)
            {
                s = cc.GeneratePdfReport(Factory, _pp, Factory.x31ReportBL.Load(v.MergedX31ID_1), BO.BAS.GetGuid(), v.rec_pid);
                pdfMerge.AppendPDFFile(s);                
            }
            Factory.CBL.SetUserParam("ReportContext-mergedx31id-1-" + v.rec_prefix, v.MergedX31ID_1.ToString());

            if (v.MergedX31ID_2 > 0)
            {
                s = cc.GeneratePdfReport(Factory, _pp, Factory.x31ReportBL.Load(v.MergedX31ID_2), BO.BAS.GetGuid(), v.rec_pid);
                pdfMerge.AppendPDFFile(s);                
            }
            Factory.CBL.SetUserParam("ReportContext-mergedx31id-2-" + v.rec_prefix, v.MergedX31ID_2.ToString());

            if (v.MergedX31ID_3 > 0)
            {
                s = cc.GeneratePdfReport(Factory, _pp, Factory.x31ReportBL.Load(v.MergedX31ID_3), BO.BAS.GetGuid(), v.rec_pid);
                pdfMerge.AppendPDFFile(s);                
            }
            Factory.CBL.SetUserParam("ReportContext-mergedx31id-3-" + v.rec_prefix, v.MergedX31ID_3.ToString());
            
            string strFinalRepFileName = cc.GetReportExportName(Factory, v.rec_pid, v.RecX31)+ ".pdf";
            pdfMerge.SaveMergedPDFToFile(Factory.x35GlobalParamBL.TempFolder() + "\\" + strUploadGuid+"_"+strFinalRepFileName);
            Factory.o27AttachmentBL.CreateTempInfoxFile(strUploadGuid, 8, strUploadGuid + "_" + strFinalRepFileName, strFinalRepFileName, "application/pdf");
            

            return strUploadGuid+"_"+strFinalRepFileName;
        }


        

    }
}