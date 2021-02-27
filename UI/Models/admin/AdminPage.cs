﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class AdminPage:BaseViewModel
    {
        public string area { get; set; }
        public string entity { get; set; }
        public string entityTitle { get; set; }
        public string entityTitleSingle { get; set; }
        public string prefix { get; set; }
        
        public int go2pid { get; set; }
        public int contextmenuflag { get; set; }

        public string master_entity { get; set; }
        public int master_pid { get; set; }
        
        public List<NavTab> NavTabs;

        public string go2pid_url_in_iframe { get; set; }

        public string dblclick { get; set; } = "tg_dblclick";

        public string TreeState { get; set; }

        public List<UI.Models.myTreeNode> treeNodes { get; set; }

        public TheGridInput gridinput { get; set; }
    }
}
