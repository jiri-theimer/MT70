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
    public class TheRobotUI : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private Timer _timer;

        private readonly IHttpClientFactory _httpclientfactory;
        private readonly BL.RunningApp _app;
        private readonly BL.TheEntitiesProvider _ep;        
        private readonly BL.TheTranslator _tt;


        public TheRobotUI(IHttpClientFactory hcf,BL.RunningApp app, BL.TheEntitiesProvider ep, BL.TheTranslator tt)
        {          
            _app = app;
            _httpclientfactory = hcf;
            _ep = ep;            
            _tt = tt;
        }



        public Task StartAsync(CancellationToken stoppingToken)
        {
            LogInfo("Time Hosted Service TheRobotUI running (Task StartAsync).");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(300));   //každých 300 sekund

           
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            LogInfo("Timed Hosted Service TheRobotUI is stopping.");

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

            LogInfo("Timed Hosted Service TheRobotUI is working, count: "+ count.ToString());

            var c = new UI.Controllers.RobotController(_httpclientfactory,_app,_ep,_tt);
            LogInfo("RobotController result: "+c.Run());
            c.RunRobotPingByHttpClient();
        }







        private void LogInfo(string strMessage)
        {
            var strPath = string.Format("{0}\\TheRobotUI-{1}.log", _app.LogFolder, DateTime.Now.ToString("yyyy.MM.dd"));

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", DateTime.Now.ToString() + ": ", strMessage });
        }


    }
}
