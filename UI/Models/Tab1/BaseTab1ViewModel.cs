using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab1
{
    public class BaseTab1ViewModel:BaseViewModel
    {
        public BL.Factory Factory;
        public string prefix { get; set; }
        public string caller { get; set; }
        public string TagHtml { get; set; }

        public int pid { get; set; }

        

        public FreeFieldsViewModel ff1 { get; set; }

        
        public void SetTagging()
        {
            if (this.pid == 0) return;
            if (this.caller != null)
            {
                if (this.prefix == "le5")
                {
                    Factory.CBL.SaveLastCallingRecPid("p41", this.pid, this.caller, false, false);    //úroveň le5 zapisovat jako p41
                }
                else
                {
                    Factory.CBL.SaveLastCallingRecPid(this.prefix, this.pid, this.caller, false, false);    //zapsat informaci o naposledy navštíveném záznamu
                }
                
            }
            
            var tg = Factory.o51TagBL.GetTagging(this.prefix, this.pid);

            this.TagHtml = tg.TagHtml;
        }

        public void SetFreeFields(int intRecTypePid)
        {
            if (this.ff1 == null)
            {
                this.ff1 = new FreeFieldsViewModel();
                this.ff1.InhaleFreeFieldsView(Factory, this.pid, this.prefix);

                if (this.ff1.VisibleInputsCount > 0)
                {
                    this.ff1.RefreshInputsVisibility(Factory, this.pid, this.prefix, intRecTypePid);
                }
            }
        }
    }
}
