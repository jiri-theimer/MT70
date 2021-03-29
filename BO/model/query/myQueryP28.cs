﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP28:baseQuery
    {
        public int j02id { get; set; }
        public int p29id { get; set; }
        public int b02id { get; set; }
        public int p51id { get; set; }        
        public int p28parentid { get; set; }
        public bool? canbe_supplier { get; set; }
        public bool? canbe_client { get; set; }
        
        public myQueryP28()
        {
            this.Prefix = "p28";
        }

        public override List<QRow> GetRows()
        {
            if (this.o51ids != null)
            {
                AQ(" AND a.p28ID IN (SELECT o52RecordPID FROM o52TagBinding WHERE x29ID=328 AND o51ID IN (" + String.Join(",", this.o51ids) + "))", null, null);
            }
            if (this.j02id > 0)
            {
                AQ("a.p28ID IN (SELECT p28ID FROM p30Contact_Person WHERE j02ID=@j02id)", "j02id", this.j02id);
            }
            if (this.p29id > 0)
            {
                AQ("a.p29ID=@p29id", "p29id", this.p29id);
            }
            if (this.b02id > 0)
            {
                AQ("a.b02ID=@b02id", "b02id", this.b02id);
            }
            if (this.p51id > 0)
            {
                AQ("a.p51ID_Billing=@p51id", "p51id", this.p51id);
            }
            //if (this.p29ids !=null)
            //{
            //    AQ("a.p29ID IN (" + string.Join(",", this.p29ids) + ")", null, null);
            //}
            //if (this.b02ids != null)
            //{
            //    AQ("a.b02ID IN (" + string.Join(",", this.b02ids) + ")", null, null);
            //}
            //if (this.p51ids != null)
            //{
            //    AQ("a.p51ID_Billing IN (" + string.Join(",", this.p51ids) + ")", null, null);
            //}
            if (this.p28parentid > 0)
            {
                AQ("a.p28ParentID=@parentpid", "parentpid", this.p28parentid);
            }

            if (this.canbe_client != null)
            {
                if (this.canbe_client == true)
                {
                    AQ("a.p28SupplierFlag IN (1,3)", null, null);
                }
                if (this.canbe_client == false)
                {
                    AQ("a.p28SupplierFlag = 2", null, null);
                }
            }

            if (this.canbe_supplier != null)
            {
                if (this.canbe_supplier == true)
                {
                    AQ("a.p28SupplierFlag IN (2,3)", null, null);
                }
                if (this.canbe_supplier == false)
                {
                    AQ("a.p28SupplierFlag = 1", null, null);
                }
            }


            if (!string.IsNullOrEmpty(_searchstring))
            {
                string s = "";
                if (_searchstring.Length == 1)
                {
                    //hledat pouze podle počátečních písmen
                    s = "a.p28Name Like @expr+'%' OR a.p28Code LIKE '%'+@expr+'%' OR a.p28CompanyShortName LIKE @expr+'%' OR a.p28CompanyName LIKE @expr+'%'";
                    s += " OR a.p28ID IN (select p30.p28ID FROM p30Contact_Person p30 INNER JOIN j02Person j02 ON p30.j02ID=j02.j02ID WHERE j02LastName LIKE @expr+'%')";
                }
                else
                {
                    //něco jako fulltext
                    s = "a.p28Name LIKE '%'+@expr+'%' OR a.p28CompanyShortName LIKE '%'+@expr+'%' OR a.p28CompanyName LIKE '%'+@expr+'%'";
                    if (_searchstring.Length >= 2)
                    {
                        s += " OR a.p28Code LIKE '%'+@expr+'%' OR a.p28RegID LIKE @expr+'%' OR a.p28VatID LIKE @expr+'%'";
                    }
                    s += " OR a.p28ID IN (select p30.p28ID FROM p30Contact_Person p30 INNER JOIN j02Person j02 ON p30.j02ID=j02.j02ID WHERE j02LastName LIKE '%'+@expr+'%')";
                }
                AQ(s, "expr", _searchstring);

            }

            

            return this.InhaleRows();

        }
    }
}