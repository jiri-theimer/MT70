using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace BL.bas
{
    public class ICalendarGenerator
    {
        private System.Text.StringBuilder _sb { get; set; }
        private Factory _Factory { get; set; }
        private IEnumerable<BO.j02Person> _lisJ02 { get; set; }
        public int ReturnEventRows { get; set; }


        public ICalendarGenerator(BL.Factory f)
        {
            _sb = new System.Text.StringBuilder();
            _Factory = f;
        }

        public bool Save2TempInviteFile(string strICal, string strFileName)
        {
            string strPath = _Factory.x35GlobalParamBL.TempFolder() + @"\" + strFileName;
            System.IO.StreamWriter objWriter = new System.IO.StreamWriter(strPath, false);
            objWriter.Write(strICal);
            objWriter.Close();
            return true;
        }

        private void sr(string s)
        {
            _sb.AppendLine(s);
        }
        private void sr_vcalendar_header()
        {
            BO.myQueryJ02 mq = new BO.myQueryJ02() { IsRecordValid = null };
            _lisJ02 = _Factory.j02PersonBL.GetList(mq);

            sr("BEGIN:VCALENDAR");
            sr("VERSION:2.0");
            sr("METHOD:PUBLISH");
            sr("PRODID:marktime.net");
            sr("X-SZN-COLOR:#088acc");
            sr("X-WR-CALNAME:MARKTIME");
        }
        private void sr_timezone()
        {
            sr("BEGIN:VTIMEZONE");
            sr("TZID:Europe/Prague");
            sr("BEGIN:STANDARD");
            sr("DTSTART:20001029T030000");
            sr("RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10");
            sr("TZNAME:CET");
            sr("TZOFFSETFROM:+0200");
            sr("TZOFFSETTO:+0100");
            sr("END:STANDARD");
            sr("BEGIN:DAYLIGHT");
            sr("DTSTART:20000326T020000");
            sr("RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3");
            sr("TZNAME:CEST");
            sr("TZOFFSETFROM:+0100");
            sr("TZOFFSETTO:+0200");
            sr("END:DAYLIGHT");
            sr("END:VTIMEZONE");
        }



        public string Generate_p56_Calendar(string strPIDs, string strQuery_Me, string strP57IDs, string strClosed, DateTime d1, DateTime d2)
        {
            sr_vcalendar_header();
            sr_timezone();

            var mq = new BO.myQueryP56() { global_d1 = d1, global_d2 = d2,period_field= "p56PlanUntil" };
            if (strClosed == "0")
                mq.IsRecordValid = false;
            else
                mq.IsRecordValid = null;

            mq.MyRecordsDisponible = true;
            if (!string.IsNullOrEmpty(strPIDs))
            {
                mq.SetPids(strPIDs);
            }



            if (strQuery_Me == "1")
                mq.j02id = _Factory.CurrentUser.j02ID;

            if (strQuery_Me == "2")
                mq.j02id_owner = _Factory.CurrentUser.j02ID;

            if (!string.IsNullOrEmpty(strP57IDs))
                mq.explicit_sqlwhere = "a.p57ID IN (" + strP57IDs + ")";

            var lis = _Factory.p56TaskBL.GetList(mq).Where(p => p.p56PlanUntil != null);
            bool bolShowP57Name = false;
            this.ReturnEventRows = lis.Count();
            if (lis.Count() > 1 && lis.Select(p => p.p57ID).Distinct().Count() > 1)
            {
                bolShowP57Name = true;
            }


            foreach (var c in lis)
            {
                p56_record(c, bolShowP57Name);
            }


            sr("END:VCALENDAR");

            return _sb.ToString();
        }

        


        private void p56_record(BO.p56Task cRec, bool bolShowP57Name)
        {
            sr("BEGIN:VEVENT");

            {
                var withBlock = cRec;
                var lisX69 = _Factory.x67EntityRoleBL.GetList_X69(356, cRec.pid);

                string strName = withBlock.p56Name;
                if (withBlock.p57ID > 0 & bolShowP57Name)
                    strName += " [" + withBlock.p57Name + "]";
                List<string> memos = new List<string>();

                if (withBlock.b02ID > 0)
                    memos.Add("Aktuální stav: " + withBlock.b02Name);

                if (!string.IsNullOrEmpty(withBlock.Client))
                    memos.Add("Klient: " + withBlock.Client);

                if (!string.IsNullOrEmpty(withBlock.ProjectWithClient))
                    memos.Add("Projekt: " + withBlock.p41Name);

                if (!string.IsNullOrEmpty(withBlock.ReceiversInLine))
                    memos.Add("Řešitelé: " + withBlock.ReceiversInLine);

                if (!string.IsNullOrEmpty(withBlock.p56Description))
                {
                    memos.Add(withBlock.p56Description.Replace(Constants.vbCrLf, @" \n"));

                }

                string strDescription = "";
                if (memos.Count > 0)
                    strDescription = string.Join(@"\n", memos);



                sr("UID:p56-" + withBlock.pid.ToString());


                sr("DTSTAMP:" + Convert.ToDateTime(withBlock.DateInsert).ToUniversalTime().ToString("yyyyMMddTHHmmssZ"));

                sr("DTSTART:" + BO.BAS.ObjectDate2String(withBlock.p56PlanUntil, "yyyyMMdd"));
                var d = new DateTime(Convert.ToDateTime(withBlock.p56PlanUntil).Year, Convert.ToDateTime(withBlock.p56PlanUntil).Month, Convert.ToDateTime(withBlock.p56PlanUntil).Day).AddDays(1);
                sr("DTEND:" + BO.BAS.ObjectDate2String(d, "yyyyMMdd"));



                if (lisX69.Count() > 0)
                {
                    foreach (var c in lisX69.Where(p => p.j02ID > 0))
                    {
                        {
                            var withBlock1 = _lisJ02.Where(p => p.pid == c.j02ID).First();
                            sr($"ATTENDEE;CN={withBlock1.FullNameAsc};PARTSTAT=NEEDS-ACTION;ROLE=REQ-PARTICIPANT:mailto:{withBlock1.j02Email}");
                        }
                    }
                    foreach (var c in lisX69.Where(p => p.j11ID > 0))
                    {
                        foreach (var cc in _Factory.j02PersonBL.GetList(new BO.myQueryJ02() { j11id = c.j11ID }))
                        {
                            sr($"ATTENDEE;CN={cc.FullNameAsc};PARTSTAT=NEEDS-ACTION;ROLE=REQ-PARTICIPANT:mailto:{cc.j02Email}");
                        }

                    }
                }
                if (!string.IsNullOrEmpty(strName))
                {
                    sr("SUMMARY:" + strName);
                }


                if (!string.IsNullOrEmpty(strDescription))
                {
                    sr("DESCRIPTION:" + strDescription);
                }


                if (_lisJ02.Where(p => p.pid == withBlock.j02ID_Owner).Count() > 0)
                {
                    {
                        var withBlock1 = _lisJ02.Where(p => p.pid == withBlock.j02ID_Owner).First();
                        sr($"ORGANIZER;CN={ withBlock1.FullNameAsc}:mailto:{withBlock1.j02Email}");
                    }
                }

                sr("TRANSP:OPAQUE");
            }

            sr("END:VEVENT");
        }



        public string Generate_p31_Calendar(string strPIDs, string strP32IDs, string strP41IDs, DateTime d1, DateTime d2)
        {
            sr_vcalendar_header();
            sr_timezone();

            var mq = new BO.myQueryP31() { IsRecordValid = true,global_d1=d1,global_d2=d2,period_field= "p31Date" };


            mq.MyRecordsDisponible = true;
            

            if (!string.IsNullOrEmpty(strPIDs))
            {
                mq.SetPids(strPIDs);
            }


            if (!string.IsNullOrEmpty(strP32IDs))
                mq.p32ids = BO.BAS.ConvertString2ListInt(strP32IDs);
            else
                mq.tabquery = "time";

            if (!string.IsNullOrEmpty(strP41IDs))
                mq.p41ids = BO.BAS.ConvertString2ListInt(strP41IDs);


            mq.TopRecordsOnly = 5000;

            var lis = _Factory.p31WorksheetBL.GetList(mq);
            bool bolRenderClient = true;
            this.ReturnEventRows = lis.Count();

            if (lis.Count() > 1 && lis.Select(p => p.p28ID_Client).Distinct().Count() <= 1)
                bolRenderClient = false;

            foreach (var c in lis)
            {
                p31_record(c, bolRenderClient);
            }
                
           
            sr("END:VCALENDAR");

            return _sb.ToString();
        }
        private void p31_record(BO.p31Worksheet rec, bool bolRenderClient)
        {
            sr("BEGIN:VEVENT");


            string strName = rec.Person;
            if (rec.p33ID == BO.p33IdENUM.Cas)
            {
                strName += " [" + rec.p31HHMM_Orig + "]";
            }


            List<string> memos = new List<string>();
            if (rec.p28ID_Client > 0 && bolRenderClient)
            {
                memos.Add("Klient: " + rec.ClientName);
            }

            memos.Add("Projekt: " + rec.p41Name);
            if (rec.p56ID > 0)
            {
                memos.Add("Úkol: " + rec.p56Name);
            }

            memos.Add("Aktivita: " + rec.p32Name);

            memos.Add(BO.BAS.OM2(rec.p31Text, 50).Replace(Constants.vbCrLf, @" \n"));


            sr("UID:p31-" + rec.pid.ToString());
            sr("DTSTAMP:" + Convert.ToDateTime(rec.DateInsert).ToUniversalTime().ToString("yyyyMMddTHHmmssZ"));

            if (rec.TimeFrom == rec.TimeUntil)
            {
                sr("DTSTART:" + BO.BAS.ObjectDate2String(rec.p31Date, "yyyyMMdd"));
                var d = new DateTime(Convert.ToDateTime(rec.p31Date).Year, Convert.ToDateTime(rec.p31Date).Month, Convert.ToDateTime(rec.p31Date).Day).AddDays(1);
                sr("DTEND:" + BO.BAS.ObjectDate2String(d, "yyyyMMdd"));
            }
            else
            {
                sr("DTSTART:" + Convert.ToDateTime(rec.p31DateTimeFrom_Orig).ToUniversalTime().ToString("yyyyMMddTHHmmssZ"));
                sr("DTEND:" + Convert.ToDateTime(rec.p31DateTimeUntil_Orig).ToUniversalTime().ToString("yyyyMMddTHHmmssZ"));
            }

            sr("SUMMARY:" + strName);
            sr("DESCRIPTION:" + string.Join(@"\n", memos));


            if (_lisJ02.Where(p => p.pid == rec.j02ID).Count() > 0)
            {
                {
                    var withBlock1 = _lisJ02.Where(p => p.pid == rec.j02ID).First();
                    sr(string.Format("ATTENDEE;CN={0};PARTSTAT=NEEDS-ACTION;ROLE=REQ-PARTICIPANT:mailto:{1}", withBlock1.FullNameAsc, withBlock1.j02Email));

                    sr(string.Format("ORGANIZER;CN={0}:mailto:{1}", withBlock1.FullNameAsc, withBlock1.j02Email));
                }
            }


            sr("TRANSP:OPAQUE");

            sr("END:VEVENT");
        }

    }
}
