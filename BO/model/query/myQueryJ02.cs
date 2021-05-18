using System;
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
        

        public myQueryJ02()
        {
            this.Prefix = "j02";
        }

        public override List<QRow> GetRows()
        {
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
           

            return this.InhaleRows();

        }
    }
}
