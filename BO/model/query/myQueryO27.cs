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
                
        public int recpid { get; set; }
        public string tempguid { get; set; }


        public myQueryO27()
        {
            this.Prefix = "o27";
        }

        public override List<QRow> GetRows()
        {
            switch (this.x29id)
            {
                case 931:
                    this.x31id = this.recpid; break;
                case 607:
                    this.b07id = this.recpid; break;
                case 940:
                    this.x40id = this.recpid; break;
            }

            if (this.o13id > 0)
            {
                AQ("a.o13ID=@o13id", "o13id", this.o13id);
            }

            if (this.x29id > 0)
            {
                AQ("a.o13ID IN (select o13ID FROM o13AttachmentType WHERE x29ID=@x29id)", "x29id", this.x29id);

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

            if (this.tempguid != null)
            {
                AQ("a.o27ID NOT IN (select p85DataPID FROM p85Tempbox WHERE p85Guid=@tempguid)", "tempguid", this.tempguid);
                
            }


            return this.InhaleRows();

        }
    }
}
