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

                    

                    AF("j02Person", "a03Email", "a.j02Email", "E-mail");
                    AF("j02Person", "j02Code", "a.j02Code", "Osobní kód");
                    AF("j02Person", "j02JobTitle", "a.j02JobTitle", "Funkce na vizitce");
                    AF("j02Person", "j02Office", "a.j02Office", "Kancelář");
                    
                    AF("j02Person", "j02Phone", "a.j02Phone", "Pevný tel");
                    AF("j02Person", "ExistsJ03", "(case when exists(select j03ID FROM j03User WHERE j02ID=a.j02ID) then 1 else 0 end)", "Má uživatelský účet", null, null, "bool");
                    
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
