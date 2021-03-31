using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public abstract class BaseContextMenu
    {
        protected BL.Factory _f;
        protected int pid { get; set; }
        private List<MenuItem> _lis;

        public BaseContextMenu(BL.Factory f,int pid)
        {
            _lis = new List<MenuItem>();
            _f = f;
        }

        public List<MenuItem> GetItems()
        {
            return _lis;
        }

        public MenuItem AMI(string strName, string strUrl, string icon = null, string strParentID = null, string strID = null, string strTarget = null)
        {
            var c = new MenuItem() { Name = _f.tra(strName), Url = strUrl, Target = strTarget, ID = strID, ParentID = strParentID, Icon = icon };
            _lis.Add(c);
            return c;
        }

        public MenuItem AMI_RecPage(string strName, string prefix,int pid)
        {
            return AMI(strName, $"javascript:_location_replace_top('/{prefix}/RecPage?pid={pid}')", "k-i-layout");
        }
        public MenuItem AMI_Clone(string prefix, int pid)
        {
            return AMI("Kopírovat", $"javascript:_clone('{prefix}',{pid}", "k-i-copy");
            
        }
        public MenuItem AMI_Report(string prefix,int pid)
        {
            return AMI("Tisková sestava", $"javascript: _window_open('/x31/ReportContext?pid={pid}&prefix={prefix}',2)", "k-i-print");
        }

        public void DIV(string strName = null, string strParentID = null)
        {
            _lis.Add(new MenuItem() { IsDivider = true, Name = BO.BAS.OM2(strName, 30), ParentID = strParentID });
        }
        public void DIV_TRANS(string strName = null)
        {
            _lis.Add(new MenuItem() { IsDivider = true, Name = BO.BAS.OM2(_f.tra(strName), 30) });
        }
        public void HEADER(string strName)
        {
            _lis.Add(new MenuItem() { IsHeader = true, Name = BO.BAS.OM2(strName, 100) + ":" });
        }
    }
}
