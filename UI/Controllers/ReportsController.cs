﻿using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Telerik.Reporting;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;
using System.Collections.Generic;
using Telerik.Reporting.Services.Engine;
using System.IO;
using BL;
using System;

namespace UI.Controllers
{    
    [Route("api/reports")]    
    public class ReportsController : ReportsControllerBase
    {
        

        public ReportsController(IReportServiceConfiguration reportServiceConfiguration,BL.RunningApp app,BL.Factory f) : base(reportServiceConfiguration)
        {
            
        
            var resolver = new CustomReportSourceResolver(app,f);
            reportServiceConfiguration.Storage = new Telerik.Reporting.Cache.File.FileStorage(f.x35GlobalParamBL.TempFolder());
            reportServiceConfiguration.HostAppId = "ReportViewer" + app.AppName;    //HostAppId musí být unikátní v rámci všech websites na serveru


            reportServiceConfiguration.ReportSourceResolver = resolver;

            
            

        }



        

    }


    public class CustomReportSourceResolver : IReportSourceResolver
    {
        private BL.RunningApp _app;
        private BL.Factory _f;
        public CustomReportSourceResolver(BL.RunningApp app,BL.Factory f)
        {
            _app = app;
            _f = f;
            
        }
        public Telerik.Reporting.ReportSource Resolve(string reportId, OperationOrigin operationOrigin, IDictionary<string, object> currentParameterValues)
        {
            //soubor sestavy###login uživatele###j72id###reportexportname
            

            List<string> lis = BO.BAS.ConvertString2List(reportId, "###");
            reportId = lis[0];
            string strLogin = lis[1];
            int intJ72ID = 0;
            
            if (lis.Count > 2)
            {
                intJ72ID = BO.BAS.InInt(lis[2]);
            }
            



            string reportXml = File.ReadAllText(_f.x35GlobalParamBL.ReportFolder() + "\\" + reportId);
            if (lis.Count > 3)
            {
                reportXml = reportXml.Replace("report1", lis[3]);        //explicitně definovaný název exportu sestavy        
            }
            
            if (reportXml.Contains("1=1"))
            {
                var cu = new BO.RunningUser() { j03Login = strLogin };
                BL.Factory f = new BL.Factory(cu, _app,null,null);
                if (intJ72ID > 0)
                {
                    var recJ72 = f.j72TheGridTemplateBL.Load(intJ72ID);                    
                    var mq = new BO.InitMyQuery().Load(recJ72.j72Entity);
                    mq.lisJ73= f.j72TheGridTemplateBL.GetList_j73(intJ72ID, recJ72.j72Entity.Substring(0, 3));
                    
                    DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql("", mq, cu);
                    
                    string strFilterAlias = recJ72.j72Name;
                    if (recJ72.j72HashJ73Query)
                    {
                        strFilterAlias = f.j72TheGridTemplateBL.getFiltrAlias(recJ72.j72Entity.Substring(0, 3), mq);
                    }
                    reportXml = reportXml.Replace("1=1", fq.SqlWhere).Replace("#query_alias#", strFilterAlias);
                    
                }
                
            }

            

            //if (operationOrigin.ToString() == "GenerateReportDocument")     //finální volání z report viewer - celkem se volá až 3x!
            //{

            //    var uriReportSource = new Telerik.Reporting.UriReportSource();
            //    uriReportSource.Uri = _f.x35GlobalParamBL.ReportFolder() + "\\" + reportId;

            //    Telerik.Reporting.Processing.ReportProcessor processor = new Telerik.Reporting.Processing.ReportProcessor();

            //    var result = processor.RenderReport("PDF", uriReportSource, null);

            //    MemoryStream ms = new MemoryStream();
            //    ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
            //    ms.Seek(0, SeekOrigin.Begin);
            //    BO.BASFILE.SaveStream2File("c:\\temp\\marktime_report.pdf", ms);
            //}


            return new Telerik.Reporting.XmlReportSource { Xml = reportXml };
        }
    }


}