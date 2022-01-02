using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class NoContext_GridSelMenu: BaseNonContextMenu
    {
        public NoContext_GridSelMenu(BL.Factory f, TheGridUIContext tgi) :base(f)
        {

            if (f.CurrentUser.IsApprovingPerson)
            {
                if ("p31,p41,p28,j02,p56,le1,le2,le3,le4,le5".Contains(tgi.prefix))
                {
                    AMI("Schválit/Vyúčtovat", "javascript:tg_approve()", "approval");
                }
                if (f.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator))
                {


                    AMI("Přidat do vybraného vyúčtování", "javascript:tg_append2invoice()", "receipt_long");
                }
            }

            if (f.CurrentUser.IsRatesAccess)
            {

            }


            switch (tgi.prefix)
            {
                case "p31":
                    if (tgi.master_entity == "p91Invoice")
                    {
                        AMI("Vyjmout úkony z vyúčtování", "javascript:p91oper_p31operbatch('remove')");
                        AMI("Zahrnout do paušálu", "javascript:p91oper_p31operbatch('p70-6')");
                        AMI("Viditelný odpis", "javascript:p91oper_p31operbatch('p70-2')");
                        AMI("Skrytý odpis", "javascript:p91oper_p31operbatch('p70-3')");
                    }

                    break;
                case "p91":
                    AMI("ISDOC Export", "javascript:p91oper_isdoc()");
                    break;
            }
            if ("p31,p41,p28,j02,p91,p56,le1,le2,le3,le4,le5".Contains(tgi.prefix))
            {
                DIV();
                AMI("Oštítkovat", "javascript:tg_tagging()", "local_offer");
            }
            if (tgi.prefix == "p31" || tgi.prefix == "p56" || tgi.prefix == "o22")
            {
                AMI("Exportovat do Kalendáře (iCalendar)", "javascript:tg_ical()", "event");
            }

            DIV();
            AMI("GRID-REPORT", "javascript:tg_gridreport_selected()", "design_services");
            AMI("MS EXCEL Export", "javascript:tg_export('xlsx','selected')", "cloud_download");
            AMI("CSV Export", "javascript:tg_export('csv','selected')", "cloud_download");

        }
    }
}
