using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class RoleAssignViewModel
    {
        public string elementidprefix { get; set; } = "roles.";
        public string RecPrefix { get; set; }
        public int RecPid { get; set; }

        public List<RoleAssignRepeator> lisRepeator { get; set; }
    }

    public class RoleAssignRepeator
    {
        public int x67ID { get; set; }
        public string x67Name { get; set; }
        public string j02IDs { get; set; }
        public string Persons { get; set; }

        public string j11IDs { get; set; }
        public string Teams { get; set; }

        public bool IsAllPersons { get; set; }
        public bool IsNone { get; set; } = true;
      
    }
}
