using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class b07ColumnsProvider:ColumnsProviderBase
    {
        public b07ColumnsProvider()
        {
            this.EntityName = "b07Comment";

            oc=AF("b07Date", "Datum",null,"datetime"); oc.DefaultColumnFlag = gdc1;
            oc=AF("b07Value", "Text poznámky", "convert(varchar(255),a.b07Value)"); oc.DefaultColumnFlag = gdc1;
            AF("b07LinkName", "Název odkazu");
            AF("b07LinkUrl", "Odkaz (url)", "case when a.b07LinkUrl IS NOT NULL THEN '<a href='+ char(34)+a.b07LinkUrl + char(34)+ ' target='+char(34)+'_blank'+char(34)+'>'+a.b07LinkName+'</a>' end");
            AF("b07ReminderDate", "Čas připomenutí", null, "datetime");

            AppendTimestamp();
        }
    }
}
