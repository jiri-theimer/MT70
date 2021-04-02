using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class o23ColumnsProvider: ColumnsProviderBase
    {
        public o23ColumnsProvider()
        {
            this.EntityName = "o23Doc";
            
            oc = AF("o23Name", "Název dokumentu"); oc.NotShowRelInHeader = true; oc.DefaultColumnFlag = gdc1;
            oc = AF("DocType", "Typ dokumentu", "x18.x18Name"); oc.RelSqlInCol = "INNER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID INNER JOIN x18EntityCategory x18 On x23.x23ID=x18.x23ID"; oc.NotShowRelInHeader = true; oc.DefaultColumnFlag = gdc1;
            AF("o23Code", "Kód dokumentu");
            AFDATE("o23FreeDate01", "Datum");
            AF("o23ReminderDate", "Připomenutí", null, "datetime");
            AF("ReceiversInLine", "Příjemci", "dbo.o23_getroles_inline(a.o23ID)");

           
            AppendTimestamp();
        }
    }
}
