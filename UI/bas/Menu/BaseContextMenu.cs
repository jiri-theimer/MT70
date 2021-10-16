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

        

        public MenuItem AMI(string strName, string strUrl, string icon = null, string strParentID = null, string strID = null, string strTarget = null,string strAfterName=null)
        {
            var c = new MenuItem() { Url = strUrl, Target = strTarget, ID = strID, ParentID = strParentID, Icon = icon };
            c.Name = _f.tra(strName);
            if (strAfterName != null)
            {
                c.Name += " " + strAfterName;
            }
            _lis.Add(c);
            return c;
        }

        public MenuItem AMI_RecPage(string strName, string prefix,int pid)
        {
            //return AMI(strName, $"javascript:_location_replace_top('/{prefix}/RecPage?pid={pid}')", "k-i-layout");
            return AMI(strName, $"/Record/RecPage?prefix={prefix}&pid={pid}", "maps_home_work", null,null,"_top");
        }
        public MenuItem AMI_RecGrid(string strName, string prefix, int pid)
        {
            string s= "/TheGrid/MasterView?prefix="+prefix+"&go2pid=" + pid.ToString();
            if (!_f.CBL.LoadUserParamBool("grid-"+prefix+"-show11", true))
            {
                s = "/TheGrid/FlatView?prefix="+prefix+"&go2pid=" + pid.ToString();
            }
            return AMI(strName, s, "grid_on", null,null,"_top");
        }
        public MenuItem AMI_Clone(string prefix, int pid)
        {

            return AMI("Kopírovat", $"javascript:_clone('{prefix}',{pid})", "content_copy");
            
        }
        public MenuItem AMI_Report(string prefix,int pid, string parentmenuid = null)
        {
            return AMI("Tisková sestava", $"javascript: _window_open('/x31/ReportContext?pid={pid}&prefix={prefix}')", "print", parentmenuid);
        }
        public MenuItem AMI_SendMail(string prefix, int pid, string parentmenuid = "more")
        {
            return AMI("Odeslat zprávu", $"javascript: _window_open('/Mail/SendMail?x29id={BO.BASX29.GetInt(prefix)}&recpid={pid}',2)", "email", parentmenuid);
        }
        public MenuItem AMI_Doc(string prefix, int pid,string parentmenuid="more")
        {
            return AMI("Nový dokument", $"javascript: _window_open('/o23/Record?prefix={prefix}&pid=0&recpid={pid}')", "file_present", parentmenuid);            
        }
        public MenuItem AMI_Memo(string prefix, int pid, string parentmenuid = "more")
        {
            return AMI("Nová poznámka", $"javascript: _window_open('/b07/Record?prefix={prefix}&pid=0&recpid={pid}')", "speaker_notes", parentmenuid);
        }
        public MenuItem AMI_ChangeLog(string prefix, int pid, string parentmenuid = "more")
        {
            return AMI("CHANGE-LOG", $"javascript: _window_open('/Record/ChangeLog?prefix={prefix}&pid={pid}',2)", "k-i-stick", parentmenuid);
        }

        public void AMI_Vykazat(BO.p41Project rec,int p56id=0)
        {
            if (rec.isclosed || rec.p41IsDraft ||  !_f.CurrentUser.j04IsMenu_Worksheet)
            {
                return; //není oprávnění a podmínky pro vykazování úkonů v projektu
            }
            var lisP34 = _f.p34ActivityGroupBL.GetList_WorksheetEntryInProject(rec.pid, rec.p42ID, _f.CurrentUser.j02ID);
            if (lisP34.Count() > 0)
            {
                AMI("Vykázat úkon", null, "approval", null, "p31create");
                foreach (BO.p34ActivityGroup c in lisP34.Take(10))
                {
                    string strIcon = "more_time";
                    switch (c.p33ID)
                    {
                        case BO.p33IdENUM.Kusovnik:
                            strIcon = "handyman"; break;
                        case BO.p33IdENUM.PenizeBezDPH:
                        case BO.p33IdENUM.PenizeVcDPHRozpisu:
                            strIcon = "paid"; break;
                    }
                    string url = $"javascript: _window_open('/p31/Record?newrec_prefix=p41&newrec_pid={rec.pid}&p34id={c.pid}')";
                    if (p56id > 0)
                    {
                        url = $"javascript: _window_open('/p31/Record?newrec_prefix=p56&newrec_pid={p56id}&p34id={c.pid}')";
                    }
                    AMI(c.p34Name, url, strIcon, "p31create");
                }
            }
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
            _lis.Add(new MenuItem() { IsHeader = true, Name = BO.BAS.OM2(strName, 100) });
        }


    }
}
