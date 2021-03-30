using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class FreeFieldsViewModel
    {
        
        public List<FreeFieldInput> inputs { get; set; }
       
        public void SetupInputs(IEnumerable<BO.x28EntityField> lisX28)
        {
            this.inputs = new List<FreeFieldInput>();

            foreach(var recX28 in lisX28)
            {
                var c = new FreeFieldInput() { x28Field = recX28.x28Field,TypeName=recX28.TypeName,x28Name=recX28.x28Name };
                this.inputs.Add(c);
            }
        }
    }
}
