﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ffColumnsProvider:ColumnsProviderBase
    {
        public ffColumnsProvider(BL.Factory f,string prefix)
        {
            if (prefix == "o23" || prefix==null)
            {
                Handle_o23UserFields(f);    //dokumenty přes x16EntityCategory_FieldSetting
            }

            var lisX28 = f.x28EntityFieldBL.GetList(new BO.myQuery("x28")).OrderBy(p=>p.x29TableName);
            string strField = "";string strPrefix = "";
            foreach(var rec in lisX28)
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

                

                if (strPrefix == "p41" && f.CurrentUser.p07LevelsCount>1 && (prefix==null || prefix.Substring(0,2)=="le"))
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        if (f.CurrentUser.getP07Level(i, true) != null)
                        {
                            this.EntityName = "le"+i.ToString();
                            var ocx = AF($"gridFreefield_le{i}_" + rec.pid.ToString(), oc.Header, oc.SqlSyntax, oc.FieldType, oc.IsShowTotals);
                            ocx.SqlSyntax = ocx.SqlSyntax.Replace("_p41", $"_le{i}");
                            ocx.RelSqlInCol = $"LEFT OUTER JOIN p41Project_FreeField ff_relname_le{i} ON a.p41ID = ff_relname_le{i}.p41ID";
                        }
                    }                    
                }

            }


            Handle_Stitky(f);

            


        }

        
        private void Handle_Stitky_All(int x29id,string entity,string prefix,string prefixdb)
        {            
            oc = AF($"AllFreeTags_{prefix}", "Štítky", $"{prefix}_o54.o54InlineHtml");
            oc.RelSqlInCol = $"LEFT OUTER JOIN o54TagBindingInline {prefix}_o54 ON a.{prefixdb}ID = {prefix}_o54.o54RecordPid AND {prefix}_o54.x29ID={x29id}";
            oc.Entity = entity;
            oc = AF($"AllFreeTags_{prefix}text", "Štítky pouze text", $"{prefix}_o54.o54InlineText");
            oc.RelSqlInCol = $"LEFT OUTER JOIN o54TagBindingInline {prefix}_o54 ON a.{prefixdb}ID = {prefix}_o54.o54RecordPid AND {prefix}_o54.x29ID={x29id}";
            oc.Entity = entity;
        }
        private void Handle_Stitky(BL.Factory f)
        {
            //štítky
            this.CurrentFieldGroup = "Štítky";

            Handle_Stitky_All(328, "p28Contact","p28","p28");
            
            Handle_Stitky_All(223, "o23Doc", "o23", "o23");
            Handle_Stitky_All(331, "p31Worksheet", "p31", "p31");
            Handle_Stitky_All(356, "p56Task", "p56", "p56");
            Handle_Stitky_All(102, "j02Person", "j02", "j02");

            if (f.CurrentUser.p07LevelsCount > 1)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (f.CurrentUser.getP07Level(i, true) != null)
                    {
                        Handle_Stitky_All(141, "le" + i.ToString(), "le" + i.ToString(), "p41");
                    }

                }
            }
            else
            {
                Handle_Stitky_All(141, "p41Project", "p41", "p41");
            }
            
            
            var lisO53 = f.o53TagGroupBL.GetList(new BO.myQuery("o53")).Where(p => p.o53Field != null && p.x29IDs != null);
            foreach (var c in lisO53)
            {
                var x29ids = BO.BAS.ConvertString2ListInt(c.x29IDs);
                
                foreach (int x29id in x29ids)
                {
                    var cc = (BO.x29IdEnum)x29id;
                    this.EntityName = BO.BASX29.GetEntity(cc);
                    string strTagPrefix = this.EntityName.Substring(0, 3);
                    
                    oc = AF("FreeTag"+c.o53Field, c.o53Name, strTagPrefix+"_o54."+c.o53Field);  //důležité je, aby obsahoval výraz "Free"
                    oc.RelSqlInCol = $"LEFT OUTER JOIN o54TagBindingInline {strTagPrefix}_o54 ON a.{strTagPrefix}ID = {strTagPrefix}_o54.o54RecordPid AND {strTagPrefix}_o54.x29ID={x29id}";
                    if (x29id == 141 && f.CurrentUser.p07LevelsCount>1)
                    {
                        for(int i = 1; i <= 5; i++)
                        {
                            if (f.CurrentUser.getP07Level(i, true) != null)
                            {
                                strTagPrefix = "le" + i.ToString();
                                string strDbPrefix = "p41";
                                oc = AF("FreeTag" + c.o53Field, c.o53Name, strTagPrefix + "_o54." + c.o53Field);  //důležité je, aby obsahoval výraz "Free"
                                oc.Entity = "le" + i.ToString();
                                oc.RelSqlInCol = $"LEFT OUTER JOIN o54TagBindingInline {strTagPrefix}_o54 ON a.{strDbPrefix}ID = {strTagPrefix}_o54.o54RecordPid AND {strTagPrefix}_o54.x29ID={x29id}";

                            }

                        }
                    }
                }



            }
        }

        private void Handle_o23UserFields(BL.Factory f)
        {
            this.EntityName = "o23Doc";
            var lisX18 = f.x18EntityCategoryBL.GetList(new BO.myQueryX18());
            var lisX16 = f.x18EntityCategoryBL.GetList_x16();
            foreach (var recX18 in lisX18)
            {
                this.CurrentFieldGroup = recX18.x18Name;
                var qry = lisX16.Where(p => p.x18ID == recX18.pid && p.x16IsGridField==true).OrderBy(p => p.x16Ordinary);
                foreach(var c in qry)
                {
                    string strSQL = null;string strType = "string";
                    switch (c.FieldType)
                    {
                        case BO.x24IdENUM.tDate:
                            strSQL = "dbo.my_iif2_date(x18.x18ID," + c.x18ID.ToString() + "," + c.x16Field + ",null)";
                            strType = "date";
                            break;
                        case BO.x24IdENUM.tDateTime:
                            strSQL = "dbo.my_iif2_date(x18.x18ID," + c.x18ID.ToString() + "," + c.x16Field + ",null)";
                            strType = "datetime";                            
                            break;
                        case BO.x24IdENUM.tDecimal:
                            strSQL = "dbo.my_iif2_number(x18.x18ID," + c.x18ID.ToString() + "," + c.x16Field + ",null)";
                            strType = "num";
                            break;
                        case BO.x24IdENUM.tInteger:
                            strSQL = "dbo.my_iif2_number(x18.x18ID," + c.x18ID.ToString() + "," + c.x16Field + ",null)";
                            strType = "num0";
                            break;
                        case BO.x24IdENUM.tBoolean:
                            strSQL = "dbo.my_iif2_bit(x18.x18ID," + c.x18ID.ToString() + "," + c.x16Field + ",null)";
                            strType = "bool";
                            break;
                        default:                            
                            strSQL = "dbo.my_iif2_string(x18.x18ID," + c.x18ID.ToString() + "," + c.x16Field + ",null)";
                            strType = "string";
                            break;
                    }
                    
                    oc = AF("x16_Freefield" + c.x16ID.ToString(), c.x16Name, strSQL, strType);
                    if (c.x16NameGrid != null)
                    {
                        oc.Header = c.x16NameGrid;
                    }
                }
            }

        }


        private void AA(BO.x28EntityField rec)
        {
            
        }
    }
}
