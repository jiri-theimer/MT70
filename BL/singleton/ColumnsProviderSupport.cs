using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public abstract class ColumnsProviderSupport
    {
        private List<BO.TheGridColumn> _lis;
        
        public BO.TheGridColumn oc;
        public string CurrentFieldGroup { get; set; }
        public BO.TheGridDefColFlag gdc1 = BO.TheGridDefColFlag.GridAndCombo;
        public BO.TheGridDefColFlag gdc0 = BO.TheGridDefColFlag._none;
        public BO.TheGridDefColFlag gdc2 = BO.TheGridDefColFlag.GridOnly;
        public string EntityName { get; set; }

        public ColumnsProviderSupport()
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
                TranslateLang3 = strHeader
            });
            
            return _lis[_lis.Count - 1];
        }

        public BO.TheGridColumn AFDATE(string strField, string strHeader, string strSqlSyntax = null)
        {
            return AF(strField, strHeader, strSqlSyntax, "date");
        }

        public BO.TheGridColumn AFNUM_OCAS(string strField, string strHeader, string strSqlSyntax = null, bool bolIsShowTotals = false)
        {
            BO.TheGridColumn c = AF(strField, strHeader, strSqlSyntax, "num",bolIsShowTotals);
            c.RelSql = "LEFT OUTER JOIN dbo.view_p31_ocas p31_ocas ON a.p31ID=p31_ocas.p31ID";
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
                case "bool":
                    return 75;
                default:
                    return 0;
            }

        }
    }
}
