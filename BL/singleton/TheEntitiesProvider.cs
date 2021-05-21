using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL
{
    public class TheEntitiesProvider
    {
        private readonly BL.RunningApp _app;
        private readonly BL.TheTranslator _tt;
        private List<BO.TheEntity> _lis;
        public TheEntitiesProvider(BL.RunningApp runningapp, BL.TheTranslator tt)
        {
            _app = runningapp;
            _tt = tt;

            SetupPallete();

            DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
            var dt = db.GetDataTable("select x29ID,LEFT(x29TableName,3) as Prefix from x29Entity");
            foreach (System.Data.DataRow dbrow in dt.Rows)
            {
                if (_lis.Any(p => p.Prefix == dbrow["Prefix"].ToString()))
                {
                    ByPrefix(dbrow["Prefix"].ToString()).x29ID = Convert.ToInt32(dbrow["x29ID"]);
                    
                }
                
                
            }
        }

        public BO.TheEntity ByPrefix(string strPrefix)
        {            
            return _lis.Where(p => p.Prefix == strPrefix).First();
        }
        public BO.TheEntity ByTable(string strTableName)
        {            
            return _lis.Where(p => p.TableName == strTableName).First();
        }
        public BO.TheEntity ByX29ID(int intX29ID)
        {
            if (intX29ID == 0) return null;

            if (_lis.Where(p => p.x29ID == intX29ID).Count() > 0)
            {
                return _lis.Where(p => p.x29ID == intX29ID).First();
            }

            return null;
        }


        private void SetupPallete()
        {
            _lis = new List<BO.TheEntity>();
            AE("j02Person", "Osobní profily", "Osobní profil", "j02Person a", "a.j02LastName,a.j02FirstName");
            AE("j03User", "Uživatelské účty", "Uživatel", "j03User a INNER JOIN j04UserRole j03_j04 ON a.j04ID=j03_j04.j04ID", "a.j03Login");
            AE("j04UserRole", "Aplikační role", "Aplikační role", "j04UserRole a ", "a.j04Name");
            AE("j05MasterSlave", "Nadřízený/Podřízený", "Nadřízení/Podřízení", "j05MasterSlave a INNER JOIN j02Person j02master ON a.j02ID_Master=j02master.j02ID LEFT OUTER JOIN j02Person j02slave ON a.j02ID_Slave=j02slave.j02ID LEFT OUTER JOIN j11Team j11slave ON a.j11ID_Slave=j11slave.j11ID", "a.j05ID DESC");
            AE("j11Team", "Týmy osob", "Tým", "j11Team a ", "a.j11Name");
            AE("j07PersonPosition", "Pozice osob", "Pozice", "j07PersonPosition a ", "a.j07Ordinary");
            AE("j18Region", "Střediska", "Středisko", "j18Region a ", "a.j18Ordinary");
            AE("j17Country", "Regiony", "Region", "j17Country a ", "a.j17Ordinary");
            AE("j19PaymentType", "Forma úhrady", "Forma úhrady", "j19PaymentType a ", "a.j19Ordinary");
            


            AE("j27Currency", "Měny", "Měna", "j27Currency a ", "a.j27Ordinary,a.j27Code");
            AE("j61TextTemplate", "Šablony poštovních zpráv", "Šablona poštovní zprávy", "j61TextTemplate a ", "a.j61Ordinary");

            AE_TINY("j90LoginAccessLog", "LOGIN Log", "LOGIN Log");
            ByPrefix("j90").IsWithoutValidity = true;
            AE_TINY("j92PingLog", "PING Log", "PING Log");
            ByPrefix("j92").IsWithoutValidity = true;

            AE("x40MailQueue", "OUTBOX", "OUTBOX", "x40MailQueue a", "a.x40ID DESC", "a.x40ID DESC");
            ByPrefix("x40").IsWithoutValidity = true;

            AE("b01WorkflowTemplate", "Workflow šablony", "Workflow šablona", "b01WorkflowTemplate a", "a.b01Name");
            AE("b02WorkflowStatus", "Workflow stavy", "Workflow stav", "b02WorkflowStatus a", "a.b01ID,a.b02Order,a.b02Name", "a.b01ID,a.b02Order,a.b02Name");
            //AE("b05Workflow_History", "Workflow historie", "Workflow historie", "b05Workflow_History a", "a.b05ID DESC");
            //ByPrefix("b05").IsWithoutValidity = true;
            //AE("b06WorkflowStep", "Workflow kroky", "Workflow krok", "b06WorkflowStep a", "a.b06Order", "a.b06Order");
            //AE("b65WorkflowMessage", "Šablony notifikačních zpráv", "Workflow notifikační zpráva", "b65WorkflowMessage a", "a.b65Name");

            AE("o15AutoComplete", "AutoComplete položky", "AutoComplete položka", "o15AutoComplete a", "a.o15Flag");

            AE("p29ContactType", "Typy klientů", "Typ klienta", "p29ContactType a", "a.p29Ordinary");
            AE("p34ActivityGroup", "Sešity aktivit", "Sešit", "p34ActivityGroup a", "a.p34Ordinary");
            AE("p32Activity", "Aktivity", "Aktivita", "p32Activity a", "a.p32Ordinary");
            AE("p35Unit", "Kusovníkové jednotky", "Jednotka kusovníku", "p35Unit a", "a.p35Name");
            AE("p36LockPeriod", "Uzamčená období", "Uzamčené období", "p36LockPeriod a", "a.p36DateFrom");
            AE("p38ActivityTag", "Odvětví aktivit", "Odvětví aktivity", "p38ActivityTag a", "a.p38Ordinary");
            AE("p53VatRate", "DPH sazby", "DPH sazba", "p53VatRate a", "a.p53ID DESC");
            AE("p61ActivityCluster", "Klastry aktivit", "Klastr aktivit", "p61ActivityCluster a", "a.p61Name");
            AE("p63Overhead", "Režijní přirážka k fakturaci", "Režijní přirážka", "p63Overhead a","a.p63Name");
            AE("p87BillingLanguage", "Fakturační jazyk", "Fakturační jazyk", "p87BillingLanguage a", "a.p87LangIndex");
            AE("p95InvoiceRow", "Fakturační oddíly", "Fakturační oddíl", "p95InvoiceRow a", "a.p95Ordinary");
            AE("p80InvoiceAmountStructure", "Struktury rozpisu částky faktury", "Struktura cenového rozpisu faktury", "p80InvoiceAmountStructure a", "a.p80Name");
            AE("p41Project", "Projekty", "Projekt", "p41Project a", "a.p41ID DESC");
            AE("le4", "L4", "L4", "p41Project a INNER JOIN p42ProjectType e2 ON a.p42ID=e2.p42ID INNER JOIN p07ProjectLevel e3 ON e2.p07ID=e3.p07ID AND e3.p07Level=4", "a.p41ID DESC","a.p41ID DESC");
            AE("le5", "L5", "L5", "p41Project a INNER JOIN p42ProjectType e2 ON a.p42ID=e2.p42ID INNER JOIN p07ProjectLevel e3 ON e2.p07ID=e3.p07ID AND e3.p07Level=5", "a.p41ID DESC","a.p41ID DESC");            

            AE("p28Contact", "Klienti", "Klient", "p28Contact a", "a.p28ID DESC");
            AE("p56Task", "Úkoly", "Úkol", "p56Task a INNER JOIN p41Project p41x ON a.p41ID=p41x.p41ID", "a.p56ID DESC");

            AE("view_PrimaryAddress", "Adresy", "Adresa", "view_PrimaryAddress a",null);
            
            AE("p31Worksheet", "Úkony", "Úkon", "p31Worksheet a INNER JOIN p41Project p41x ON a.p41ID=p41x.p41ID INNER JOIN p32Activity p32x ON a.p32ID=p32x.p32ID INNER JOIN p34ActivityGroup p34x ON p32x.p34ID=p34x.p34ID LEFT OUTER JOIN p91Invoice p91x ON a.p91ID=p91x.p91ID", "a.p31ID DESC");            
            AE("o23Doc", "Dokumenty", "Dokument", "o23Doc a", "a.o23ID DESC");
            AE("b07Comment", "Poznámky", "Poznámka", "b07Comment a INNER JOIN j02Person j02x ON a.j02ID_Owner=j02x.j02ID AND a.b07WorkflowInfo IS NULL AND ISNULL(a.b07Value,'') NOT LIKE 'upload'", "a.b07ID DESC");

            AE("p91Invoice", "Vyúčtování", "Vyúčtování", "p91Invoice a", "a.p91ID DESC");
            AE("p90Proforma","Zálohy", "Záloha", "p90Proforma a", "a.p90ID DESC");

            AE("p07ProjectLevel", "Úrovně projektů", "Úroveň projektu", "p07ProjectLevel a", "a.p07Level");
            AE("p42ProjectType", "Typy projektů", "Typ projektu", "p42ProjectType a", "a.p42Ordinary");
            AE("p51PriceList", "Ceníky sazeb", "Ceník sazeb", "p51PriceList a", "a.p51Ordinary");
            AE("c21FondCalendar", "Pracovní fondy", "Pracovní fond", "c21FondCalendar a", "a.c21Ordinary");
            AE("c26Holiday", "Dny svátků", "Svátek", "c26Holiday a", "a.c26Date");
            AE("x67EntityRole", "Role osob", "Role", "x67EntityRole a", "a.x67Ordinary,a.x67Name");


            AE("p92InvoiceType", "Typy faktur", "Typ faktury", "p92InvoiceType a", "a.p92Ordinary");
            AE("p93InvoiceHeader", "Vystavovatelé faktur", "Vystavovatel faktury", "p93InvoiceHeader a", "a.p93Name");
            AE("p86BankAccount", "Bankovní účty", "Bankovní účet", "p86BankAccount a", "a.p86Name");
            AE("p98Invoice_Round_Setting_Template", "Zaokrouhlovací pravidla", "Zaokrouhlovací pravidlo", "p98Invoice_Round_Setting_Template a","a.p98Name");
            AE("p80InvoiceAmountStructure", "Struktury rozpisu částky faktury", "Struktura rozpisu částky faktury", "p80InvoiceAmountStructure a", "a.p80Name");
            AE("p82Proforma_Payment", "Úhrady záloh", "Úhrada zálohy", "p82Proforma_Payment a","a.p82ID DESC");
            AE("p89ProformaType", "Typy záloh", "Typ zálohy", "p89ProformaType a", "a.p89Name");

            AE("m62ExchangeRate", "Měnové kurzy", "Měnový kurz", "m62ExchangeRate a","a.m62ID DESC");

            AE("o38Address", "Adresy", "Adresa", "o38Address a", "a.o38ID DESC");
            AE("o53TagGroup", "Štítky", "Štítek", "o53TagGroup a", "a.o53Ordinary");
            AE_TINY("o54TagBindingInline", "Štítky", "Štítky");
            AE("o51Tag", "Položky štítků", "Položka štítku", "o51Tag a", "a.o51ID DESC");
            AE("o40SmtpAccount", "Poštovní účty", "Poštovní účet", "o40SmtpAccount a", "a.o40ID DESC");
            AE("x29Entity", "Entity", "Entita", "x29Entity a","a.x29Name");
            ByPrefix("x29").IsWithoutValidity = true;
            AE("x15VatRateType", "Entity", "Entita", "x15VatRateType a", "a.x15Ordinary");
            ByPrefix("x15").IsWithoutValidity = true;

            AE("x18EntityCategory", "Typ dokumentů", "Typ dokumentu", "x18EntityCategory a", "a.x18Ordinary");

            AE_TINY("x28EntityField", "Uživatelská pole", "Uživatelské pole");
            AE_TINY("x27EntityFieldGroup", "Skupiny uživatelských polí", "Skupina uživatelských polí");
            AE_TINY("x31Report", "Report šablony", "Pevná tisková sestava");
            AE_TINY("j25ReportCategory", "Kategorie sestav", "Kategorie sestavy");
            AE_TINY("x38CodeLogic", "Číselné řady", "Číselná řada");
            AE_TINY("x46EventNotification", "Aplikační události", "Událost");

            AE_TINY("x51HelpCore", "Uživatelská nápověda", "Uživatelská nápověda");
            AE_TINY("x55Widget", "Widgety", "Widget");
            AE_TINY("x97Translate", "Aplikační překlad", "Aplikační překlad");
            ByPrefix("x97").IsWithoutValidity = true;
            
            
        }

        private void AE(string strTabName, string strPlural, string strSingular, string strSqlFromGrid, string strSqlOrderByCombo, string strSqlOrderBy = null)
        {
            
            if (strSqlOrderBy == null) strSqlOrderBy = "a." + strTabName.Substring(0, 3) + "ID DESC";
            BO.TheEntity c = new BO.TheEntity() { TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular, SqlFromGrid = strSqlFromGrid, SqlOrderByCombo = strSqlOrderByCombo, SqlOrderBy = strSqlOrderBy };
            c.TranslateLang1 = _tt.DoTranslate(strPlural, 1,"TheEntitiesProvider:AE");
            c.TranslateLang2 = _tt.DoTranslate(strPlural, 2, "TheEntitiesProvider:AE");
            _lis.Add(c);

        }
        private void AE_TINY(string strTabName, string strPlural, string strSingular)
        {

            _lis.Add(new BO.TheEntity() { TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular, SqlFromGrid = strTabName + " a", SqlOrderByCombo = "a." + strTabName.Substring(0, 3) + "Name", SqlOrderBy = "a." + strTabName.Substring(0, 3) + "ID DESC",TranslateLang1= _tt.DoTranslate(strPlural, 1),TranslateLang2= _tt.DoTranslate(strPlural, 2) });
        }
        private BO.EntityRelation getREL(string strTabName, string strRelName, string strSingular, string strSqlFrom, string strDependOnRel = null)
        {
            return new BO.EntityRelation() { TableName = strTabName, RelName = strRelName, AliasSingular = strSingular, SqlFrom = strSqlFrom, RelNameDependOn = strDependOnRel,Translate1=_tt.DoTranslate(strSingular,1), Translate2 = _tt.DoTranslate(strSingular, 2, "TheEntitiesProvider:getREL") };



        }

        public List<BO.EntityRelation> getApplicableRelations(string strPrimaryPrefix)
        {
            
            var lis = new List<BO.EntityRelation>();
            BO.TheEntity ce = ByPrefix(strPrimaryPrefix);           

            switch (strPrimaryPrefix)
            {
                case "j02":
                    lis.Add(getREL("j03User", "j02_j03", "Uživatelský účet", "LEFT OUTER JOIN j03User j02_j03 ON a.j02ID=j02_j03.j02ID LEFT OUTER JOIN j04UserRole j03_j04 ON j02_j03.j04ID=j03_j04.j04ID"));
                    lis.Add(getREL("j07PersonPosition", "j02_j07", "Pozice", "LEFT OUTER JOIN j07PersonPosition j02_j07 ON a.j07ID=j02_j07.j07ID"));

                    break;
                case "j03":
                    lis.Add(getREL("j02Person", "j03_j02", "Osobní profil", "LEFT OUTER JOIN j02Person j03_j02 ON a.j02ID=j03_j02.j02ID"));
                    break;
                case "j90":
                    lis.Add(getREL("j03User", "j90_j03", "Uživatelský účet", "INNER JOIN j03User j90_j03 ON a.j03ID=j90_j03.j03ID INNER JOIN j04UserRole j03_j04 ON j90_j03.j04ID=j03_j04.j04ID"));
                    break;
                case "j92":
                    lis.Add(getREL("j03User", "j92_j03", "Uživatelský účet", "INNER JOIN j03User j92_j03 ON a.j03ID=j92_j03.j03ID INNER JOIN j04UserRole j03_j04 ON j92_j03.j04ID=j03_j04.j04ID"));
                    break;
                case "p32":
                    lis.Add(getREL("p34ActivityGroup", "p32_p34", "Sešit", "INNER JOIN p34ActivityGroup p32_p34 ON a.p34ID=p32_p34.p34ID"));
                    lis.Add(getREL("p95InvoiceRow", "p32_p95", "Fakturační oddíl", "LEFT OUTER JOIN p95InvoiceRow p32_p95 ON a.p95ID=p32_p95.p95ID"));
                    lis.Add(getREL("p38ActivityTag", "p32_p38", "Kategorie", "LEFT OUTER JOIN p38ActivityTag p32_p38 ON a.p38ID=p32_p38.p38ID"));
                    lis.Add(getREL("p35Unit", "p32_p35", "Jednotka kusovníku", "LEFT OUTER JOIN p35Unit p32_p35 ON a.p35ID=p32_p35.p35ID"));
                    
                    break;
                case "p28":
                    lis.Add(getREL("view_PrimaryAddress", "p28_address_primary", "Fakturační adresa", "LEFT OUTER JOIN view_PrimaryAddress p28_address_primary ON a.p28ID=p28_address_primary.p28ID"));
                    lis.Add(getREL("p92InvoiceType", "p28_p92", "Typ faktury", "LEFT OUTER JOIN p92InvoiceType p28_p92 ON a.p92ID=p28_p92.p92ID"));
                    lis.Add(getREL("p29ContactType", "p28_p29", "Typ klienta", "LEFT OUTER JOIN p29ContactType p28_p29 ON a.p29ID=p28_p29.p29ID"));                    
                    break;
                case "p31":
                    lis.Add(getREL("j02Person", "p31_j02", "Osoba úkonu", "LEFT OUTER JOIN j02Person p31_j02 ON a.j02ID=p31_j02.j02ID"));
                    lis.Add(getREL("j02Person", "p31_j02contact", "Kontaktní osoba úkonu", "LEFT OUTER JOIN j02Person p31_j02contact ON a.j02ID_ContactPerson=p31_j02contact.j02ID"));                    
                    lis.Add(getREL("p32Activity", "p31_p32", "Aktivita", "INNER JOIN p32Activity p31_p32 ON a.p32ID=p31_p32.p32ID"));
                    lis.Add(getREL("p34ActivityGroup", "p32_p34", "Sešit", "INNER JOIN p32Activity aktivita1 ON a.p32ID=aktivita1.p32ID INNER JOIN p34ActivityGroup p32_p34 ON aktivita1.p34ID=p32_p34.p34ID"));
                    lis.Add(getREL("p41Project", "p31_p41", "Projekt", "LEFT OUTER JOIN p41Project p31_p41 ON a.p41ID=p31_p41.p41ID"));
                    lis.Add(getREL("p28Contact", "p31_p41_p28", "Klient projektu", "LEFT OUTER JOIN p28Contact p31_p41_p28 ON p31_p41.p28ID_Client=p31_p41_p28.p28ID", "p31_p41"));
                    lis.Add(getREL("p91Invoice", "p31_p91", "Vyúčtování", "LEFT OUTER JOIN p91Invoice p31_p91 ON a.p91ID=p31_p91.p91ID"));

                    break;
                case "p36":
                    lis.Add(getREL("j02Person", "p36_j02", "Osoba", "LEFT OUTER JOIN j02Person p36_j02 ON a.j02ID=p36_j02.j02ID"));
                    lis.Add(getREL("j11Team", "p36_j11", "Tým", "LEFT OUTER JOIN j11Team p36_j11 ON a.j11ID=p36_j11.j11ID"));
                    break;
                case "p41":
                case "le1":
                case "le2":
                case "le3":
                case "le4":
                case "le5":
                    lis.Add(getREL("p28Contact", "p41_p28client", "Klient", "LEFT OUTER JOIN p28Contact p41_p28client ON a.p28ID_Client=p41_p28client.p28ID"));
                    lis.Add(getREL("p28Contact", "p41_p28billing", "Odběratel faktury", "LEFT OUTER JOIN p28Contact p41_p28billing ON a.p28ID_Billing=p41_p28billing.p28ID"));
                    lis.Add(getREL("p42ProjectType", "p41_p42", "Typ projektu", "LEFT OUTER JOIN p42ProjectType p41_p42 ON a.p42ID=p41_p42.p42ID"));
                    lis.Add(getREL("p07ProjectLevel", "p41_p07", "Úroveň projektu", "LEFT OUTER JOIN p07ProjectLevel p41_p07 ON p41_p42.p07ID=p41_p07.p07ID", "p41_p42"));
                    lis.Add(getREL("b02WorkflowStatus", "p41_b02", "Workflow stav", "LEFT OUTER JOIN b02WorkflowStatus p41_b02 ON a.b02ID=p41_b02.b02ID"));

                    break;
                case "p51":
                    lis.Add(getREL("j27Currency", "p51_j27", "Měna", "LEFT OUTER JOIN j27Currency p51_j27 ON a.j27ID=p51_j27.j27ID"));
                    break;
                case "p53":
                    lis.Add(getREL("j27Currency", "p53_j27", "Měna", "LEFT OUTER JOIN j27Currency p53_j27 ON a.j27ID=p53_j27.j27ID"));
                    lis.Add(getREL("x15VatRateType", "p53_x15", "Hladina DPH", "LEFT OUTER JOIN x15VatRateType p53_x15 ON a.x15ID=p53_x15.x15ID"));
                    break;
                case "p56":
                    lis.Add(getREL("p41Project", "p56_p41", "Projekt", "INNER JOIN p41Project p56_p41 ON a.p41ID=p56_p41.p41ID"));
                    lis.Add(getREL("p57TaskType", "p56_p57", "Typ úkolu", "INNER JOIN p57TaskType p56_p57 ON a.p57ID=p56_p57.p57ID"));
                    lis.Add(getREL("b02WorkflowStatus", "p56_b02", "Workflow stav", "LEFT OUTER JOIN b02WorkflowStatus p56_b02 ON a.b02ID=p56_b02.b02ID"));

                    break;
                case "m62":                    
                    lis.Add(getREL("j27Currency", "m62_j27slave", "Cizí měna", "LEFT OUTER JOIN j27Currency m62_j27slave ON a.j27ID_Slave=m62_j27slave.j27ID"));
                    lis.Add(getREL("j27Currency", "m62_j27master", "Domácí měna", "LEFT OUTER JOIN j27Currency m62_j27master ON a.j27ID_Master=m62_j27master.j27ID"));
                    break;
                case "j18":
                    lis.Add(getREL("j17Country", "j18_j17", "Region", "LEFT OUTER JOIN j17Country j18_j17 ON a.j17ID=j18_j17.j17ID"));
                    break;
                case "p42":
                    lis.Add(getREL("p07ProjectLevel", "p42_p07", "Úroveň", "INNER JOIN p07ProjectLevel p42_p07 ON a.p07ID=p42_p07.p07ID"));
                    break;
                case "p90":
                    lis.Add(getREL("j27Currency", "p90_j27", "Měna", "LEFT OUTER JOIN j27Currency p90_j27 ON a.j27ID=p90_j27.j27ID"));
                    lis.Add(getREL("p28Contact", "p90_p28", "Klient zálohy", "LEFT OUTER JOIN p28Contact p90_p28 ON a.p28ID=p90_p28.p28ID"));
                    lis.Add(getREL("p89ProformaType", "p90_p89", "Typ zálohy", "LEFT OUTER JOIN p89ProformaType p90_p89 ON a.p89ID=p90_p89.p89ID"));
                    break;
                case "p91":
                    lis.Add(getREL("j27Currency", "p91_j27", "Měna", "LEFT OUTER JOIN j27Currency p91_j27 ON a.j27ID=p91_j27.j27ID"));
                    lis.Add(getREL("p28Contact", "p91_p28", "Klient", "LEFT OUTER JOIN p28Contact p91_p28 ON a.p28ID=p91_p28.p28ID"));
                    lis.Add(getREL("p92InvoiceType", "p91_p92", "Typ faktury", "LEFT OUTER JOIN p92InvoiceType p91_p92 ON a.p92ID=p91_p92.p92ID"));
                    lis.Add(getREL("p93InvoiceHeader", "p91_p93", "Vystavovatel faktury", "LEFT OUTER JOIN p93InvoiceHeader p91_p93 ON p91_p92.p93ID=p91_p93.p93ID", "p91_p92"));
                    lis.Add(getREL("b02WorkflowStatus", "p91_b02", "Workflow stav", "LEFT OUTER JOIN b02WorkflowStatus p91_b02 ON a.b02ID=p91_b02.b02ID"));
                    lis.Add(getREL("p98Invoice_Round_Setting_Template", "p91_p98", "Zaokrouhlovací pravidlo", "LEFT OUTER JOIN p98Invoice_Round_Setting_Template p91_p98 ON a.p98ID=p91_p98.p98ID"));
                    lis.Add(getREL("p80InvoiceAmountStructure", "p91_p80", "Struktura cenového rozpisu", "LEFT OUTER JOIN p80InvoiceAmountStructure p91_p80 ON a.p80ID=p91_p80.p80ID"));
                    lis.Add(getREL("p63Overhead", "p91_p63", "Režijní přirážka", "LEFT OUTER JOIN p63Overhead p91_p63 ON a.p63ID=p91_p63.p63ID"));
                    lis.Add(getREL("j17Country", "p91_j17", "DPH region", "LEFT OUTER JOIN j17Country p91_j17 ON a.j17ID=p91_j17.j17ID"));
                    break;
                case "p92":
                    lis.Add(getREL("j27Currency", "p92_j27", "Cílová měna", "LEFT OUTER JOIN j27Currency p92_j27 ON a.j27ID=p92_j27.j27ID"));
                    lis.Add(getREL("p93InvoiceHeader", "p92_p93", "Vystavovatel faktury", "LEFT OUTER JOIN p93InvoiceHeader p92_p93 ON a.p93ID=p92_p93.p93ID"));
                    lis.Add(getREL("x15VatRateType", "p92_x15", "Cílová DPH", "LEFT OUTER JOIN x15VatRateType p92_x15 ON a.x15ID=p92_x15.x15ID"));
                    break;
                case "c26":
                    lis.Add(getREL("j17Country", "c26_j17", "Region", "LEFT OUTER JOIN j17Country c26_j17 ON a.j17ID=c26_j17.j17ID"));
                    break;
                case "j61":
                    lis.Add(getREL("x29Entity", "j61_x29", "Kontext", "LEFT OUTER JOIN x29Entity j61_x29 ON a.x29ID=j61_x29.x29ID"));
                    lis.Add(getREL("j02Person", "j61_j02", "Vlastník záznamu", "LEFT OUTER JOIN j02Person j61_j02 ON a.j02ID_Owner=j61_j02.j02ID"));
                    break;
                case "x31":
                    lis.Add(getREL("x29Entity", "x31_x29", "Kontext", "LEFT OUTER JOIN x29Entity x31_x29 ON a.x29ID=x31_x29.x29ID"));
                    break;
                case "o23":
                    lis.Add(getREL("b02WorkflowStatus", "o23_b02", "Workflow stav", "LEFT OUTER JOIN b02WorkflowStatus o23_b02 ON a.b02ID=o23_b02.b02ID"));
                    lis.Add(getREL("p28Contact", "o23_p28", "Vazba s klientem", "LEFT OUTER JOIN p28Contact o23_p28 ON a.p28ID_First=o23_p28.p28ID"));
                    lis.Add(getREL("p41Project", "o23_p41", "Vazba s projektem", "LEFT OUTER JOIN p41Project o23_p41 ON a.p41ID_First=o23_p41.p41ID"));
                    lis.Add(getREL("j02Person", "o23_j02", "Vazba s osobou", "LEFT OUTER JOIN j02Person o23_j02 ON a.j02ID_First=o23_j02.j02ID"));
                    lis.Add(getREL("p91Invoice", "o23_p91", "Vazba s vyúčtováním", "LEFT OUTER JOIN p91Invoice o23_p91 ON a.p91ID_First=o23_p91.p91ID"));
                    break;
                case "b07":
                    lis.Add(getREL("p28Contact", "b07_p28", "Vazba s klientem", "LEFT OUTER JOIN p28Contact b07_p28 ON a.x29ID=328 AND a.b07RecordPID=b07_p28.p28ID"));
                    lis.Add(getREL("p41Project", "b07_p41", "Vazba s projektem", "LEFT OUTER JOIN p41Project b07_p41 ON a.x29ID=141 AND a.b07RecordPID=b07_p41.p41ID"));
                    lis.Add(getREL("j02Person", "b07_j02", "Vazba s osobou", "LEFT OUTER JOIN j02Person b07_j02 ON a.x29ID=102 AND a.b07RecordPID=b07_j02.j02ID"));
                    lis.Add(getREL("p91Invoice", "b07_p91", "Vazba s vyúčtováním", "LEFT OUTER JOIN p91Invoice b07_p91 ON a.x29ID=391 AND a.b07RecordPID=b07_p91.p91ID"));
                    break;
                case "o51":
                    lis.Add(getREL("o53TagGroup", "o51_o53", "Štítek", "LEFT OUTER JOIN o53TagGroup o51_o53 ON a.o53ID=o51_o53.o53ID"));
                    break;
                case "o40":
                    lis.Add(getREL("j02Person", "o40_j02", "Osoba", "LEFT OUTER JOIN j02Person o40_j02 ON a.j02ID_Owner=o40_j02.j02ID"));
                    break;
                case "x28":
                    lis.Add(getREL("x29Entity", "x28_x29", "Entita", "LEFT OUTER JOIN x29Entity x28_x29 ON a.x29ID=x28_x29.x29ID"));
                    break;
                default:
                    break;
            }

            return lis;
        }

        private string getOwnerSql(string prefix)
        {
            return string.Format("LEFT OUTER JOIN j02Person {0}_owner ON a.j02ID_Owner = {0}_owner.j02ID", prefix);
        }

    }
}
