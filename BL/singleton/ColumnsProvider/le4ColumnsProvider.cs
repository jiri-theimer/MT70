using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class le4ColumnsProvider: ColumnsProviderBase
    {
        public le4ColumnsProvider()
        {
            this.EntityName = "le4";

            this.CurrentFieldGroup = "Root";
            oc = AF("p41Name", "L4"); oc.NotShowRelInHeader = true; oc.DefaultColumnFlag = gdc1;
            oc = AF("PrefferedName", "Projekt+", "isnull(a.p41NameShort,a.p41Name)"); oc.NotShowRelInHeader = true;
            AF("p41NameShort", "Zkrácený název");
            oc = AF("p41Code", "Kód projektu"); oc.FixedWidth = 100; oc.DefaultColumnFlag = gdc1;
            AF("ProjectPid", "PID", "a.p41ID", "int");
            AF("p41PlanFrom", "Plánované zahájení", "a.p41PlanFrom", "datetime");
            AF("p41PlanUntil", "Plánované dokončení", "a.p41PlanUntil", "datetime");

            AF("p41ExternalPID", "Externí kód");

            this.CurrentFieldGroup = "Fakturační nastavení";
            AF("p41BillingMemo", "Fakturační poznámka");
            oc = AF("PrirazenyCenik", "Přiřazený fakturační ceník", "le4p51billing.p51Name"); oc.RelSqlInCol = "LEFT OUTER JOIN p51PriceList le4p51billing ON a.p51ID_Billing=le4p51billing.p51ID";
            oc = AF("SkutecnyCenik", "Skutečný fakturační ceník", "dbo.get_billing_pricelist_name(a.p41ID,a.p28ID_Client)");
            oc = AF("FakturacniJazyk", "Fakturační jazyk", "p87le4.p87Name"); oc.RelSqlInCol = "LEFT OUTER JOIN p87BillingLanguage p87le4 On a.p87ID=p87le4.p87ID";
            AF("p41LimitHours_Notification", "WIP limit Fa hodin", "a.p41LimitHours_Notification", "num", true);
            AF("p41LimitFee_Notification", "WIP limitní honorář", "a.p41LimitFee_Notification", "num", true);

            AppendTimestamp();
        }
    }
}
