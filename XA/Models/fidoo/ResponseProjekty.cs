using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.fidoo
{
    public class ResponseProjekty
    {
        public List<Project> projects { get; set; }
        public bool complete { get; set; }
    }

    public class Project
    {
        public string code { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string state { get; set; }
    }

    public class ResponseUpdateProject
    {
        public string info { get; set; }
    }

    public class ResponseAddProject
    {
        public string projectId { get; set; }
    }
}
