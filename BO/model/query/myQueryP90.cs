using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP90:baseQuery
    {
        public int p28id { get; set; }
        public int p91id { get; set; }
        public int j27id { get; set; }

        public myQueryP90()
        {
            this.Prefix = "p90";
        }

        public override List<QRow> GetRows()
        {

            if (this.p91id > 0)
            {
                AQ("a.p90ID IN (SELECT zb.p90ID FROM p99Invoice_Proforma za INNER JOIN p82Proforma_Payment zb ON za.p82ID=zb.p82ID WHERE za.p91ID=@p91id)", "p91id", this.p91id);
            }
            if (this.p28id > 0)
            {
                AQ("a.p28ID=@p28id", "p28id", this.p28id);
            }
            if (this.j27id > 0)
            {
                AQ("a.j27ID=@j27id)", "j27id", this.j27id);
            }

            if (_searchstring != null && _searchstring.Length > 2)
            {
                AQ("(a.p90Code like '%'+@expr+'%' OR a.p90Text1 LIKE '%'+@expr+'%' OR a.p28ID IN (select p28ID FROM p28Contact WHERE p28Name like '%'+@expr+'%'))", "expr", _searchstring);

            }

            Handle_MyDisponible();

            return this.InhaleRows();

        }

        private void Handle_MyDisponible()
        {
            if ((this.CurrentUser.IsAdmin || this.CurrentUser.TestPermission(x53PermValEnum.GR_P90_Owner) || this.CurrentUser.TestPermission(x53PermValEnum.GR_P90_Reader)))
            {
                return; //přístup ke všem zálohám v systému
            }

            string s = "EXISTS (SELECT 1 FROM x73EntityRole_DelegationCache xa INNER JOIN x69EntityRole_Assign xb ON xa.x67ID_Master=xb.x67ID AND xa.x73MasterPid=xb.x69RecordPID";
            s += " WHERE xa.x73SlavePrefix='p90'";
            s += " AND xa.x73SlavePid=a.p90ID";

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
