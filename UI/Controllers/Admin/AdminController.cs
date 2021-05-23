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
            
            var c = new UI.TheGridSupport(GetGridInput(tgi.entity,tgi.prefix,0, tgi.viewstate,tgi.myqueryinline), Factory, _colsProvider);
            
            return c.Event_HandleTheGridFilter(tgi, filter);

        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi, List<BO.StringPair> pathpars)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            var c = new UI.TheGridSupport(GetGridInput(tgi.entity,tgi.prefix,0, tgi.viewstate,tgi.myqueryinline), Factory, _colsProvider);
            return c.Event_HandleTheGridOper(tgi);

        }
        public string HandleTheGridMenu(TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda: zobrazení grid menu
        {
            var c = new UI.TheGridSupport(GetGridInput(tgi.entity,tgi.prefix,0, tgi.viewstate,tgi.myqueryinline), Factory, _colsProvider);
            return c.Event_HandleTheGridMenu(tgi.j72id);
        }
        public TheGridExportedFile HandleTheGridExport(string format, string pids, TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda pro export dat
        {           
            var c = new UI.TheGridSupport(GetGridInput(tgi.entity,tgi.prefix,0, tgi.viewstate,tgi.myqueryinline), Factory, _colsProvider);
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

        
        public IActionResult Index(bool? signpost)
        {
            if (signpost != null)
            {
                if (signpost==true)
                {   //odkaz do administrace z hlavního menu -> najít naposledy zobrazovanou sekci admin stránek
                    string strArea = Factory.CBL.LoadUserParam("Admin/last-area");
                    if (strArea != null && strArea !="index")
                    {
                        return RedirectToAction("Page", new { area = strArea });
                    }
                }
                else
                {
                    //kliknutí na [Úvod] v administraci
                    Factory.CBL.SetUserParam("Admin/last-area", "index");
                }
            }
            
            var v = new AdminHome();
            v.lisP87 = Factory.FBL.GetListP87().ToList();
            v.lisP07 = Factory.p07ProjectLevelBL.GetList(new BO.myQuery("p07") { IsRecordValid = null });
            return View(v);
        }
        public IActionResult Page(string area,string prefix, int go2pid, string myqueryinline)
        {
            var v = new AdminPage() {area=area, prefix = prefix, go2pid = go2pid };
            if (area !=null && prefix == null)
            {
                if (Factory.CBL.LoadUserParam("Admin/last-area") != area)
                {
                    Factory.CBL.SetUserParam("Admin/last-area", area);
                }
            }
            string defprefix = null;
            switch (area)
            {
                case "users":defprefix = "j03";break;
                case "projects": defprefix = "p42"; break;
                case "billing": defprefix = "p92"; break;
                case "proforma": defprefix = "p89"; break;
                case "clients": defprefix = "p29"; break;
                case "worksheet": defprefix = "p32"; break;
                case "docs": defprefix = "x18"; break;
                case "tasks": defprefix = "p57"; break;
                case "tags": defprefix = "o53"; break;
                case "misc": defprefix = "x38"; break;
                case "index":
                    return RedirectToAction("Index");

            }
            handle_default_link(v, area, defprefix,ref myqueryinline);
            
            inhale_entity(v, v.prefix);
            v.gridinput = GetGridInput(v.entity, v.prefix, go2pid, null, myqueryinline);

            return View(v);
        }
        public IActionResult CompanyLogo()
        {
            var v = new AdminCompanyLogo() { UploadGuidLogo = BO.BAS.GetGuid(),IsMakeResize=true };           
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompanyLogo(AdminCompanyLogo v)
        {
            if (ModelState.IsValid)
            {
                
                if (Factory.o27AttachmentBL.GetTempFiles(v.UploadGuidLogo).Count() == 0)
                {
                    this.AddMessage("Pro změnu loga musíte nahrát soubor grafického loga.");
                    return View(v);
                }
                else
                {
                    var tempfile = Factory.o27AttachmentBL.GetTempFiles(v.UploadGuidLogo).First();
                    if (!(tempfile.o27FileExtension == ".png" || tempfile.o27FileExtension==".jpg" || tempfile.o27FileExtension == ".gif" || tempfile.o27FileExtension == ".jpeg"))
                    {
                        this.AddMessage("Jako grafické logo lze nahrát pouze PNG, JPG nebo GIF soubor.");
                        return View(v);
                    }
                    var strOrigFileName = "company_logo_original" + tempfile.o27FileExtension;                    
                    System.IO.File.Copy(tempfile.FullPath, Factory.App.WwwRootFolder + "\\Plugins\\" + strOrigFileName, true);

                    var strDestFileName = "company_logo";
                    if (Factory.App.IsCloud)
                    {
                        strDestFileName = BO.BAS.ParseDbNameFromCloudLogin(Factory.CurrentUser.j03Login) + "_logo";
                    }                                        
                    var files2delete = BO.BASFILE.GetFileListFromDir(Factory.App.WwwRootFolder + "\\Plugins", strDestFileName + ".*", System.IO.SearchOption.TopDirectoryOnly, true);
                    foreach (string file2delete in files2delete)
                    {
                        System.IO.File.Delete(file2delete); //odstranit stávající logo soubory
                    }

                    strDestFileName += tempfile.o27FileExtension;

                    if (v.IsMakeResize)
                    {
                        basUI.ResizeImage(tempfile.FullPath, Factory.App.WwwRootFolder + "\\Plugins\\" + strDestFileName, 250, 100);
                    }
                    else
                    {
                        System.IO.File.Copy(tempfile.FullPath, Factory.App.WwwRootFolder + "\\Plugins\\" + strDestFileName, true);
                    }
                    
                    
                    

                    v.SetJavascript_CallOnLoad(1);
                    return View(v);
                }
               
            }
            return View(v);
        }

        private void handle_default_link(AdminPage v,string module,string defprefix,ref string myqueryinline)
        {
            if (v.prefix == null)
            {
                v.prefix = Factory.CBL.LoadUserParam($"Admin/{module}-prefix", defprefix);
                myqueryinline= Factory.CBL.LoadUserParam($"Admin/{module}-{v.prefix}-myqueryinline");
            }
            else
            {
                if (Factory.CBL.LoadUserParam($"Admin/{module}-prefix") != v.prefix)
                {
                    Factory.CBL.SetUserParam($"Admin/{module}-prefix", v.prefix);
                    Factory.CBL.SetUserParam($"Admin/{module}-{v.prefix}-myqueryinline", myqueryinline);
                }
            }
        }
        private void inhale_entity(AdminPage v, string prefix)
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

        private TheGridInput GetGridInput(string entity,string prefix,int go2pid,List<string> viewstate,string myqueryinline)
        {
            string strMyQueryInline = null;
            switch (prefix)
            {
                case "j02":
                    strMyQueryInline = "j02isintraperson|bool|1";
                    break;
            }
            if (!string.IsNullOrEmpty(myqueryinline))
            {
                if (strMyQueryInline == null)
                {
                    strMyQueryInline = myqueryinline;
                }
                else
                {
                    strMyQueryInline += "|"+myqueryinline;
                }
                
            }

            var gi = new TheGridInput();
            gi.controllername = "Admin";
            gi.entity = entity;
            gi.go2pid = go2pid;
            gi.ondblclick = "handle_dblclick";
            gi.myqueryinline = strMyQueryInline;

            gi.query = new BO.InitMyQuery().Load(prefix,null,0, strMyQueryInline);
            gi.query.IsRecordValid = null;
            
            gi.j72id = Factory.CBL.LoadUserParamInt("Admin/" + prefix + "-j72id");
            
            //if (viewstate != null)
            //{
            //    if (viewstate[0] == "projects" && prefix == "x67")
            //    {
            //        gi.query.explicit_sqlwhere = "a.x29ID=141";
            //    }
            //}
            
            

            return gi;
        }




    }
}