using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public static class basTime
    {
        public static int ConvertTimeToSeconds(string strTime)
        {
            try
            {
                double dblTime = System.Convert.ToDouble(strTime);
                return System.Convert.ToInt32(dblTime * 60 * 60);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            string[] arr = strTime.Split(":");
            int lngMinutes = 0;
            int lngHours = 0;
            if (arr.Length < 2)
                return 0;

            if (BO.BAS.InInt(arr[0]) != 0)
                lngHours = System.Convert.ToInt32(arr[0]);

            if (BO.BAS.InInt(arr[1]) != 0)
                lngMinutes = System.Convert.ToInt32(arr[1]);


            return lngHours * 60 * 60 + lngMinutes * 60;
        }

        public static double ShowAsDec(string strTime, double dblRetWithPrec = 0, int lngMinTimeUnit = 0)
        {
            // strTime je ve formátu hh:mm
            // fce vrací čas z výrazu hh:mm na decadické číslo
            int lngSec = ConvertTimeToSeconds(strTime);
            if (lngMinTimeUnit > 0)
                lngSec = RoundSeconds(lngSec, lngMinTimeUnit);
            return System.Convert.ToDouble(lngSec) / 60 / 60;
        }

        public static int RoundSeconds(int lngSeconds, int lngMinTimeSecUnit)
        {
            // zaokrouhlí sekundy na jednotky lngMinTimeSecUnit - vše NAHORU!!
            if (lngMinTimeSecUnit == 0)
                return lngSeconds; // nezaokrouhlovat

            int ret;
            int lng;
            double dbl = System.Convert.ToDouble(lngSeconds) / System.Convert.ToDouble(lngMinTimeSecUnit);

            if (System.Convert.ToInt32(dbl) != dbl || dbl == 0)
            {
                // je třeba zaokrouhlovat
                lng = Convert.ToInt32(Math.Round(dbl + 0.5, 0));
                ret = lng * lngMinTimeSecUnit;


                if (ret == 0)
                    ret = lngMinTimeSecUnit;
            }
            else
                ret = lngSeconds;


            return ret;
        }

        public static string FormatNumeric(double hours,bool bolShowAsHHMM)
        {
            if (hours == 0) return null;
            if (bolShowAsHHMM)
            {
                return ShowAsHHMM(hours.ToString());
            }
            else
            {
                return string.Format("{0:#,0.00}", hours);
            }
        }

        public static string ShowAsHHMM(string strTime, int lngMinTimeUnit = 0)
        {
            // strTime jsou dekadické hodiny
            // fce vrací čas z dekadického hodinového výrazu na hh:mm
            string strHHMM;
            int lngSec = ConvertTimeToSeconds(strTime);            
            if (lngMinTimeUnit > 0)
            {
                lngSec = RoundSeconds(lngSec, lngMinTimeUnit);
            }
            if (lngSec == 0) return null;

            strHHMM = GetTimeFromSeconds(System.Convert.ToDouble(lngSec));
            return strHHMM;
        }

        public static string GetTimeFromSeconds(double tim, bool bolIncludeSeconds = false)
        {
            
            // tim... časový úsek vyjádřený v sekundách

            if (tim == 0.00)
            {
                if (!bolIncludeSeconds)
                    return "00:00";
                else
                    return "00:00:00";
            }
            string cmin;
            string chod;
            string znam;

            tim = Convert.ToDouble(Convert.ToInt32(tim));


            if (tim < 0)
            {
                znam = "-";
                tim = Math.Abs(tim);
            }
            else
                znam = "";

            double hod = Convert.ToDouble(Convert.ToInt32(tim) / 3600);    //zaokrouhlí na celé hodiny dolů!

            if (hod > 0)
                tim = tim - (hod * 3600);

            int Min = Convert.ToInt32(tim) / 60;
            if (Min > 0)
                tim = tim - (Min * 60);




            if (hod < 10)
                chod = "0" + hod.ToString().Trim();
            else
                chod = hod.ToString().Trim();

            if (Min < 10)
                cmin = "0" + Min.ToString().Trim();
            else
                cmin = Min.ToString().Trim();



            if (bolIncludeSeconds)
            {
                string csec = "";
                if (tim < 10)
                    csec = "0" + tim.ToString().Trim();
                else
                    csec = tim.ToString().Trim();

                return znam + chod + ":" + cmin + ":" + csec;
            }
            else
                return znam + chod + ":" + cmin;
        }

        public static double GetDecTimeFromSeconds(double tim, double dblRetPrec = 0, int lngRoundToDecimals = 0)
        {
            if (lngRoundToDecimals == 0)
                lngRoundToDecimals = 3;
            // tim... časový úsek vyjádřený v sekundách
            if (tim == 0)
            {
                return 0;
            }

           
            
           
           double dbl = Math.Round(tim / 60 / 60, lngRoundToDecimals);
            dblRetPrec = Math.Round(tim / 60 / 60, 6);
            return dbl;
        }

        public static string DurationFormatted(DateTime datFrom, DateTime datTo, string strFormat = null)
        {
            TimeSpan span = datTo - datFrom;

            string[] values = new string[4];  //4 slots: days, hours, minutes, seconds
            StringBuilder readableTime = new StringBuilder();

            if (span.Days > 0)
            {
                values[0] = span.Days.ToString() + "d"; //day

                readableTime.Append(values[0]);
                readableTime.Append(" ");
            }
            else
                values[0] = String.Empty;


            if (span.Hours > 0)
            {
                values[1] = span.Hours.ToString() + "h";  //hour

                readableTime.Append(values[1]);
                readableTime.Append(" ");

            }
            else
                values[1] = string.Empty;

            if (span.Minutes > 0)
            {
                values[2] = span.Minutes.ToString() + "m";  //minute

                readableTime.Append(values[2]);
                readableTime.Append(" ");
            }
            else
                values[2] = string.Empty;

            if (span.Seconds > 0)
            {
                values[3] = span.Seconds.ToString() + "s";  //second

                readableTime.Append(values[3]);
            }
            else
                values[3] = string.Empty;


            return readableTime.ToString();
        }

        public static DateTime GetDateFromDecimal(double decHours)
        {
            double intMinutes = decHours * 60;
            return DateTime.Today.AddMinutes(intMinutes);
        }
        public static double GetDecimalFromDate(DateTime dat)
        {
            double intHours = dat.Hour;
            double intMinutes = dat.Minute;
            return intHours + intMinutes / (double)60;
        }
    }


}
