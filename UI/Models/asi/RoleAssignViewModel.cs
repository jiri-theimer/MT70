using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class RoleAssignViewModel
    {
        public string elementidprefix { get; set; } = "roles.";
        public string RecPrefix { get; set; }
        public int RecPid { get; set; }

        public List<RoleAssignRepeator> lisRepeator { get; set; }

        public List<BO.x69EntityRole_Assign> getList4Save(BL.Factory f)
        {
            var lis = new List<BO.x69EntityRole_Assign>();
            foreach(var c in lisRepeator.Where(p=>!p.IsNone))
            {
                var cc = new BO.x69EntityRole_Assign() { x67ID = c.x67ID };
                if (c.IsAllPersons)
                {
                    cc.j11ID = f.j11TeamBL.LoadTeamOfAllPersons().pid;
                    lis.Add(cc);
                }
                else
                {
                   if (c.j02IDs != null)
                    {
                        foreach(int intJ02ID in BO.BAS.ConvertString2ListInt(c.j02IDs))
                        {
                            cc = new BO.x69EntityRole_Assign() { x67ID = c.x67ID,j02ID=intJ02ID };
                            lis.Add(cc);
                        }
                    }
                    if (c.j11IDs != null)
                    {
                        foreach (int intJ11ID in BO.BAS.ConvertString2ListInt(c.j11IDs))
                        {
                            cc = new BO.x69EntityRole_Assign() { x67ID = c.x67ID, j11ID = intJ11ID };
                            lis.Add(cc);
                        }
                    }
                }

                
            }

            return lis;
        }
    }

    public class RoleAssignRepeator
    {
        public int x67ID { get; set; }
        public string x67Name { get; set; }
        public string j02IDs { get; set; }
        public string Persons { get; set; }

        public string j11IDs { get; set; }
        public string Teams { get; set; }

        public bool IsAllPersons { get; set; }
        public bool IsNone { get; set; } = true;

        public string CssVisibility_j11 { get; set; } = "visible";
        public string CssVisibility_j02 { get; set; } = "visible";
        public string CssVisibility_all { get; set; } = "visible";

    }
}
