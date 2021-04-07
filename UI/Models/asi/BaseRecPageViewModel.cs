using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class BaseRecPageViewModel:BaseViewModel
    {
        public BL.Factory Factory;
        public string prefix { get; set; }
        public string TagHtml { get; set; }

        public int pid { get; set; }

        public int pid_loaded { get; set; }
        public List<NavTab> NavTabs;

        public string DefaultNavTabUrl { get; set; }

        public string MenuCode { get; set; }
        public string Go2GridUrl { get; set; }

        public int SearchedPid { get; set; }
        public string SearchedText { get; set; }
        

        public void SetGridUrl()
        {
            if (this.pid > 0)
            {
                this.Go2GridUrl = $"/TheGrid/MasterView?prefix={this.prefix}&go2pid={this.pid}";
                if (!Factory.CBL.LoadUserParamBool("grid-j02-show11", true))
                {
                    this.Go2GridUrl = $"/TheGrid/FlatView?prefix={this.prefix}&go2pid={this.pid}";
                }
            }


        }
        public int LoadLastUsedPid()
        {
            this.pid_loaded= Factory.CBL.LoadUserParamInt($"{this.prefix}-RecPage-pid");
            return this.pid_loaded;
        }
        public void SaveLastUsedPid()
        {
            if (this.pid > 0 && this.pid_loaded !=this.pid)
            {
                Factory.CBL.SetUserParam($"{prefix}-RecPage-pid", this.pid.ToString());
            }
            
        }

        public void SetTagging()
        {
            var tg = Factory.o51TagBL.GetTagging(this.prefix, this.pid);
            
            this.TagHtml = tg.TagHtml;
        }
    }
}
