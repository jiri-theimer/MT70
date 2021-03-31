using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class FreeFieldsViewModel
    {
        public string elementidprefix { get; set; } = "ff1.";
        public List<BO.FreeFieldInput> inputs { get; set; }
       
        public void SetupInputs(IEnumerable<BO.x28EntityField> lisX28, System.Data.DataTable vals)
        {
            this.inputs = new List<BO.FreeFieldInput>();

            foreach(var recX28 in lisX28)
            {
                var c = new BO.FreeFieldInput() { x28Field = recX28.x28Field,x28Name=recX28.x28Name,TypeName=recX28.TypeName,
                    x29ID=recX28.x29ID,x24ID=recX28.x24ID,x28IsRequired=recX28.x28IsRequired,
                    x28DataSource=recX28.x28DataSource,x28IsFixedDataSource=recX28.x28IsFixedDataSource
                };             
                
                this.inputs.Add(c);
            }

            if (vals != null && vals.Rows.Count>0)
            {
                System.Data.DataRow dbRow = vals.Rows[0];
                foreach(var c in this.inputs)
                {
                    if (dbRow[c.x28Field] != System.DBNull.Value)
                    {
                        switch (c.TypeName)
                        {
                            case "boolean":
                                c.CheckInput = Convert.ToBoolean(dbRow[c.x28Field]);
                                break;
                            case "date":
                            case "datetime":
                                c.DateInput = Convert.ToDateTime(dbRow[c.x28Field]);
                                break;
                            case "decimal":
                            case "integer":
                                c.NumInput = Convert.ToDouble(dbRow[c.x28Field]);
                                break;
                            default:
                                c.StringInput = Convert.ToString(dbRow[c.x28Field]);
                                break;
                        }
                    }
                    
                        
                }
            }
        }
    }
}
