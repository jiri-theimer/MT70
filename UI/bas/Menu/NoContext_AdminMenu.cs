using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class NoContext_AdminMenu:BaseNonContextMenu
    {
        public NoContext_AdminMenu(BL.Factory f, string area, string prefix) : base(f)
        {
            MenuItem c = AMI("Úvod", "/Admin/Index?signpost=false", "settings");
            c.CssClass = BO.BAS.IIFS(area == null, "topmenulink_active", "topmenulink");
            c = AMI("Správa uživatelů", aurl("users"), "manage_accounts");
            c.CssClass = tmclass("users", area);
            c = AMI("Vykazování úkonů", aurl("worksheet"), "schedule");
            c.CssClass = tmclass("worksheet", area);
            c = AMI("Projekty", aurl("projects"), "work");
            c.CssClass = tmclass("projects", area);
            c = AMI("Klienti", aurl("clients"), "business");
            c.CssClass = tmclass("clients", area);
            c = AMI("Vyúčtování", aurl("billing"), "receipt_long");
            c.CssClass = tmclass("billing", area);
            c = AMI("Zálohy", aurl("proforma"), "receipt");
            c.CssClass = tmclass("proforma", area);
            c = AMI("Štítky", aurl("tags"), "local_offer");
            c.CssClass = tmclass("tags", area);
            c = AMI("Dokumenty", aurl("docs"), "file_present");
            c.CssClass = tmclass("docs", area);
            c = AMI("Úkoly", aurl("tasks"), "task");
            c.CssClass = tmclass("tasks", area);
            c = AMI("Střediska", aurl("centres"), "category");
            c.CssClass = tmclass("centres", area);
            c = AMI("Různé", aurl("misc"), "miscellaneous_services");
            c.CssClass = tmclass("misc", area);

            //AMI("Globální parametry", "javascript: _window_open('/x35/x35Params',1)", "k-i-gear");

            switch (area)
            {
                case "projects":
                    Handle_AdminProjects(prefix); break;
                case "clients":
                    Handle_AdminClients(prefix); break;
                case "users":
                    Handle_AdminUsers(prefix); break;
                case "worksheet":
                    Handle_AdminWorksheet(prefix); break;
                case "billing":
                    Handle_AdminBilling(prefix); break;
                case "proforma":
                    Handle_AdminProforma(prefix); break;
                case "docs":
                    Handle_AdminDocs(prefix); break;
                case "tasks":
                    Handle_AdminTasks(prefix); break;
                case "tags":
                    Handle_AdminTags(prefix); break;
                case "centres":
                    Handle_AdminCentres(prefix); break;
                case "misc":
                    Handle_AdminMisc(prefix); break;

            }
        }

        private string tmclass(string area, string curarea)
        {
            if (area == curarea)
            {
                return "topmenulink_active";
            }
            else
            {
                return "topmenulink";
            }
        }
        private string aurl(string area, string prefix)
        {
            return "/Admin/Page?area=" + area + "&prefix=" + prefix;
        }
        private string aurl(string area, string prefix, string ocas)
        {
            return "/Admin/Page?area=" + area + "&prefix=" + prefix + "&" + ocas;
        }
        private string aurl(string area)
        {
            return "/Admin/Page?area=" + area;
        }
        private void handle_selected_item(string prefix)
        {
            
            if (prefix != null)
            {
                if (_lis.Where(p => p.Url != null && p.Url.Contains("=" + prefix)).Count() > 0)
                {
                    _lis.Where(p => p.Url != null && p.Url.Contains("=" + prefix)).First().IsActive = true;
                }
            }
        }

        private void Handle_AdminUsers(string prefix)
        {
            //DIV_TRANS("Správa uživatelů");
            AMI("Uživatelské účty", aurl("users", "j03"));
            AMI("Aplikační role", aurl("users", "j04"));
            DIV();
            AMI("Přihlásit se pod jinou identitou", "/Admin/LogAsUser");

            DIV_TRANS("Osobní profily");
            AMI("Osobní profily", aurl("users", "j02"));
            AMI("Pozice", aurl("users", "j07"));
            AMI("Týmy osob", aurl("users", "j11"));
            AMI("Nadřízení/Podřízení", aurl("users", "j05"));

            DIV_TRANS("Časový fond");
            AMI("Pracovní fondy", aurl("users", "c21"));
            AMI("Dny svátků", aurl("users", "c26"));



            DIV_TRANS("Provoz");
            AMI("PING Log", aurl("users", "j92"));

            AMI("LOGIN Log", aurl("users", "j90"));

            DIV_TRANS("Pošta");
            AMI("Poštovní účty", aurl("users", "o40"));
            AMI("Šablony poštovních zpráv", aurl("users", "j61", "myqueryinline=x29id|int|j02"));
            AMI("OUTBOX", aurl("users", "x40"));


            handle_selected_item(prefix);

        }


        private void Handle_AdminWorksheet(string prefix)
        {
            //DIV_TRANS("Vykazování úkonů");
            AMI("Sešity", aurl("worksheet", "p34"));
            AMI("Aktivity", aurl("worksheet", "p32"));
            DIV();
            AMI("Fakturační oddíly", aurl("worksheet", "p95"));
            AMI("Odvětví aktivit", aurl("worksheet", "p38"));
            AMI("Klastry aktivit", aurl("worksheet", "p61"));

            DIV();
            AMI("Uzamknutá období", aurl("worksheet", "p36"));
            AMI("Jednotky kusovníkových úkonů", aurl("worksheet", "p35"));

            DIV_TRANS("Hodinové sazby");
            AMI("Ceníky sazeb", aurl("worksheet", "p51"));

            AMI("Uživatelská pole", aurl("worksheet", "x28", "myqueryinline=x29id|int|331"));
            handle_selected_item(prefix);

        }
        private void Handle_AdminBilling(string prefix)
        {
            AMI("Typy faktur", aurl("billing", "p92"));
            AMI("Bankovní účty", aurl("billing", "p86"));
            AMI("Vystavovatelé faktur", aurl("billing", "p93"));
            DIV();
            AMI("Měnové kurzy", aurl("billing", "m62"));
            AMI("DPH sazby", aurl("billing", "p53"));
            AMI("Fakturační oddíly", aurl("billing", "p95"));
            AMI("Zaokrouhlovací pravidla faktury", aurl("billing", "p98"));
            AMI("Struktury rozpisu částky faktury", aurl("billing", "p80"));
            AMI("Režijní přirážky k fakturaci", aurl("billing", "p63"));

            DIV();

            AMI("Ceníky sazeb", aurl("billing", "p51"));

            AMI("Šablony poštovních zpráv", aurl("billing", "j61", "myqueryinline=x29id|int|391"));
            DIV();
            AMI("Role uživatelů ve vyúčtování", aurl("billing", "x67", "myqueryinline=x29id|int|391"));
            AMI("Uživatelská pole", aurl("billing", "x28", "myqueryinline=x29id|int|391"));


            handle_selected_item(prefix);

        }
        private void Handle_AdminProforma(string prefix)
        {
            AMI("Typy záloh", aurl("proforma", "p89"));

            AMI("Role uživatelů v zálohách", aurl("proforma", "x67", "myqueryinline=x29id|int|390"));
            AMI("Uživatelská pole", aurl("proforma", "x28", "myqueryinline=x29id|int|390"));
            AMI("Šablony poštovních zpráv", aurl("proforma", "j61", "myqueryinline=x29id|int|390"));

            handle_selected_item(prefix);

        }

        private void Handle_AdminTasks(string prefix)
        {
            AMI("Typy úkolů", aurl("tasks", "p57"));

            AMI("Role uživatelů v úkolech", aurl("tasks", "x67", "myqueryinline=x29id|int|356"));
            AMI("Uživatelská pole", aurl("tasks", "x28", "myqueryinline=x29id|int|356"));
            AMI("Šablony poštovních zpráv", aurl("tasks", "j61", "myqueryinline=x29id|int|356"));

            handle_selected_item(prefix);

        }

        private void Handle_AdminProjects(string prefix)
        {
            AMI("Vertikální úrovně projektů", "javascript:_window_open('/p07/ProjectLevels')");
            DIV();
            AMI("Typy projektů", aurl("projects", "p42"));
            AMI("Role uživatelů v projektech", aurl("projects", "x67", "myqueryinline=x29id|int|141"));


            AMI("Uživatelská pole", aurl("projects", "x28", "myqueryinline=x29id|int|141"));

            handle_selected_item(prefix);

        }
        private void Handle_AdminClients(string prefix)
        {

            AMI("Typy klientů", aurl("clients", "p29"));
            AMI("Role uživatelů v klientech", aurl("clients", "x67", "myqueryinline=x29id|int|328"));

            AMI("Uživatelská pole", aurl("clients", "x28", "myqueryinline=x29id|int|328"));

            handle_selected_item(prefix);

        }
        private void Handle_AdminDocs(string prefix)
        {
            AMI("Typy dokumentů", aurl("docs", "x18"));
            AMI("Role uživatelů v dokumentu", aurl("docs", "x67", "myqueryinline=x29id|int|223"));
            AMI("Role uživatelů v typu dokumentu", aurl("docs", "x67", "myqueryinline=x29id|int|918"));

            handle_selected_item(prefix);
        }
        private void Handle_AdminCentres(string prefix)
        {
            AMI("Střediska", aurl("centres", "j18"));
            AMI("Role uživatelů ve středisku", aurl("centres", "x67", "myqueryinline=x29id|int|118"));


            handle_selected_item(prefix);
        }
        private void Handle_AdminTags(string prefix)
        {
            AMI("Skupiny štítků", aurl("tags", "o53"));
            AMI("Položky štítků", aurl("tags", "o51"));

            handle_selected_item(prefix);
        }
        private void Handle_AdminMisc(string prefix)
        {

            AMI("Číselné řady", aurl("misc", "x38"));

            AMI("Autocomplete položky", aurl("misc", "o15"));

            AMI("Daňové regiony", aurl("misc", "j17"));

            DIV_TRANS("Uživatelská pole");
            AMI("Katalog uživatelských polí", aurl("misc", "x28"));
            AMI("Skupiny uživatelských polí", aurl("misc", "x27"));

            DIV_TRANS("Technologie");
            AMI("Dashboard Widgety", aurl("misc", "x55"));
            AMI("Notifikace událostí", aurl("misc", "x46"));
            AMI("Report šablony", aurl("misc", "x31"));
            AMI("Report kategorie", aurl("misc", "j25"));


            AMI("Uživatelská nápověda", aurl("misc", "x51"));
            AMI("Aplikační překlad", aurl("misc", "x97"));

            handle_selected_item(prefix);

        }


    }
}
