using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryO27:baseQuery
    {
        public int o13id { get; set; }
        public int x40id { get; set; }
        public int x31id { get; set; }
        public int b07id { get; set; }
        public List<int> b07ids { get; set; }

        public int x29id { get; set; }
        public int recpid { get; set; }
        public string tempguid { get; set; }


        public myQueryO27()
        {
            this.Prefix = "o27";
        }

        public override List<QRow> GetRows()
        {
            

            if (this.o13id > 0)
            {
                AQ("a.o13ID=@o13id", "o13id", this.o13id);
            }

            if (this.x29id > 0 && this.recpid>0)
            {
                AQ("a.b07ID IN (select b07ID FROM b07Comment WHERE x29ID=@rec_x29id AND b07RecordPID=@rec_pid)","rec_x29id",this.x29id,"AND",null,null,"rec_pid",this.recpid);
            }
          
            if (this.x40id > 0)
            {                
                AQ("a.x40ID=@x40id", "x40id", this.x40id);
            }
            
            if (this.x31id > 0)
            {
                AQ( "a.x31ID=@x31id", "x31id", this.x31id);
            }
            if (this.b07id > 0)
            {
                AQ("a.b07ID=@b07id", "b07id", this.b07id);
            }
            if (this.b07ids !=null && this.b07ids.Count > 0)
            {
                AQ("a.b07ID IN ("+string.Join(",",this.b07ids)+")",null,null);
            }

            if (this.tempguid != null)
            {
                AQ("a.o27ID NOT IN (select p85DataPID FROM p85Tempbox WHERE p85Guid=@tempguid)", "tempguid", this.tempguid);
                
            }


            return this.InhaleRows();

        }
    }
}
