using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class AdminController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;
        public AdminController(BL.TheColumnsProvider cp)
        {            
            _colsProvider = cp;
        }
        //-----------Začátek GRID událostí-------------
        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi, List<BO.StringPair> pathpars, List<BO.TheGridColumnFilter> filter) //TheGrid povinná metoda: sloupcový filtr
        {
            
            var c = new UI.TheGridSupport(GetGridInput(tgi.entity,tgi.prefix,0, tgi.viewstate), Factory, _colsProvider);
            
            return c.Event_HandleTheGridFilter(tgi, filter);

        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi, List<BO.StringPair> pathpars)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            var c = new UI.TheGridSupport(GetGridInput(tgi.entity,tgi.prefix,0, tgi.viewstate), Factory, _colsProvider);
            return c.Event_HandleTheGridOper(tgi);

        }
        public string HandleTheGridMenu(TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda: zobrazení grid menu
        {
            var c = new UI.TheGridSupport(GetGridInput(tgi.entity,tgi.prefix,0, tgi.viewstate), Factory, _colsProvider);
            return c.Event_HandleTheGridMenu(tgi.j72id);
        }
        public TheGridExportedFile HandleTheGridExport(string format, string pids, TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda pro export dat
        {           
            var c = new UI.TheGridSupport(GetGridInput(tgi.entity,tgi.prefix,0, tgi.viewstate), Factory, _colsProvider);
            return c.Event_HandleTheGridExport(format, tgi.j72id, pids);
        }
        //-----------Konec GRID událostí-------------
        

        public IActionResult LogAsUser(string login, string code)
        {
            var v = new AdminLogAsUser() { Login = login, Code = code };

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }
        [HttpPost]
        public IActionResult LogAsUser(AdminLogAsUser v)
        {
            if (ModelState.IsValid)
            {

                if (string.IsNullOrEmpty(v.Login) || string.IsNullOrEmpty(v.Code))
                {
                    this.AddMessage("Login i Ověřovací kód je povinné zadat."); return View(v);
                }
                var recJ03 = Factory.j03UserBL.LoadByLogin(v.Login, 0);
                if (recJ03 == null)
                {
                    this.AddMessage("Zadaný login neexistuje."); return View(v);
                }
                if (v.Code != Factory.x35GlobalParamBL.LoadParam("SuperPin","barbarossa"))
                {
                    this.AddMessage("Ověřovací kód není správný."); return View(v);
                }

                var strID = recJ03.j02Email;
                if (string.IsNullOrEmpty(strID)) { strID = "info@marktime.cz"; };
                var userClaims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, recJ03.j03Login),
                new Claim("access_token","inspis_core_token"),
                new Claim(ClaimTypes.Email, strID)
                 };

                var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });

                var xx = new AuthenticationProperties() { IsPersistent = true, ExpiresUtc = DateTime.Now.AddHours(1) };
                HttpContext.SignInAsync(userPrincipal, xx);

                if (recJ03 != null)
                {

                    return Redirect("/Dashboard/Index");
                    
                }

            }

            return View(v);
        }

        
        public IActionResult Users(string prefix, int go2pid)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid };
            handle_default_link(v, "Users", "j03");
            
            inhale_entity(ref v, v.prefix);
            v.gridinput = GetGridInput(v.entity,v.prefix,go2pid,BO.BAS.ConvertString2List("users"));
            
            return View(v);
        }
        public IActionResult Billing(string prefix, int go2pid)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid  };
            handle_default_link(v, "Billing", "p92");
            
            inhale_entity(ref v, v.prefix);
            v.gridinput = GetGridInput(v.entity,v.prefix,v.go2pid, BO.BAS.ConvertString2List("billing"));            

            return View(v);
        }
        public IActionResult Worksheet(string prefix, int go2pid)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid };
            handle_default_link(v, "Worksheet", "p32");
            
            inhale_entity(ref v, v.prefix);
            v.gridinput = GetGridInput(v.entity, v.prefix,v.go2pid, BO.BAS.ConvertString2List("worksheet"));

            return View(v);
        }
        public IActionResult Projects(string prefix, int go2pid)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid };
            handle_default_link(v, "Projects", "p42");
            
            inhale_entity(ref v, v.prefix);
            v.gridinput = GetGridInput(v.entity, v.prefix,v.go2pid, BO.BAS.ConvertString2List("projects"));
            v.gridinput.viewstate = "projects";

            return View(v);
        }
        public IActionResult Misc(string prefix, int go2pid)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid };
            handle_default_link(v,"Misc", "x38");

            inhale_entity(ref v, v.prefix);
            v.gridinput = GetGridInput(v.entity, v.prefix, v.go2pid, BO.BAS.ConvertString2List("misc"));
            v.gridinput.viewstate = "misc";

            return View(v);
        }
        public IActionResult Workflow(string prefix, int go2pid)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid };
            inhale_entity(ref v, prefix);
            v.gridinput = GetGridInput(v.entity,v.prefix,v.go2pid, BO.BAS.ConvertString2List("workflow"));
            return View(v);
        }

        private void handle_default_link(AdminPage v,string module,string defprefix)
        {
            if (v.prefix == null)
            {
                v.prefix = Factory.CBL.LoadUserParam("Admin/"+ module +"-prefix", defprefix);
            }
            else
            {
                if (Factory.CBL.LoadUserParam("Admin/"+ module +"-prefix") != v.prefix)
                {
                    Factory.CBL.SetUserParam("Admin/"+ module +"-prefix", v.prefix);
                }
            }
        }
        private void inhale_entity(ref AdminPage v, string prefix)
        {
            if (prefix != null)
            {
                var c = Factory.EProvider.ByPrefix(prefix);
                v.entity = c.TableName;
                v.entityTitleSingle = c.AliasSingular;

                switch (Factory.CurrentUser.j03LangIndex)
                {
                    case 1:
                        v.entityTitle = c.TranslateLang1;                        
                        break;
                    case 2:
                        v.entityTitle = c.TranslateLang2;
                        break;
                    default:
                        v.entityTitle = c.AliasPlural;
                        break;
                }
                
            }
        }

        //private void inhale_tree_o13(UI.Models.AdminPage v)
        //{
        //    v.treeNodes = new List<myTreeNode>();
        //    var lis = Factory.o13AttachmentTypeBL.GetList(new BO.myQuery("o13AttachmentType"));
        //    foreach (var rec in lis)
        //    {
        //        var c = new myTreeNode()
        //        {
        //            TreeIndex = rec.o13TreeIndex,
        //            TreeLevel = rec.o13TreeLevel,
        //            Text = rec.o13Name,
        //            TreeIndexFrom = rec.o13TreeIndexFrom,
        //            TreeIndexTo = rec.o13TreeIndexTo,
        //            Pid = rec.pid,
        //            ParentPid = rec.o13ParentID,
        //            Prefix = "o13",
        //            Expanded=true

        //        };

        //        v.treeNodes.Add(c);

        //    }
        //}
        


        public IActionResult System()
        {

            return View();
        }
        public BO.Result GenerateSpGenerateCreateUpdateScript(string scope)
        {
            var lis = Factory.FBL.GetList_SysObjects();
            if (scope == "_core")
            {
                lis = lis.Where(p => p.Name.StartsWith("_core"));
            }
            Factory.FBL.GenerateCreateUpdateScript(lis);

            return new BO.Result(false, "Soubor byl vygenerován (do TEMPu)");
        }

        private TheGridInput GetGridInput(string entity,string prefix,int go2pid,List<string> viewstate)
        {
            string strMyQueryInline = null;
            switch (prefix)
            {
                case "j02":
                    strMyQueryInline = "j02isintraperson@bool@1";
                    break;
            }

            var gi = new TheGridInput();
            gi.controllername = "Admin";
            gi.entity = entity;
            gi.go2pid = go2pid;
            gi.ondblclick = "handle_dblclick";

            gi.query = new BO.InitMyQuery().Load(prefix,null,0, strMyQueryInline);
            gi.query.IsRecordValid = null;
            
            gi.j72id = Factory.CBL.LoadUserParamInt("Admin/" + prefix + "-j72id");
            
            if (viewstate != null)
            {
                if (viewstate[0] == "projects" && prefix == "x67")
                {
                    gi.query.explicit_sqlwhere = "a.x29ID=141";
                }
            }
            
            

            return gi;
        }




    }
}