using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading;
using Microsoft.AspNetCore.Mvc;
using BL;
using Microsoft.VisualBasic;
using System.Net.Http;

namespace UI
{
    public class TheRobot : IHostedService, IDisposable
    {
        private int executionCount = 0;        
        private Timer _timer;

        private readonly IHttpClientFactory _httpclientfactory;
        private readonly BL.TheEntitiesProvider _ep;
        private readonly BL.RunningApp _app;
        private readonly BL.TheTranslator _tt;

        public TheRobot(IHttpClientFactory hcf, BL.TheEntitiesProvider ep, BL.RunningApp app, BL.TheTranslator tt)
        {
            _httpclientfactory = hcf;
            _ep = ep;
            _app = app;
            _tt = tt;
        }



        public Task StartAsync(CancellationToken stoppingToken)
        {
            LogInfo("Timed Hosted Service running.");
            
            _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromSeconds(120));   //každých 120 sekund
            
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            LogInfo("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }


        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);
                        
            LogInfo("Timed Hosted Service is working.");


            var fidoo = new XA.Controllers.FidooController(_httpclientfactory, _ep, _app, _tt);
            //fidoo.Index();

            LogInfo("Alfter fidoo.Index();.");
        }

        
        

        

        
        private void LogInfo(string strMessage)
        {
            var strPath = string.Format("{0}\\the-robot-{1}.log", _app.LogFolder, DateTime.Now.ToString("yyyy.MM.dd"));

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", DateTime.Now.ToString() + ": ", strMessage });
        }


    }
}
