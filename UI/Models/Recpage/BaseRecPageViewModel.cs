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
        
        public int pid_loaded { get; set; }     //pid načtený z user parametrů
        public List<NavTab> NavTabs;

        public string DefaultNavTabUrl { get; set; }

        public string MenuCode { get; set; }
        public string Go2GridUrl { get; set; }

        public int SearchedPid { get; set; }
        public string SearchedText { get; set; }
        
        private string _PanelHeight { get; set; }
        private bool? _ShowGridPanel { get; set; }
        private bool? _ShowContextMenu { get; set; }

        public FreeFieldsViewModel ff1 { get; set; }

        public void SetGridUrl()
        {
            if (this.pid > 0)
            {
                this.Go2GridUrl = basUI.GetGridUrl(Factory, this.prefix, this.pid);                
            }


        }
        public int LoadLastUsedPid()
        {
            this.pid_loaded= Factory.CBL.LoadUserParamInt($"recpage-{this.prefix}-pid");
            return this.pid_loaded;
        }

        public void SaveLastUsedPid()
        {
            if (this.pid > 0 && this.pid_loaded !=this.pid)
            {
                Factory.CBL.SetUserParam($"recpage-{prefix}-pid", this.pid.ToString());
            }
            
        }

        public string PanelHeight {
            get
            {
                if (_PanelHeight == null)
                {
                    _PanelHeight = Factory.CBL.LoadUserParam($"recpage-{prefix}-panel-height", "200px");
                }
                return _PanelHeight;
            }
        }

        
        public bool ShowGridPanel
        {
            get{
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


        public void SetTagging()
        {
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
