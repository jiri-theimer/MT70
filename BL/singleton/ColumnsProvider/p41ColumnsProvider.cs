using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class p41ColumnsProvider:ColumnsProviderBase
    {
        public p41ColumnsProvider()
        {
            this.EntityName = "p41Project";

            this.CurrentFieldGroup = "Root";
            oc = AF("p41Name", "L5"); oc.NotShowRelInHeader = true; oc.DefaultColumnFlag = gdc1;
            this.AppendProjectColumns("p41");

            AppendTimestamp();
        }
    }
}
