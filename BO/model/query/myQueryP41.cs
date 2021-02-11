﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP41:baseQuery
    {
        public int p42id { get; set; }
        public int p51id { get; set; }
        public int j02id_owner { get; set; }
        public int j02id_contactperson { get; set; }
        public int p41parentid { get; set; }
        public int b02id { get; set; }
        public int j18id { get; set; }
        public int p61id { get; set; }
        public int p91id { get; set; }
       
        public myQueryP41()
        {
            this.Prefix = "p41";
        }

        public override List<QRow> GetRows()
        {
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
                AQ("a.p91ID=@p61id", "p91id", this.p91id);
            }
            return this.InhaleRows();

        }
    }
}