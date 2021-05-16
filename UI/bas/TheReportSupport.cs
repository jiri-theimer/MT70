using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI
{
    public class TheReportSupport
    {
        public string GeneratePdfReport(BL.Factory f, BL.ThePeriodProvider pp,BO.x31Report recX31,string strUploadGuid,int recpid,bool bolReturnFullPath=true)
        {
            var uriReportSource = new Telerik.Reporting.UriReportSource();
            uriReportSource.Uri = f.x35GlobalParamBL.ReportFolder() + "\\" + f.x31ReportBL.LoadReportDoc(recX31.pid).o27ArchiveFileName;

            if (recpid > 0)
            {
                uriReportSource.Parameters.Add("pid", recpid);
            }

            var per = new UI.Models.PeriodViewModel() { UserParamKey = "report-period" };
            per.InhaleUserPeriodSetting(pp, f, null, null);
            
            uriReportSource.Parameters.Add("datfrom", per.d1);
            uriReportSource.Parameters.Add("datuntil", per.d2);

            Telerik.Reporting.Processing.ReportProcessor processor = new Telerik.Reporting.Processing.ReportProcessor(f.App.Configuration);

            var result = processor.RenderReport("PDF", uriReportSource, null);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
            ms.Seek(0, System.IO.SeekOrigin.Begin);

            int intO13ID = 8;
            string strReportFileName = GetReportExportName(f, recpid, recX31)+".pdf";

            BO.BASFILE.SaveStream2File(f.x35GlobalParamBL.TempFolder() + "\\" + strUploadGuid + "_"+ strReportFileName, ms);

            

            f.o27AttachmentBL.CreateTempInfoxFile(strUploadGuid, intO13ID, strUploadGuid + "_"+ strReportFileName, strReportFileName, "application/pdf");
            if (bolReturnFullPath)
            {
                return f.x35GlobalParamBL.TempFolder() + "\\" + strUploadGuid + "_"+ strReportFileName;
            }
            else
            {
                return strUploadGuid + "_"+ strReportFileName;
            }
            
        }

        public string GetReportExportName(BL.Factory f,int pid,BO.x31Report recX31) //vygeneruje název PDF souboru tiskové sestavy
        {
            string s = null;
            string prefix = BO.BASX29.GetPrefix(recX31.x29ID);
            if (recX31.x31ExportFileNameMask != null)
            {
                s = f.x31ReportBL.ParseExportFileNameMask(recX31.x31ExportFileNameMask, prefix, pid);
                if (s != null)
                {
                    return s;
                }
            }
            
            if (s==null && prefix !=null && pid > 0 && prefix !="x31")
            {
                s = f.CBL.GetObjectAlias(prefix, pid);
            }

            if (s == null)
            {
                s = BO.BASFILE.PrepareFileName(recX31.x31Name);
            }
            else
            {
                s = BO.BASFILE.PrepareFileName(s);
            }


            return s;



        }
    }
}
