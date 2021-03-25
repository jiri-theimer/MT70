using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ffColumnsProvider:ColumnsProviderBase
    {
        public ffColumnsProvider(BL.Factory f)
        {
            this.EntityName = "p41Project_FreeField";

            AF("p41FreeText01", "E/V/IL");
            AF("p41FreeDate01", "Expirace pasu",null,"datetime");

        }
    }
}
