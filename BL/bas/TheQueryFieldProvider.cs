using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL
{
    public class TheQueryFieldProvider
    {
        //private readonly BL.TheEntitiesProvider _ep;
        private string _Prefix;
        private List<BO.TheQueryField> _lis;
        private string _lastEntity;

        public TheQueryFieldProvider(string strPrefix)
        {
            _Prefix = strPrefix;
            //_ep = ep;
            _lis = new List<BO.TheQueryField>();
            SetupPallete();


        }
        public List<BO.TheQueryField> getPallete()
        {
            return _lis;
        }
        private void SetupPallete()
        {
            BO.TheQueryField of;
            switch (_Prefix)
            {                
                case "j02":
                    of = AF("j02Person", "j02IsIntraPerson", "a.j02IsIntraPerson", "Interní osoba", null, null, "bool");
                    of = AF("j02Person", "Pozice", "a.j07ID", "Pozice", "j07PersonPosition", null, "multi");
                    of = AF("j02Person", "Stredisko", "a.j18ID", "Středisko", "j18Region", null, "multi");
                    of = AF("j02Person", "Fond", "a.c21ID", "Pracovní fond", "c21FondCalendar", null, "multi");
                    of = AF("j02Person", "Role", "j04ID","Aplikační role", "j04UserRole",null, "multi");
                    of.SqlWrapper = "a.j02ID IN (select j02ID FROM j03User WHERE j02ID IS NOT NULL AND #filter#)";

                    
                    of = AF("j02Person", "ExistRozpracovane","a.j02ID", "Existují rozpracované úkony", null, null, "bool");
                    of.SqlWrapper = "select xa.j02ID FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xa.p71ID IS NULL AND xa.p91ID IS NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE() AND xb.p41BillingFlag<99";
                    of = AF("j02Person", "ExistNevyuctovane", "a.j02ID", "Existují nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select xa.j02ID FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xa.p91ID IS NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND xa.p31ValidUntil>GETDATE() AND xb.p41BillingFlag<99";
                    of = AF("j02Person", "ExistCekajici", "a.j02ID", "Existují schválené a dosud nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select xa.j02ID FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xa.p71ID=1 AND xa.p91ID IS NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND xa.p31ValidUntil>GETDATE() AND xb.p41BillingFlag<99";
                    of = AF("j02Person", "ExistVyuctovane", "a.j02ID", "Existují vyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select j02ID FROM p31Worksheet WHERE p91ID IS NOT NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";

                    of = AF("j02Person", "ExistsJ03", "a.j02ID", "Má uživatelský účet", null, null, "bool");
                    of.SqlWrapper = "select j02ID FROM j03User WHERE j02ID IS NOT NULL";



                    AF("j02Person", "a03Email", "a.j02Email", "E-mail");
                    AF("j02Person", "j02Code", "a.j02Code", "Osobní kód");
                    AF("j02Person", "j02JobTitle", "a.j02JobTitle", "Funkce na vizitce");
                    AF("j02Person", "j02Office", "a.j02Office", "Kancelář");                    
                    AF("j02Person", "j02Phone", "a.j02Phone", "Pevný tel");
                    
                    
                    break;
                case "p28":
                    of = AF("p28Contact", "ExistRozpracovane", "a.p28ID", "Existují rozpracované úkony", null, null, "bool");
                    of.SqlWrapper = "select xb.p28ID_Client FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xb.p28ID_Client IS NOT NULL AND xa.p71ID IS NULL AND xa.p91ID IS NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE() AND xb.p41BillingFlag<99";
                    of = AF("p28Contact", "ExistNevyuctovane", "a.p28ID", "Existují nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select xb.p28ID_Client FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xb.p28ID_Client IS NOT NULL AND xa.p91ID IS NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE() AND xb.p41BillingFlag<99";
                    of = AF("p28Contact", "ExistCekajici", "a.p28ID", "Existují schválené a dosud nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select xb.p28ID_Client FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xb.p28ID_Client IS NOT NULL AND xa.p71ID=1 AND xa.p91ID IS NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE() AND xb.p41BillingFlag<99";
                    of = AF("p28Contact", "ExistVyuctovane", "a.p28ID", "Existují vyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select xb.p28ID_Client FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xb.p28ID_Client IS NOT NULL AND xa.p91ID IS NOT NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    break;
                case "p41":
                case "le1":
                case "le2":
                case "le3":
                case "le4":
                case "le5":
                    of = AF("p41Project", "ExistRozpracovane", "a.p41BillingFlag<99 AND a.p41ID", "Existují rozpracované úkony", null, null, "bool");
                    of.SqlWrapper = "select p41ID FROM p31Worksheet WHERE p71ID IS NULL AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p41Project", "ExistNevyuctovane", "a.p41BillingFlag<99 AND a.p41ID", "Existují nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p41ID FROM p31Worksheet WHERE p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p41Project", "ExistCekajici", "a.p41BillingFlag<99 AND a.p41ID", "Existují schválené a dosud nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p41ID FROM p31Worksheet WHERE p71ID=1 AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p41Project", "ExistVyuctovane", "a.p41ID", "Existují vyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p41ID FROM p31Worksheet WHERE p91ID IS NOT NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    break;
                case "p31":
                    of = AF("p31Worksheet", "FStatus", "a.p70ID", "Fakturační status", "p70BillingStatus", null, "multi");
                    of = AF("p31Worksheet", "ApproveStatus", "a.p72ID_AfterApprove", "Fakturační status od schvalovatele", "p72PreBillingStatus", null, "multi");
                    of = AF("p31Worksheet", "Sesit", "p32x.p34ID", "Sešit", "p34ActivityGroup", null, "multi");
                    of = AF("p31Worksheet", "Aktivita", "a.p32ID", "Aktivita", "p32Activity", null, "multi");
                    of = AF("p31Worksheet", "FakturacniOddil", "p32x.p95ID", "Fakturační oddíl", "p95InvoiceRow", null, "multi");
                    of = AF("p31Worksheet", "TypProjektu", "p41x.p42ID", "Typ projektu", "p42ProjectType", null, "multi");
                    of = AF("p31Worksheet", "Osoba", "a.j02ID", "Osoba", "j02Person", null, "multi");
                    of = AF("p31Worksheet", "StrediskoProjektu", "p41x.j18ID", "Středisko projektu", "j18Region", null, "multi");
                    of = AF("p31Worksheet", "KlientProjektu", "p41x.p28ID_Client", "Klient projektu", "p28Contact", null, "multi");
                    of = AF("p31Worksheet", "Dodavatel", "a.p28ID_Supplier", "Dodavatel výdaje", "p28Contact", null, "multi");

                    of = AF("p31Worksheet", "Rozpracovane", "a.p71ID IS NULL AND a.p91ID IS NULL AND p41x.p41BillingFlag<99", "Rozpracované úkony", null, null, "bool1x");
                    of = AF("p31Worksheet", "Schvalene", "a.p71ID=1 AND a.p91ID IS NULL AND p41x.p41BillingFlag<99", "Schváleno, čeká na vyúčtování", null, null, "bool1x");                    
                    of = AF("p31Worksheet", "Vyuctovane", "a.p91ID IS NOT NULL", "Vyúčtováno", null, null, "bool1x");
                    of = AF("p31Worksheet", "Nevyuctovane", "a.p91ID IS NULL AND a.p31ValidUntil>GETDATE() AND p41x.p41BillingFlag<99", "Nevyúčtováno", null, null, "bool1x");
                    of = AF("p31Worksheet", "VyplnenaKorekce", "a.p72ID_AfterTrimming IS NOT NULL", "Vyplněná korekce úkonu", null, null, "bool1x");
                    of = AF("p31Worksheet", "SUkolem", "a.p56ID IS NOT NULL", "Vykázáno s úkolem", null, null, "bool1x");
                    of = AF("p31Worksheet", "SDodavatelem", "a.p28ID_Supplier IS NOT NULL", "Přiřazen dodavatel výdaje", null, null, "bool1x");
                    of = AF("p31Worksheet", "SKodem", "a.p31Code IS NOT NULL", "Vyplněn kód dokladu", null, null, "bool1x");
                    of = AF("p31Worksheet", "SDokumentem", "a.p31ID IN (SELECT xa.x19RecordPID FROM x19EntityCategory_Binding xa INNER JOIN x20EntiyToCategory xb ON xa.x20ID=xb.x20ID WHERE xb.x29ID=331)", "Přiřazen dokument", null, null, "bool1x");
                    of = AF("p31Worksheet", "VArchivu", "a.p31ValidUntil<GETDATE()", "Přesunuto do archivu", null, null, "bool1x");
                    break;
                case "p56":
                    of = AF("p56Task", "ExistRozpracovane", "p41x.p41BillingFlag<99 AND a.p56ID", "Existují rozpracované úkony", null, null, "bool");
                    of.SqlWrapper = "select p56ID FROM p31Worksheet WHERE p56ID IS NOT NULL AND p71ID IS NULL AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p56Task", "ExistNevyuctovane", "p41x.p41BillingFlag<99 AND a.p56ID", "Existují nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p56ID FROM p31Worksheet WHERE p56ID IS NOT NULL AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p56Task", "ExistCekajici", "p41x.p41BillingFlag<99 AND a.p56ID", "Existují schválené a dosud nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p56ID FROM p31Worksheet WHERE p56ID IS NOT NULL AND p71ID=1 AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p56Task", "ExistVyuctovane", "a.p56ID", "Existují vyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p56ID FROM p31Worksheet WHERE p56ID IS NOT NULL AND p91ID IS NOT NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    break;
                default:
                    break;
            }




        }


        private BO.TheQueryField AF(string strEntity, string strField,string strSqlSyntax, string strHeader, string strSourceEntity = null, string strSourceSql = null, string strFieldType = "string")
        {
            if (strEntity != _lastEntity)
            {
                //zatím nic
            }

            _lis.Add(new BO.TheQueryField() { Field = strField,FieldSqlSyntax=strSqlSyntax, Entity = strEntity, Header = strHeader, FieldType = strFieldType, SourceEntity = strSourceEntity, SourceSql = strSourceSql, TranslateLang1 = strHeader, TranslateLang2 = strHeader, TranslateLang3 = strHeader });
            _lastEntity = strEntity;
            return _lis[_lis.Count - 1];
        }
    }
}
