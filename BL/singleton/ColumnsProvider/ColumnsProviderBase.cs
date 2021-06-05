using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public abstract class ColumnsProviderBase
    {
        private List<BO.TheGridColumn> _lis;
        
        public BO.TheGridColumn oc;
        public string CurrentFieldGroup { get; set; }
        public BO.TheGridDefColFlag gdc1 = BO.TheGridDefColFlag.GridAndCombo;
        public BO.TheGridDefColFlag gdc0 = BO.TheGridDefColFlag._none;
        public BO.TheGridDefColFlag gdc2 = BO.TheGridDefColFlag.GridOnly;
        public string EntityName { get; set; }

        public ColumnsProviderBase()
        {
            _lis = new List<BO.TheGridColumn>();
        }

        public List<BO.TheGridColumn> getColumns()
        {
            return _lis;
        }

        public BO.TheGridColumn AF(string strField, string strHeader, string strSqlSyntax = null, string strFieldType = "string", bool bolIsShowTotals = false)
        {
            
            _lis.Add(new BO.TheGridColumn() { Field = strField,
                Entity = this.EntityName,
                Header = strHeader,
                DefaultColumnFlag = BO.TheGridDefColFlag._none,
                SqlSyntax = strSqlSyntax,
                FieldType = strFieldType,
                IsShowTotals = bolIsShowTotals,
                NotShowRelInHeader = false,
                FixedWidth = SetDefaultColWidth(strFieldType),
                TranslateLang1 = strHeader, TranslateLang2 = strHeader,
                TranslateLang3 = strHeader,
                DesignerGroup=this.CurrentFieldGroup
            });
            
            return _lis[_lis.Count - 1];
        }

        public BO.TheGridColumn AFBOOL(string strField, string strHeader)
        {
            return AF(strField, strHeader, null, "bool");
        }
        public BO.TheGridColumn AFNUM0(string strField, string strHeader)
        {
            return AF(strField, strHeader, null, "num0", false);
        }
       
        public BO.TheGridColumn AFDATE(string strField, string strHeader, string strSqlSyntax = null)
        {
            return AF(strField, strHeader, strSqlSyntax, "date");
        }

        public BO.TheGridColumn AFNUM_OCAS(string strField, string strHeader, string strSqlSyntax = null, bool bolIsShowTotals = false)
        {
            BO.TheGridColumn c = AF(strField, strHeader, strSqlSyntax, "num",bolIsShowTotals);
            c.RelSqlInCol = "LEFT OUTER JOIN dbo.view_p31_ocas p31_ocas ON a.p31ID=p31_ocas.p31ID";
            return c;
        }

        private int SetDefaultColWidth(string strFieldType)
        {
            switch (strFieldType)
            {
                case "date":
                    return 90;
                case "datetime":
                    return 120;
                case "num":
                case "num4":
                case "num5":
                case "num3":
                    return 100;
                case "num0":
                    return 75;
                case "int":
                    return 65;
                case "bool":
                    return 75;
                default:
                    return 0;
            }

        }


        private BO.TheGridColumn AF_TIMESTAMP(string strField, string strHeader, string strSqlSyntax, string strFieldType)
        {
            BO.TheGridColumn c = AF(strField, strHeader, strSqlSyntax, strFieldType, false);
            c.IsTimestamp = true;
            return c;
            

        }

        public void AppendTimestamp(bool include_validity = true)
        {
            this.CurrentFieldGroup = "Časové razítko záznamu";
            string prefix = this.EntityName.Substring(0, 3);
            AF_TIMESTAMP("DateInsert_" + this.EntityName, "Založeno", "a." + prefix + "DateInsert", "datetime");
            AF_TIMESTAMP("UserInsert_" + this.EntityName, "Založil", "a." + prefix + "UserInsert", "string");
            AF_TIMESTAMP("DateUpdate_" + this.EntityName, "Aktualizace", "a." + prefix + "DateUpdate", "datetime");
            AF_TIMESTAMP("UserUpdate_" + this.EntityName, "Aktualizoval", "a." + prefix + "UserUpdate", "string");
            if (include_validity == true)
            {
                AF_TIMESTAMP("ValidFrom_" + this.EntityName, "Platné od", "a." + prefix + "ValidFrom", "datetime");
                AF_TIMESTAMP("ValidUntil_" + this.EntityName, "Platné do", "a." + prefix + "ValidUntil", "datetime");

                AF_TIMESTAMP("IsValid_" + this.EntityName, "Časově platné", string.Format("convert(bit,case when GETDATE() between a.{0}ValidFrom AND a.{0}ValidUntil then 1 else 0 end)", prefix), "bool");
            }

            this.CurrentFieldGroup = null;
        }


        public void AppendProjectColumns(string prefix)
        {
            oc = AF("PrefferedName", "Projekt+", "isnull(a.p41NameShort,a.p41Name)"); oc.NotShowRelInHeader = true;
            AF("p41NameShort", "Zkrácený název");
            oc = AF("p41Code", "Kód projektu"); oc.FixedWidth = 100; oc.DefaultColumnFlag = gdc1;
            AF("p41PlanFrom", "Plánované zahájení", "a.p41PlanFrom", "datetime");
            AF("p41PlanUntil", "Plánované dokončení", "a.p41PlanUntil", "datetime");

            if (prefix == "le5" || prefix=="le4" || prefix=="le3" || prefix=="le2")
            {
                oc = AF("Nadrizeny", "Nadřízená úroveň", $"{prefix}parent.p41Name"); oc.RelSqlInCol = $"LEFT OUTER JOIN p41Project {prefix}parent On a.p41ParentID={prefix}parent.p41ID";
            }
            
            this.CurrentFieldGroup = "Fakturační nastavení";
            AF("p41BillingMemo", "Fakturační poznámka");
            oc = AF("Odberatel", "Odběratel faktury", $"{prefix}_odberatel.p28Name"); oc.RelSqlInCol = $"LEFT OUTER JOIN p28Contact {prefix}_odberatel On a.p28ID_Billing={prefix}_odberatel.p28ID";
            oc = AF("PrirazenyCenik", "Přiřazený fakturační ceník", $"{prefix}p51billing.p51Name"); oc.RelSqlInCol = $"LEFT OUTER JOIN p51PriceList {prefix}p51billing ON a.p51ID_Billing={prefix}p51billing.p51ID";
            oc = AF("SkutecnyCenik", "Skutečný fakturační ceník", "dbo.get_billing_pricelist_name(a.p41ID,a.p28ID_Client)");
            oc = AF("FakturacniJazyk", "Fakturační jazyk", $"p87{prefix}.p87Name"); oc.RelSqlInCol = $"LEFT OUTER JOIN p87BillingLanguage p87{prefix} On a.p87ID=p87{prefix}.p87ID";
            AF("p41LimitHours_Notification", "WIP limit Fa hodin", "a.p41LimitHours_Notification", "num", true);
            AF("p41LimitFee_Notification", "WIP limitní honorář", "a.p41LimitFee_Notification", "num", true);
        }
    }
}
