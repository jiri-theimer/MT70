using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Menu
{
    public class NoContext_MyProfileMenu:BaseNonContextMenu
    {
        public NoContext_MyProfileMenu(BL.Factory f):base(f)
        {
            AMI("Aktuální stránku uložit jako domovskou", "javascript:_save_as_home_page()", "favorite_border");
            if (f.CurrentUser.j03HomePageUrl != null)
            {
                AMI("Vyčistit odkaz na domovskou stránku", "javascript:_clear_home_page()", "heart_broken");
            }
            AMI("Tovární HOME stránka (Dashboard)", "/Dashboard/Index", "dashboard");

            DIV();

            if (f.CurrentUser.j04IsMenu_MyProfile)
            {
                AMI("Můj profil", "/Home/MyProfile", "person");
            }

            AMI("Odeslat zprávu", "javascript:_window_open('/Mail/SendMail',1)", "email");

            AMI("Změnit přístupové heslo", "/Home/ChangePassword", "password");

            DIV();
            AMI("Nápověda", "javascript: _window_open('/x51/Index')", "help_outline");
            AMI("O aplikaci", "/Home/About", "info");
            DIV();
            AMI("Odhlásit se", "/Home/logout", "logout");

        }
    }
}
