﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryJ02:baseQuery
    {
        public bool? j02isintraperson { get; set; }
        public bool? isvirtualperson { get; set; }
        public bool? isintranonvirtualperson { get; set; }  //interní a ne-virtuální osoby
        public bool? allowed_for_p31_entry { get; set; }    //osoby, za které přihlášený uživatel může vykazovat úkony
        public int j04id { get; set; }
        public int j11id { get; set; }
        public List<int> j11ids { get; set; }
        public int j07id { get; set; }
        public List<int> j07ids { get; set; }
        public int j18id { get; set; }
        public int p28id { get; set; }
        
        public int p41id { get; set; }
        public int p91id { get; set; }
        public string tabquery { get; set; }

        public myQueryJ02()
        {
            this.Prefix = "j02";            
        }

        public override List<QRow> GetRows()
        {
            if (this.p31statequery>0) this.Handle_p31StateQuery();
            Handle_Wip();

            if (this.global_d1 != null && this.global_d2 != null)
            {
                DateTime d1 = Convert.ToDateTime(this.global_d1); DateTime d2 = Convert.ToDateTime(this.global_d2);
                if (d1.Year > 1900 || d2.Year < 3000)
                {
                    switch (this.period_field)
                    {
                        case "j02DateInsert":
                            AQ("a.j02DateInsert BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", this.global_d2_235959);
                            break;                       
                        case "p91Date":
                            //AQ("a.j02ID IN (select xa.j02ID FROM p31Worksheet xa INNER JOIN p91Invoice xb ON xa.p91ID=xb.p91ID WHERE xb.p91Date BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            AQ("EXISTS (select 1 FROM p31Worksheet xa INNER JOIN p91Invoice xb ON xa.p91ID=xb.p91ID WHERE xa.j02ID=a.j02ID AND xb.p91Date BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                        case "p91DateSupply":
                            //AQ("a.j02ID IN (select xa.j02ID FROM p31Worksheet xa INNER JOIN p91Invoice xb ON xa.p91ID=xb.p91ID WHERE xb.p91DateSupply BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            AQ("EXISTS (select 1 FROM p31Worksheet xa INNER JOIN p91Invoice xb ON xa.p91ID=xb.p91ID WHERE xa.j02ID=a.j02ID AND xb.p91DateSupply BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                        case "p31Date":
                        default:
                            //AQ("a.j02ID IN (select j02ID FROM p31Worksheet WHERE p31Date BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            AQ("EXISTS (select 1 FROM p31Worksheet WHERE j02ID=a.j02ID AND p31Date BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                    }
                }
            }
            if (this.tabquery != null)
            {
                switch (this.tabquery)
                {
                    case "internal":
                        this.j02isintraperson=true; break;
                    case "contact":
                        this.j02isintraperson = false; break;                    
                }
            }

            if (this.j02isintraperson != null)
            {
                AQ("a.j02IsIntraPerson=@isintra", "isintra", this.j02isintraperson);
            }
            if (this.isintranonvirtualperson != null && this.isintranonvirtualperson==true)
            {
                AQ("a.j02IsIntraPerson=1 AND a.j02VirtualParentID IS NULL", null, null);
            }
            
            if (!this.CurrentUser.IsAdmin)
            {
                if (this.allowed_for_p31_entry == true)
                {
                    if (this.CurrentUser.IsMasterPerson)
                    {
                        string s = "(a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_me AND j05IsCreate_p31=1)";
                        s += " OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_me AND xj05.j05IsCreate_p31=1)";
                        s += " OR a.j02ID=@j02id_me)";
                        AQ(s, "j02id_me", this.CurrentUser.j02ID);
                    }
                    else
                    {
                        AQ("a.j02ID=@j02id_me", "j02id_me", this.CurrentUser.j02ID);
                    }
                }
                if (this.MyRecordsDisponible)
                {
                    if (this.CurrentUser.IsMasterPerson)
                    {
                        string s = "(a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_me)";
                        s += " OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_me)";
                        s += " OR a.j02ID=@j02id_me OR a.j02IsIntraPerson=0)";
                        AQ(s, "j02id_me", this.CurrentUser.j02ID);
                    }
                    else
                    {
                        AQ("a.j02ID=@j02id_me", "j02id_me", this.CurrentUser.j02ID);
                    }
                }
            }

            if (this.isvirtualperson != null)
            {
                if (this.isvirtualperson == true)
                {
                    AQ("a.j02VirtualParentID IS NOT NULL", null, null);
                }
                else
                {
                    AQ("a.j02VirtualParentID IS NULL", null, null);
                }               
            }
            if (this.j04id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM j03User WHERE j04ID=@j04id)", "j04id", this.j04id);
            }            
            if (this.j11id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM j12Team_Person WHERE j11ID=@j11id)", "j11id", this.j11id);
            }
            if (this.j11ids != null && this.j11ids.Count > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM j12Team_Person WHERE j11ID IN ("+ string.Join(",", this.j11ids) + "))", null, null);
            }
            if (this.j07id > 0)
            {
                AQ("a.j07ID=@j07id", "j07id", this.j07id);
            }
            if (this.j07ids !=null && this.j07ids.Count > 0)
            {
                AQ("a.j07ID IN (" + string.Join(",", this.j07ids)+")", null, null);
            }
            if (this.j18id > 0)
            {
                AQ("a.j18ID=@j18id", "j18id", this.j18id);
            }
            if (this.p41id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM p30Contact_Person WHERE p41ID=@p41id OR p28ID IN (SELECT p28ID_Client FROM p41Project WHERE p41ID=@p41id AND p28ID_Client IS NOT NULL))", "p41id", this.p41id);
            }
            
            if (this.p91id > 0)
            {
                //AQ("a.j02ID IN (select a1.j02ID FROM p30Contact_Person a1 INNER JOIN p91Invoice a2 ON a1.p28ID=a2.p28ID WHERE a2.p91ID=@p91id)", "p91id", this.p91id);
                AQ("a.j02ID IN (select j02ID FROM p31Worksheet WHERE p91ID=@p91id)", "p91id", this.p91id);
            }
           

            if (_searchstring != null && _searchstring.Length > 2)
            {                
                AQ("(a.j02firstname like @expr+'%' OR a.j02LastName LIKE '%'+@expr+'%' OR a.j02Email LIKE '%'+@expr+'%' OR a.j02ID IN (select j02ID FROM j03User where j03Login LIKE '%'+@expr+'%'))", "expr", _searchstring);

            }

            if (this.p28id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM p30Contact_Person WHERE p28ID=@p28id OR p41ID IN (SELECT p41ID FROM p41Project WHERE p28ID_Client=@p28id))", "p28id", this.p28id);
            }

            if (this.iswip != null)
            {
                if (this.iswip == true)
                {
                    AQ("EXISTS (select 1 FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xa.j02ID=a.j02ID AND xa.p71ID IS NULL AND xa.p91ID IS NULL AND xa.p31Date between @p31date1 AND @p31date2 AND xb.p41BillingFlag<99)", null, null);
                }
                else
                {
                    AQ("NOT EXISTS (select 1 FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xa.j02ID=a.j02ID AND xa.p71ID IS NULL AND xa.p91ID IS NULL AND xa.p31Date between @p31date1 AND @p31date2 AND xb.p41BillingFlag<99)", null, null);
                }
            }
            if (this.isapproved_and_wait4invoice != null)
            {
                if (this.isapproved_and_wait4invoice == true)
                {
                    AQ("EXISTS (select 1 FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xa.j02ID=a.j02ID AND xa.p71ID=1 AND xa.p72ID_AfterApprove=4 AND xa.p91ID IS NULL AND xa.p31Date between @p31date1 AND @p31date2 AND xb.p41BillingFlag<99)", null, null);
                }
                else
                {
                    AQ("NOT EXISTS (select 1 FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xa.j02ID=a.j02ID AND xa.p71ID=1 AND xa.p72ID_AfterApprove=4 AND xa.p91ID IS NULL AND xa.p31Date between @p31date1 AND @p31date2 AND xb.p41BillingFlag<99)", null, null);
                }
            }

            

            return this.InhaleRows();

        }

        private void Handle_Wip()
        {
            if (this.iswip != null)
            {
                if (this.iswip == true)
                {
                    AQ("a.j02ID IN (select j02ID FROM p31Worksheet WHERE p71ID IS NULL AND p91ID IS NULL AND p31Date between @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE())", null, null);
                }
                else
                {
                    AQ("a.j02ID NOT IN (select j02ID FROM p31Worksheet WHERE p71ID IS NULL AND p91ID IS NULL AND p31Date between @p31date1 AND @p31date2)", null, null);
                }
            }

            if (this.isapproved_and_wait4invoice != null)
            {
                if (this.isapproved_and_wait4invoice == true)
                {
                    AQ("a.j02ID IN (select za.j02ID FROM p31Worksheet za INNER JOIN p41Project zb ON za.p41ID=zb.p41ID WHERE za.p71ID=1 AND za.p91ID IS NULL AND za.p31Date between @p31date1 AND @p31date2 AND zb.p41BillingFlag<99)", null, null);
                }
                else
                {
                    AQ("a.j02ID IN (select j02ID FROM p31Worksheet WHERE p91ID IS NULL AND p31Date between @p31date1 AND @p31date2)", null, null);
                }
            }
        }


        
    }
}
