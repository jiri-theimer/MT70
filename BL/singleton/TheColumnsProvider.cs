using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BL
{

    public class TheColumnsProvider
    {
        //private readonly BL.RunningApp _app;
        private readonly BL.TheEntitiesProvider _ep;
        private readonly BL.TheTranslator _tt;
        private List<BO.TheGridColumn> _lis;


        public TheColumnsProvider(BL.TheEntitiesProvider ep, BL.TheTranslator tt)
        {
            //_app = runningapp;
            _ep = ep;
            _tt = tt;
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();
            Handle_Translate();

        }

        public void Refresh()
        {
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();
            Handle_Translate();
        }



        private void Handle_Translate()
        {
            //Překlad do ostatních jazyků
            foreach (var col in _lis.Where(p => p.Header.Length > 2))
            {
                bool b = true;
                if (col.Header.Length > 3 && col.Header.Substring(0, 3) == "Col")
                {
                    b = false;
                }
                if (b)
                {
                    col.TranslateLang1 = _tt.DoTranslate(col.Header, 1, "TheColumnsProvider");
                    col.TranslateLang2 = _tt.DoTranslate(col.Header, 2, "TheColumnsProvider");
                }

            }




        }
        private void SetupPallete()
        {
            _lis.InsertRange(0, new defColumnsProvider().getColumns());
            _lis.InsertRange(0, new j02ColumnsProvider().getColumns());
            _lis.InsertRange(0, new p28ColumnsProvider().getColumns());
            _lis.InsertRange(0, new p41ColumnsProvider().getColumns());
            _lis.InsertRange(0, new p56ColumnsProvider().getColumns());
            _lis.InsertRange(0, new p90ColumnsProvider().getColumns());
            _lis.InsertRange(0, new p91ColumnsProvider().getColumns());
            _lis.InsertRange(0, new p31ColumnsProvider().getColumns());
            _lis.InsertRange(0, new o23ColumnsProvider().getColumns());
            _lis.InsertRange(0, new b07ColumnsProvider().getColumns());

            //_lis.InsertRange(0, new le4ColumnsProvider().getColumns());
            

            string strLastEntity = "";
            string strLastEntityAlias = "";
            foreach (var c in _lis)
            {
                if (c.Entity == strLastEntity)
                {
                    c.EntityAlias = strLastEntityAlias;
                }
                else
                {
                    c.EntityAlias = _ep.ByTable(c.Entity).AliasSingular;
                }
                strLastEntity = c.Entity;
                strLastEntityAlias = c.EntityAlias;
            }




        }

        //public string getDefaultPalletePreSaved(string entity,string master_entity,BO.baseQuery mq)  //vrací seznam výchozí palety sloupců pro grid: pouze pro významné entity
        //{
        //    string s = null;     
        //    switch (mq.Prefix)
        //        {                
        //        case "p31":
        //            s= "a__p31Worksheet__p31Date,p31_j02__j02Person__fullname_desc,p31_p41_p28__p28Contact__p28Name,p31_p41__p41Project__p41Name,p31_p32__p32Activity__p32Name,a__p31Worksheet__p31Hours_Orig,a__p31Worksheet__p31Rate_Billing_Orig,a__p31Worksheet__p31Amount_WithoutVat_Orig,a__p31Worksheet__p31Text";
        //            switch (master_entity)
        //            {
        //                case "p91Invoice":
        //                    s = "p31Date,p31_j02__j02Person__fullname_desc,p31_p41__p41Project__p41Name,p31_p32__p32Activity__p32Name,p31Hours_Invoiced,p31Rate_Billing_Invoiced,p31Amount_WithoutVat_Invoiced,p31VatRate_Invoiced,p31Text";
        //                    break;
        //            }
        //            break;
        //        case "p28":

        //            break;
        //        default:
        //            return null;
        //    }
            
        //    if (s !=null)
        //    {
        //        List<string> lis = new List<string>();
        //        var arr = s.Split(",");
        //        for (int i = 0; i < arr.Count(); i++)   //pokud v definici sloupce chybí určení entity, pak doplnit:
        //        {
        //            if (arr[i].Contains("__"))
        //            {
        //                lis.Add(arr[i]);
        //            }
        //            else
        //            {
        //                lis.Add("a__" + entity + "__" + arr[i]);
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(master_entity))
        //        {
        //            lis = lis.Where(p => !p.Contains(master_entity)).ToList();  //aby se v podřízeném gridu nezobrazovali duplicitní sloupce z nadřízeného gridu
        //            if (master_entity == "p41Project")
        //            {
        //                lis = lis.Where(p => !p.Contains("p28Contact")).ToList();   //eliminivat klientské sloupce, pokud je nadřízená entita: Projekt
        //            }
        //        }


        //        s = string.Join(",", lis);
        //    }

        //    return s;

        //}
        public List<BO.TheGridColumn> getDefaultPallete(bool bolComboColumns, BO.baseQuery mq)
        {
           
            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();
            IEnumerable<BO.TheGridColumn> qry = null;
            if (bolComboColumns)
            {
                qry = _lis.Where(p => p.Prefix == mq.PrefixDb && (p.DefaultColumnFlag == BO.TheGridDefColFlag.GridAndCombo || p.DefaultColumnFlag == BO.TheGridDefColFlag.ComboOnly));

            }
            else
            {
                qry = _lis.Where(p => p.Prefix == mq.PrefixDb && (p.DefaultColumnFlag == BO.TheGridDefColFlag.GridAndCombo || p.DefaultColumnFlag == BO.TheGridDefColFlag.GridOnly));
            }

            foreach (BO.TheGridColumn c in qry)
            {
                ret.Add(Clone2NewInstance(c));
            }

            List<BO.EntityRelation> rels = _ep.getApplicableRelations(mq.Prefix);   //všechny dostupné relace pro entitu mq.prefix

            switch (mq.Prefix)
            {
                case "j02":
                    if (!bolComboColumns)
                    {
                        ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j03Login", rels, bolComboColumns));
                        ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j04Name", rels, bolComboColumns));
                    }
                    ret.Add(InhaleColumn4Relation("j02_j07", "j07PersonPosition", "j07Name", rels, bolComboColumns));


                    break;



                case "j61":
                    ret.Add(InhaleColumn4Relation("j61_x29", "x29Entity", "x29Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("j61_j02", "j02Person", "fullname_desc", rels, bolComboColumns));

                    break;
                case "p28":
                    if (bolComboColumns)
                    {
                        ret.Add(InhaleColumn4Relation("p28_address_primary", "view_PrimaryAddress", "o38City", rels, bolComboColumns));
                    }
                    else
                    {
                        ret.Add(InhaleColumn4Relation("p28_address_primary", "view_PrimaryAddress", "FullAddress", rels, bolComboColumns));
                    }

                    break;
                case "p31":
                    ret.Add(InhaleColumn4Relation("p31_j02", "j02Person", "fullname_desc", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p31_p41_p28", "p28Contact", "p28Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p31_p41", "p41Project", "p41Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p31_p32", "p32Activity", "p32Name", rels, bolComboColumns));
                    break;
                case "p36":
                    ret.Add(InhaleColumn4Relation("p36_j02", "j02Person", "fullname_desc", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p36_j11", "j11Team", "j11Name", rels, bolComboColumns));
                    break;
                //case "p42":
                //    ret.Add(InhaleColumn4Relation("p42_p07", "p07ProjectLevel", "p07Name", rels, bolComboColumns));
                //    ret.Add(InhaleColumn4Relation("p42_p07", "p07ProjectLevel", "LevelIndex", rels, bolComboColumns));
                //    break;
                case "p51":
                    ret.Add(InhaleColumn4Relation("p51_j27", "j27Currency", "j27Code", rels, bolComboColumns));
                    break;
                case "p53":
                    ret.Add(InhaleColumn4Relation("p53_j27", "j27Currency", "j27Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p53_x15", "x15VatRateType", "x15Name", rels, bolComboColumns));
                    break;
                case "p32":
                    ret.Add(InhaleColumn4Relation("p32_p34", "p34ActivityGroup", "p34Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p32_p95", "p95InvoiceRow", "p95Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p32_p38", "p38ActivityTag", "p38Name", rels, bolComboColumns));
                    break;
                case "b07":
                    ret.Add(InhaleColumn4Relation("b07_p28", "p28Contact", "p28Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("b07_p41", "p41Project", "p41Name", rels, bolComboColumns));
                    break;
                case "p90":
                    ret.Add(InhaleColumn4Relation("p90_j27", "j27Currency", "j27Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p90_p28", "p28Contact", "p28Name", rels, bolComboColumns));
                    break;
                case "p92":
                    ret.Add(InhaleColumn4Relation("p92_j27", "j27Currency", "j27Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p92_p93", "p93InvoiceHeader", "p93Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p92_x15", "x15VatRateType", "x15Name", rels, bolComboColumns));
                    break;
                case "x28":
                    ret.Add(InhaleColumn4Relation("x28_x29", "x29Entity", "x29Name", rels, bolComboColumns));
                    break;
                case "x31":
                    if (!bolComboColumns)
                    {
                        ret.Add(InhaleColumn4Relation("x31_x29", "x29Entity", "x29Name", rels, bolComboColumns));
                    }
                    
                    break;
                case "o40":
                    ret.Add(InhaleColumn4Relation("o40_j02", "j02Person", "fullname_desc", rels, bolComboColumns));
                    break;
                case "m62":
                    ret.Add(InhaleColumn4Relation("m62_j27slave", "j27Currency", "j27Code", rels, bolComboColumns));
                    break;
                case "o51":
                    ret.Add(InhaleColumn4Relation("o51_o53", "o53TagGroup", "o53Name", rels, bolComboColumns));
                    break;

            }

            return ret;


        }
        public List<BO.TheGridColumn> AllColumns()
        {

            return _lis;


        }
        private BO.TheGridColumn InhaleColumn4Relation(string strRelName, string strFieldEntity, string strFieldName, List<BO.EntityRelation> applicable_rels, bool bolComboColumns)
        {
            BO.TheGridColumn c0 = ByUniqueName("a__" + strFieldEntity + "__" + strFieldName);
            BO.TheGridColumn c = Clone2NewInstance(c0);

            BO.EntityRelation rel = applicable_rels.Where(p => p.RelName == strRelName).First();
            c.RelName = strRelName;
            c.RelSql = rel.SqlFrom;
            if (rel.RelNameDependOn != null)
            {
                c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
            }

            if (c.NotShowRelInHeader == true)
            {
                return c;   //nezobrazovat u sloupce název relace
            }

            if (bolComboColumns)
            {
                c.Header = rel.AliasSingular;
            }
            else
            {
                c.Header = c.Header + " [" + rel.AliasSingular + "]";

            }


            return c;
        }
        public BO.TheGridColumn ByUniqueName(string strUniqueName)
        {
            if (_lis.Where(p => p.UniqueName == strUniqueName).Count() > 0)
            {
                return _lis.Where(p => p.UniqueName == strUniqueName).First();
            }
            else
            {
                return null;
            }
        }
        private BO.TheGridColumn Clone2NewInstance(BO.TheGridColumn c)
        {
            return new BO.TheGridColumn()
            {
                Entity = c.Entity,
                EntityAlias = c.EntityAlias,
                Field = c.Field,
                FieldType = c.FieldType,
                FixedWidth = c.FixedWidth,
                Header = c.Header,
                SqlSyntax = c.SqlSyntax,
                IsFilterable = c.IsFilterable,
                IsShowTotals = c.IsShowTotals,
                IsTimestamp = c.IsTimestamp,
                RelName = c.RelName,
                RelSql = c.RelSql,
                RelSqlDependOn = c.RelSqlDependOn,
                RelSqlInCol = c.RelSqlInCol,
                NotShowRelInHeader = c.NotShowRelInHeader,
                TranslateLang1 = c.TranslateLang1,
                TranslateLang2 = c.TranslateLang2,
                TranslateLang3 = c.TranslateLang3
            };

        }

        
        public List<BO.TheGridColumn> ParseTheGridColumns(string strPrimaryPrefix, string strJ72Columns, int intLangIndex)
        {
            //v strJ72Columns je čárkou oddělený seznam sloupců z pole j72Columns: název relace+__+entita+__+field
            List<BO.EntityRelation> applicable_rels = _ep.getApplicableRelations(strPrimaryPrefix);
            List<string> sels = BO.BAS.ConvertString2List(strJ72Columns, ",");
            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();

            string[] arr;

            for (int i = 0; i < sels.Count; i++)
            {
                arr = sels[i].Split("__");
                if (_lis.Exists(p => p.Entity == arr[1] && p.Field == arr[2]))
                {
                    BO.TheGridColumn c = CreateNewInstanceColumn(_lis.Where(p => p.Entity == arr[1] && p.Field == arr[2]).First(), arr, intLangIndex, applicable_rels);

                    if ((i == sels.Count - 1) && (c.FieldType == "num" || c.FieldType == "num0" || c.FieldType == "num3"))
                    {
                        c.CssClass = "tdn_lastcol";
                    }
                    ret.Add(c);

                }

            }

            return ret;

        }

        public List<BO.TheGridColumn> ParseTheGridColumns(string strPrimaryPrefix, string strJ72Columns, int intLangIndex, List<BO.TheGridColumn> lisFFs)
        {
            //v strJ72Columns je čárkou oddělený seznam sloupců z pole j72Columns: název relace+__+entita+__+field
            //v lisFFs se předávají další sloupce
            List<BO.EntityRelation> applicable_rels = _ep.getApplicableRelations(strPrimaryPrefix);
            List<string> sels = BO.BAS.ConvertString2List(strJ72Columns, ",");
            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();

            string[] arr;
            for (int i = 0; i < sels.Count(); i++)
            {
                arr = sels[i].Split("__");
                BO.TheGridColumn colSource = null;
                if (sels[i].Contains("Free"))
                {
                    if (lisFFs.Exists(p => p.Entity == arr[1] && p.Field == arr[2]))
                    {
                        colSource = lisFFs.Where(p => p.Entity == arr[1] && p.Field == arr[2]).First();
                    }
                }
                else
                {
                    if (_lis.Exists(p => p.Entity == arr[1] && p.Field == arr[2]))
                    {
                        colSource = _lis.Where(p => p.Entity == arr[1] && p.Field == arr[2]).First();
                    }
                }

                if (colSource != null)
                {
                    BO.TheGridColumn c = CreateNewInstanceColumn(colSource, arr, intLangIndex, applicable_rels);

                    if ((i == sels.Count - 1) && (c.FieldType == "num" || c.FieldType == "num0" || c.FieldType == "num3"))
                    {
                        c.CssClass = "tdn_lastcol";
                    }
                    ret.Add(c);
                }

            }

            return ret;

        }

        private BO.TheGridColumn CreateNewInstanceColumn(BO.TheGridColumn colSource, string[] arr, int intLangIndex, List<BO.EntityRelation> applicable_rels)
        {
            BO.TheGridColumn c = Clone2NewInstance(colSource);
            switch (intLangIndex)
            {
                case 1:
                    c.Header = c.TranslateLang1;
                    break;
                case 2:
                    c.Header = c.TranslateLang2;
                    break;
                case 3:
                    c.Header = c.TranslateLang3;
                    break;
                default:
                    c.Header = c.Header;
                    break;
            }
            if (arr[0] == "a")
            {
                c.RelName = null;
            }
            else
            {
                c.RelName = arr[0]; //název relace v sql dotazu
                BO.EntityRelation rel = applicable_rels.Where(p => p.RelName == c.RelName).First();
                c.RelSql = rel.SqlFrom;    //sql klauzule relace    
                if (c.NotShowRelInHeader == false)
                {
                    c.Header = c.Header + " [" + rel.AliasSingular + "]";   //zobrazovat název entity v záhlaví sloupce                           
                }


                if (rel.RelNameDependOn != null)
                {
                    c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
                }
            }



            return c;
        }

        public List<BO.TheGridColumnFilter> ParseAdhocFilterFromString(string strJ72Filter, IEnumerable<BO.TheGridColumn> explicit_cols)
        {
            var ret = new List<BO.TheGridColumnFilter>();
            if (String.IsNullOrEmpty(strJ72Filter) == true) return ret;


            List<string> lis = BO.BAS.ConvertString2List(strJ72Filter, "$$$");
            foreach (var s in lis)
            {
                List<string> arr = BO.BAS.ConvertString2List(s, "###");
                if (explicit_cols.Where(p => p.UniqueName == arr[0]).Count() > 0)
                {
                    var c = new BO.TheGridColumnFilter() { field = arr[0], oper = arr[1], value = arr[2] };
                    c.BoundColumn = explicit_cols.Where(p => p.UniqueName == arr[0]).First();
                    ParseFilterValue(ref c);
                    ret.Add(c);
                }


            }
            return ret;
        }

        private void ParseFilterValue(ref BO.TheGridColumnFilter col)
        {

            {
                if (col.value.Contains("|"))
                {
                    var a = col.value.Split("|");
                    col.c1value = a[0];
                    col.c2value = a[1];
                }
                else
                {
                    col.c1value = col.value;
                    col.c2value = "";
                }
                switch (col.oper)
                {
                    case "1":
                        {
                            col.value_alias = "Je prázdné";
                            break;
                        }

                    case "2":
                        {
                            col.value_alias = "Není prázdné";
                            break;
                        }

                    case "3":  // obsahuje
                        {
                            col.value_alias = col.c1value;
                            break;
                        }

                    case "5":  // začíná na
                        {

                            col.value_alias = "[*=] " + col.c1value;
                            break;
                        }

                    case "6":  // je rovno
                        {
                            col.value_alias = "[=] " + col.c1value;
                            break;
                        }

                    case "7":  // není rovno
                        {
                            col.value_alias = "[<>] " + col.c1value;
                            break;
                        }

                    case "8":
                        {
                            col.value_alias = "ANO";
                            break;
                        }

                    case "9":
                        {
                            col.value_alias = "NE";
                            break;
                        }

                    case "10": // je větší než nula
                        {
                            col.value_alias = "větší než 0";
                            break;
                        }

                    case "11":
                        {
                            col.value_alias = "0 nebo prázdné";
                            break;
                        }

                    case "4":  // interval
                        {

                            if (col.BoundColumn.FieldType == "date" | col.BoundColumn.FieldType == "datetime")
                            {
                                col.value_alias = col.c1value + " - " + col.c2value;   // datum
                            }
                            else
                            {
                                col.value_alias = col.c1value + " - " + col.c2value;
                            }    // číslo

                            break;
                        }
                }





            }


        }


    }
}
