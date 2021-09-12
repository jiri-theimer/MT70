using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31approve
{
    public class GatewayViewModel:BaseViewModel
    {
        public string guid { get; set; }
        public string prefix { get; set; }
        public string pidsinline { get; set; }
        public BO.p72IdENUM p72id { get; set; }
        public int p91id { get; set; }
        public int approvinglevel { get; set; }

        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }

        public bool DoDefaultApproveState { get; set; } = true;
    }
}
