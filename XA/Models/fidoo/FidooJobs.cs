using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.fidoo
{

    public class FidooJobs
    {
        public List<FidooJob> jobs { get; set; }
    }

    public class FidooJob
    {
        public string ApiKey { get; set; }
        public bool Closed { get; set; }            //true: job je uzavřen a nemá se spouštět
        public DateTime? LastRun { get; set; }      //kdy naposledy došlo ke spuštění jobu
        public int RepeatMinuteInterval { get; set; } = 300;
        public DateTime? FirstExpenseModifyFrom { get; set; }
        public string Name { get; set; }
        public string RobotUser { get; set; }
        public string DefaultProject { get; set; }
        public bool ExportProjects { get; set; }
        public bool ImportExpenses { get; set; }
    }

}
