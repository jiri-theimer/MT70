using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class p56ColumnsProvider: ColumnsProviderBase
    {
        public p56ColumnsProvider()
        {
            this.EntityName = "p56Task";

            this.CurrentFieldGroup = "Root";
            oc = AF("p56Name", "Úkol", null, "string"); oc.NotShowRelInHeader = true; oc.DefaultColumnFlag = gdc1;
            oc = AF("p56Code", "Kód"); oc.FixedWidth = 100;
           

            this.CurrentFieldGroup = "Plán úkolu";
            
            AFDATE("p56PlanUntil", "Termín");
            AF("DnuDoTerminu", "Dnů do termínu", "datediff(day,a.p56PlanUntil,getdate())","num0");
            AF("HodinDoTerminu", "Hodin do termínu", "datediff(hour,a.p56PlanUntil,getdate())", "num0");
            AF("DnuPoTerminu", "Dnů po termínu", "datediff(day,getdate(),a.p56PlanUntil)", "num0");
            AF("HodinPoTerminu", "Hodin po termínu", "datediff(hour,getdate(),a.p56PlanUntil)", "num0");
            AFDATE("p56PlanFrom", "Plánované zahájení");
            AF("p56Plan_Hours", "Plán hodin", null, "num");
            AF("p56Plan_Expenses", "Plán výdajů", null, "num");

            
            AppendTimestamp();

           
        }

    }
}
