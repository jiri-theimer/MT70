﻿using System;
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
                    of.SqlWrapper = "select j02ID FROM p31Worksheet WHERE p71ID IS NULL AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("j02Person", "ExistNevyuctovane", "a.j02ID", "Existují nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select j02ID FROM p31Worksheet WHERE p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("j02Person", "ExistCekajici", "a.j02ID", "Existují schválené a dosud nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select j02ID FROM p31Worksheet WHERE p71ID=1 AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
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
                    of.SqlWrapper = "select xb.p28ID_Client FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xb.p28ID_Client IS NOT NULL AND xa.p71ID IS NULL AND xa.p91ID IS NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p28Contact", "ExistNevyuctovane", "a.p28ID", "Existují nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select xb.p28ID_Client FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xb.p28ID_Client IS NOT NULL AND xa.p91ID IS NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p28Contact", "ExistCekajici", "a.p28ID", "Existují schválené a dosud nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select xb.p28ID_Client FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xb.p28ID_Client IS NOT NULL AND xa.p71ID=1 AND xa.p91ID IS NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p28Contact", "ExistVyuctovane", "a.p28ID", "Existují vyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select xb.p28ID_Client FROM p31Worksheet xa INNER JOIN p41Project xb ON xa.p41ID=xb.p41ID WHERE xb.p28ID_Client IS NOT NULL AND xa.p91ID IS NOT NULL AND xa.p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    break;
                case "p41":
                case "le5":
                    of = AF("p41Project", "ExistRozpracovane", "a.p41ID", "Existují rozpracované úkony", null, null, "bool");
                    of.SqlWrapper = "select p41ID FROM p31Worksheet WHERE p71ID IS NULL AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p41Project", "ExistNevyuctovane", "a.p41ID", "Existují nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p41ID FROM p31Worksheet WHERE p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p41Project", "ExistCekajici", "a.p41ID", "Existují schválené a dosud nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p41ID FROM p31Worksheet WHERE p71ID=1 AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p41Project", "ExistVyuctovane", "a.p41ID", "Existují vyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p41ID FROM p31Worksheet WHERE p91ID IS NOT NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    break;
                case "p56":
                    of = AF("p56Task", "ExistRozpracovane", "a.p56ID", "Existují rozpracované úkony", null, null, "bool");
                    of.SqlWrapper = "select p56ID FROM p31Worksheet WHERE p56ID IS NOT NULL AND p71ID IS NULL AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p56Task", "ExistNevyuctovane", "a.p56ID", "Existují nevyúčtované úkony", null, null, "bool");
                    of.SqlWrapper = "select p56ID FROM p31Worksheet WHERE p56ID IS NOT NULL AND p91ID IS NULL AND p31Date BETWEEN @p31date1 AND @p31date2 AND p31ValidUntil>GETDATE()";
                    of = AF("p56Task", "ExistCekajici", "a.p56ID", "Existují schválené a dosud nevyúčtované úkony", null, null, "bool");
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
