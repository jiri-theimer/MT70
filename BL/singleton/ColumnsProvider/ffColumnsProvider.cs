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
            var lis = f.x28EntityFieldBL.GetList(new BO.myQuery("x28")).OrderBy(p=>p.x29TableName);
            string strLastTable = "";   string strField = "";string strPrefix = "";
            foreach(var rec in lis)
            {
                strPrefix = rec.x29TableName.Substring(0, 3);
                if (rec.x28Flag == BO.x28FlagENUM.UserField)
                {
                    strField = rec.x28Field;
                    
                }
                else
                {
                    strField = "gridFreefield_" + rec.pid.ToString();           //musí tam být výraz 'Free', který se explicitně testuje v j72Columns           
                    rec.x28Name += "+";
                }
                

                //if (strLastTable != rec.x29TableName)
                //{
                //    this.EntityName = rec.SourceTableName;                                        
                //}

                this.EntityName = rec.x29TableName;
                this.CurrentFieldGroup = "Uživatelská pole";
                
                switch (rec.x24ID)
                {
                    case BO.x24IdENUM.tInteger:
                        oc=AF(strField, rec.x28Name, null, "num0", rec.x28IsGridTotals);
                       
                        break;
                    case BO.x24IdENUM.tDecimal:
                        oc=AF(strField, rec.x28Name, null, "num", rec.x28IsGridTotals);
                        break;
                    case BO.x24IdENUM.tDate:
                        oc=AFDATE(strField, rec.x28Name);
                        break;
                    case BO.x24IdENUM.tDateTime:
                        oc=AF(strField, rec.x28Name, null, "datetime");
                        break;
                    case BO.x24IdENUM.tBoolean:
                        oc=AFBOOL(strField, rec.x28Name);
                        break;
                    default:
                        oc=AF(strField, rec.x28Name);
                        break;
                }

                //oc.NotShowRelInHeader = true;
                if (rec.x28Flag == BO.x28FlagENUM.GridField)        //čistě grid uživatelské pole na míru
                {
                    oc.SqlSyntax = rec.x28Grid_SqlSyntax;
                    oc.RelSqlInCol = rec.x28Grid_SqlFrom;
                }
                else
                {                    
                    oc.SqlSyntax = "ff_relname_" + strPrefix + "." + rec.x28Field;                   
                    oc.RelSqlInCol = "LEFT OUTER JOIN " + rec.SourceTableName + " ff_relname_" + strPrefix + " ON a." + strPrefix + "ID = ff_relname_" + strPrefix + "." + strPrefix + "ID";
                }
               
                strLastTable = rec.x29TableName;
            }

            


        }


        private void AA(BO.x28EntityField rec)
        {
            
        }
    }
}
