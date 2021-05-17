using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class RecPageViewModel: BaseViewModel
    {
        public BL.Factory Factory;
        public string prefix { get; set; }        
        public int pid { get; set; }
        public string TabName { get; set; }
        public int pid_loaded { get; set; }     //pid načtený z user parametrů
        public List<NavTab> NavTabs;

        public string DefaultNavTabUrl { get; set; }

        public string MenuCode { get; set; }
        public string Go2GridUrl { get; set; }

        public int SearchedPid { get; set; }
        public string SearchedText { get; set; }

        private string _entity { get; set; }
        private bool? _ShowGridPanel { get; set; }
        private bool? _ShowContextMenu { get; set; }

        public TheGridInput gridinput { get; set; }

        public void SetGridUrl()
        {
            if (this.pid > 0)
            {
                this.Go2GridUrl = basUI.GetGridUrl(Factory, this.prefix, this.pid);
            }


        }
        public int LoadLastUsedPid()
        {
            this.pid_loaded = Factory.CBL.LoadUserParamInt($"recpage-{this.prefix}-pid");
            return this.pid_loaded;
        }

        public void SaveLastUsedPid()
        {
            if (this.pid > 0 && this.pid_loaded != this.pid)
            {
                Factory.CBL.SetUserParam($"recpage-{prefix}-pid", this.pid.ToString());
            }

        }

     
        public string entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = BO.BASX29.GetEntity(this.prefix);
                }
                return _entity;
            }
        }
        public bool ShowGridPanel
        {
            get
            {
                if (_ShowGridPanel == null)
                {
                    _ShowGridPanel = Factory.CBL.LoadUserParamBool($"recpage-{prefix}-panel-grid", true);
                }
                return Convert.ToBoolean(_ShowGridPanel);
            }
        }
        public bool ShowContextMenu
        {
            get
            {
                if (_ShowContextMenu == null)
                {
                    _ShowContextMenu = Factory.CBL.LoadUserParamBool($"recpage-{prefix}-panel-cm", true);
                }
                return Convert.ToBoolean(_ShowContextMenu);
            }
        }


       

        
    }
}
