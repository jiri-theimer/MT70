using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP41 : baseQuery
    {
        public int p42id { get; set; }        
        public int p51id { get; set; }
        public int p07level { get; set; }
        public int j02id_owner { get; set; }
        public int j02id_contactperson { get; set; }
        public int p41parentid { get; set; }
        public int b02id { get; set; }
        public int j18id { get; set; }
        public int p61id { get; set; }
        public int p91id { get; set; }
        public int p28id { get; set; }
        public int j02id_for_p31_entry { get; set; }    //projekty, které jsou k dispozici pro zapisování úkonů pro uživatele j02id_for_p31_entry
        public myQueryP41(string prefix)
        {
            this.Prefix = prefix;
        }

        public override List<QRow> GetRows()
        {
            if (this.p31statequery > 0) this.Handle_p31StateQuery();

            if (this.global_d1 != null && this.global_d2 != null)
            {
                DateTime d1 = Convert.ToDateTime(this.global_d1); DateTime d2 = Convert.ToDateTime(this.global_d2);
                if (d1.Year > 1900 || d2.Year < 3000)
                {
                    switch (this.period_field)
                    {
                        case "p41DateInsert":
                            AQ("a.p41DateInsert BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", this.global_d2_235959);
                            break;
                        case "p41PlanFrom":
                            AQ("a.p41PlanFrom BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", this.global_d2_235959);
                            break;
                        case "p41PlanUntil":
                            AQ("a.p41PlanUntil BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", this.global_d2_235959);
                            break;
                        case "p91Date":
                            AQ("EXISTS (select 1 FROM p31Worksheet xa INNER JOIN p91Invoice xb ON xa.p91ID=xb.p91ID WHERE xa.p41ID=a.p41ID AND xb.p91Date BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                        case "p91DateSupply":
                            AQ("EXISTS (select 1 FROM p31Worksheet xa INNER JOIN p91Invoice xb ON xa.p91ID=xb.p91ID WHERE xa.p41ID=a.p41ID AND xb.p91DateSupply BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                        case "p31Date":
                        default:
                            AQ("EXISTS (select 1 FROM p31Worksheet WHERE p41ID=a.p41ID AND p31Date BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                    }
                }
            }
            if (this.p07level > 0)
            {
                AQ("p07x.p07Level=@p07level", "p07level", this.p07level);
            }
            if (this.p42id > 0)
            {
                AQ("a.p42ID=@p42id)", "p42id", this.p42id);
            }
            if (this.p51id > 0)
            {
                AQ("(a.p51ID_Billing=@p51id OR a.p51ID_Internal=@p51id)", "p51id", this.p51id);
            }
            if (this.j02id_owner > 0)
            {
                AQ("a.j02ID_Owner=@ownerid)", "ownerid", this.j02id_owner);
            }
            if (this.j02id_contactperson > 0)
            {
                AQ("(a.p41ID IN (SELECT p41ID FROM p30Contact_Person WHERE J02ID=@j02id_contactperson AND p41ID IS NOT NULL) OR a.p28ID_Client IN (SELECT p28ID FROM p30Contact_Person WHERE J02ID=@j02id_contactperson AND p28ID IS NOT NULL))", "j02id_contactperson", this.j02id_contactperson);
            }
            if (this.p41parentid > 0)
            {
                AQ("a.p41ParentID=@parentpid", "parentpid", this.p41parentid);
            }
            if (this.p61id > 0)
            {
                AQ("a.p32ID IN (SELECT p32ID FROM p62ActivityCluster_Item WHERE p61ID=@p61id)", "p61id", this.p61id);
            }
            if (this.b02id > 0)
            {
                AQ("a.b02ID=@b02id", "b02id", this.b02id);
            }
            if (this.j18id > 0)
            {
                AQ("a.j18ID=@j18id", "j18id", this.j18id);
            }
            if (this.p61id > 0)
            {
                AQ("a.p61ID=@p61id", "p61id", this.p61id);
            }
            if (this.p91id > 0)
            {
                AQ("a.p41ID IN (select p41ID FROM p31Worksheet WHERE p91ID=@p91id)", "p91id", this.p91id);
            }
            if (this.p28id > 0)
            {
                AQ("(a.p28ID_Client=@p28id OR a.p28ID_Billing=@p28id)", "p28id", this.p28id);
            }



            if (!string.IsNullOrEmpty(_searchstring))
            {
                string s = null;
                if (_searchstring.Length == 1)
                {
                    //hledat pouze podle počátečních písmen
                    s = "a.p41Name LIKE @expr+'%' OR a.p41Code LIKE '%'+@expr+'%' OR a.p41NameShort LIKE @expr+'%'";
                    s += " OR a.p28ID_Client IN (select p28ID FROM p28Contact WHERE p28client.p28Name LIKE @expr+'%' OR p28CompanyName LIKE @expr+'%')";
                }
                else
                {
                    //něco jako fulltext
                    s = "a.p41Name LIKE '%'+@expr+'%' OR a.p41Code LIKE '%'+@expr+'%' OR a.p41NameShort LIKE '%'+@expr+'%'";
                    s += " OR a.p28ID_Client IN (select p28ID FROM p28Contact WHERE p28Name LIKE '%'+@expr+'%' OR p28CompanyName LIKE '%'+@expr+'%')";
                }
                AQ(s, "expr", _searchstring);

            }


            Handle_MyDisponible();


            return this.InhaleRows();

        }

        private void Handle_MyDisponible()
        {
            if ((this.CurrentUser.IsAdmin || this.CurrentUser.TestPermission(x53PermValEnum.GR_P41_Owner) || this.CurrentUser.TestPermission(x53PermValEnum.GR_P41_Reader)))
            {
                return; //přístup ke všem projektům v systému
            }
            
            string s = "EXISTS (SELECT 1 FROM x73EntityRole_DelegationCache xa INNER JOIN x69EntityRole_Assign xb ON xa.x67ID_Master=xb.x67ID AND xa.x73MasterPid=xb.x69RecordPID INNER JOIN p41Project xc ON xa.x73SlavePid=xc.p41ID";
            s += " WHERE xa.x73SlavePrefix='p41'";
            if (this.p07level>0 || this.p42id > 0)
            {
                s += " AND xa.x73SlavePid=a.p41ID"; //projekty z pouze jedné vertikální úrovně
            }
            else
            {
                s += " AND (xc.p41ID=a.p41ID OR xc.p41TreeIndex BETWEEN a.p41TreePrev AND a.p41TreeNext)"; //v jednom přehledu projekty z více vertikálních úrovní
            }
            
            if (string.IsNullOrEmpty(this.CurrentUser.j11IDs))
            {
                s += " AND (xb.j02ID=@j02id_query";
            }
            else
            {
                s += $" AND (xb.j02ID=@j02id_query OR xb.j11ID IN ({this.CurrentUser.j11IDs})"; //přihlášený uživatel je také členem týmů
            }
            if (this.CurrentUser.IsMasterPerson)    //přihlášený uživatel má pod sebou podřízené uživatele
            {
                s += " OR xb.j02ID IN (select j02ID_Slave FROM j05MasterSlave WHERE j02ID_Slave IS NOT NULL AND j02ID_Master=@j02id_query)";
            }
            s += ")";
            s += ")";
            
            AQ(s, "j02id_query", this.CurrentUser.j02ID);
        }
    }
}
