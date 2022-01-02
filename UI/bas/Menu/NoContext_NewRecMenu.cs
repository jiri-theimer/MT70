using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Menu
{
    public class NoContext_NewRecMenu: BaseNonContextMenu
    {
        public NoContext_NewRecMenu(BL.Factory f):base(f)
        {
            if (f.CurrentUser.j04IsMenu_Worksheet)
            {
                var lisP34 = f.p34ActivityGroupBL.GetList_WorksheetEntry_InAllProjects(f.CurrentUser.j02ID);
                if (lisP34.Count() < 10)
                {
                    foreach (var recP34 in lisP34)
                    {
                        var strIcon = "more_time";
                        if ((recP34.p33ID == BO.p33IdENUM.PenizeBezDPH || recP34.p33ID == BO.p33IdENUM.PenizeVcDPHRozpisu) && recP34.p34IncomeStatementFlag == BO.p34IncomeStatementFlagENUM.Vydaj)
                        {
                            strIcon = "price_change";
                        }
                        if ((recP34.p33ID == BO.p33IdENUM.PenizeBezDPH || recP34.p33ID == BO.p33IdENUM.PenizeVcDPHRozpisu) && recP34.p34IncomeStatementFlag == BO.p34IncomeStatementFlagENUM.Prijem)
                        {
                            strIcon = "price_check";
                        }
                        if (recP34.p33ID == BO.p33IdENUM.Kusovnik)
                        {
                            strIcon = "calculate";
                        }
                        AMI(recP34.p34Name, $"javascript:_p31_create({recP34.pid})", strIcon);
                    }
                }
                else
                {
                    AMI("Vykázat úkon", $"javascript:_p31_create()", "more_time");
                }
                DIV();
            }


            if (f.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P41_Creator) || f.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P41_Draft_Creator))
            {
                if (f.CurrentUser.p07LevelsCount == 1)
                {
                    AMI(f.CurrentUser.getP07Level(5, true), "javascript:_edit('p41',0)", "work_outline"); //nový projekt, pouze jedna vertikální úroveň
                }
                else
                {
                    for (int i = 1; i <= 5; i++)   //v systému více vertikálních úrovní
                    {
                        if (f.CurrentUser.getP07Level(i, true) != null)
                        {
                            AMI(f.CurrentUser.getP07Level(i, true), $"javascript:_edit('le{i}',0)", "work_outline");
                        }
                    }
                    DIV();
                }
            }
            if (f.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P28_Creator) || f.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P28_Draft_Creator))
            {
                AMI("Klient", "javascript:_edit('p28',0)", "business");
            }
            AMI("Úkol", "javascript:_edit('p56',0)", "task");
            if (f.CurrentUser.p07LevelsCount == 1) DIV();
            AMI("Dokument", "javascript:_window_open('/o23/SelectDocType')", "file_present");


            AMI("Záloha", "javascript:_edit('p90',0)", "receipt");
            //AMI("Poznámka", "javascript:_edit('b07',0)");
            DIV();
            //AMI("Interní osoba s uživatelským účtem", "javascript:_window_open('/j02/Record?pid=0&isintraperson=true', 1)");
            if (f.CurrentUser.IsAdmin)
            {
                AMI("Uživatelský účet", "javascript:_window_open('/j03/Record?pid=0', 1)", "person");
            }
            if (f.CurrentUser.TestPermission(BO.x53PermValEnum.GR_J02_ContactPerson_Create))
            {
                AMI("Kontaktní osoba klienta", "javascript:_window_open('/j02/Record?pid=0&isintraperson=false', 1)", "face");
            }
        }
    }
}
