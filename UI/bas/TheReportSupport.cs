using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI
{
    public class TheReportSupport
    {
        public string GeneratePdfReport(BL.Factory f, BL.ThePeriodProvider pp,BO.x31Report recX31,string strUploadGuid,int recpid)
        {
            var uriReportSource = new Telerik.Reporting.UriReportSource();
            uriReportSource.Uri = f.x35GlobalParamBL.ReportFolder() + "\\" + f.x31ReportBL.LoadReportDoc(recX31.pid).o27ArchiveFileName;

            if (recpid > 0)
            {
                uriReportSource.Parameters.Add("pid", recpid);
            }
            var per = f.x31ReportBL.InhalePeriodFilter(pp);
            uriReportSource.Parameters.Add("datfrom", per.d1);
            uriReportSource.Parameters.Add("datuntil", per.d2);

            Telerik.Reporting.Processing.ReportProcessor processor = new Telerik.Reporting.Processing.ReportProcessor(f.App.Configuration);

            var result = processor.RenderReport("PDF", uriReportSource, null);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
            ms.Seek(0, System.IO.SeekOrigin.Begin);


            BO.BASFILE.SaveStream2File(f.x35GlobalParamBL.TempFolder() + "\\" + strUploadGuid + "_report.pdf", ms);

            int intO13ID = 8;

            f.o27AttachmentBL.CreateTempInfoxFile(strUploadGuid, intO13ID, strUploadGuid + "_report.pdf", "report.pdf", "application/pdf");
            return f.x35GlobalParamBL.TempFolder() + "\\" + strUploadGuid + "_report.pdf";
        }
    }
}
