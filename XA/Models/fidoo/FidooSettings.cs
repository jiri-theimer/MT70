using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.fidoo
{

    public class FidooSettings
    {
        public List<Member> members { get; set; }
    }

    public class Member
    {
        public string ApiKey { get; set; }
        public bool Closed { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? FirstExpenseModifyFrom { get; set; }
        public string Name { get; set; }
        public string RobotUser { get; set; }
        public string DefaultProject { get; set; }
        public bool ExportProjects { get; set; }
        public bool ImportExpenses { get; set; }
    }

}
