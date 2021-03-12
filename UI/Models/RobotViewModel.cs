using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class RobotViewModel
    {
        public string MessageToUser { get; set; }

        public IEnumerable<BO.j91RobotLog> lisLast20 { get; set; }
    }
}
