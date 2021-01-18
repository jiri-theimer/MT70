using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class TimeSupport
    {
        public int ConvertTimeToSeconds(string strTime)
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
            if (arr.Length <2)
                return 0;

            if (BO.BAS.InInt(arr[0]) != 0)
                lngHours = System.Convert.ToInt32(arr[0]);
           
            if (BO.BAS.InInt(arr[1]) !=0)
                lngMinutes = System.Convert.ToInt32(arr[1]);


            return lngHours * 60 * 60 + lngMinutes * 60;
        }
        //public int ConvertMobileNumberToSeconds(string strValue)
        //{
        //    if (strValue.Contains(",") | strValue.Contains("."))
        //    {
        //        string[] n = strValue.Split(strValue.Contains(",") ? "," : ".");
        //        int intSecs = System.Convert.ToInt32(n[0]) * 60 * 60;
        //        if (n[1].Length > 15)
                    
        //            n[1] = BO.BAS.LeftString(n[1], 15);
        //        intSecs += (60 * 60 * (System.Convert.ToDouble(n[1]) / System.Convert.ToDouble("1" + BO.BAS.LeftString("000000000000000", n[1].Length))));
        //        return intSecs;
        //    }
        //    else
        //        return ConvertTimeToSeconds(strValue);
        //}

        public double ShowAsDec(string strTime, double dblRetWithPrec = 0, int lngMinTimeUnit = 0)
        {
            // strTime je ve formátu hh:mm
            // fce vrací čas z výrazu hh:mm na decadické číslo
            int lngSec = ConvertTimeToSeconds(strTime);
            if (lngMinTimeUnit > 0)
                lngSec = RoundSeconds(lngSec, lngMinTimeUnit);
            return System.Convert.ToDouble(lngSec) / 60 / 60;
        }

        public int RoundSeconds(int lngSeconds, int lngMinTimeSecUnit)
        {
            // zaokrouhlí sekundy na jednotky lngMinTimeSecUnit - vše NAHORU!!
            if (lngMinTimeSecUnit == 0)
                return lngSeconds; // nezaokrouhlovat

            int ret;                
            int lng;
            double dbl = System.Convert.ToDouble(lngSeconds) / System.Convert.ToDouble(lngMinTimeSecUnit);

            if (System.Convert.ToDouble(dbl) != dbl | dbl == 0)
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

        public string ShowAsHHMM(string strTime, int lngMinTimeUnit = 0)
        {
            int lngSec;
            // strTime jsou dekadické hodiny
            // fce vrací čas z dekadického hodinového výrazu na hh:mm
            string strHHMM;
            lngSec = ConvertTimeToSeconds(strTime);
            if (lngMinTimeUnit > 0)
                lngSec = RoundSeconds(lngSec, lngMinTimeUnit);
            strHHMM = GetTimeFromSeconds(System.Convert.ToDouble(lngSec));
            return strHHMM;
        }

        public string GetTimeFromSeconds(double tim, bool bolIncludeSeconds = false)
        {
            double hod;
            string cmin;
            string chod;
            string znam;
            // tim... časový úsek vyjádřený v sekundách
            int Min;

            if (tim == 0)
            {
                if (!bolIncludeSeconds)
                    return "00:00";
                else
                    return "00:00:00";
            }
            tim = Convert.ToDouble(Convert.ToInt32(tim));

            
            if (tim < 0)
            {
                znam = "-";
                tim = Math.Abs(tim);
            }
            else
                znam = "";

            // 'hod = Int(tim / 90000)
            hod = Convert.ToDouble(Convert.ToInt32(tim / 3600));

            // 'If hod > 0 Then tim = tim - (hod * 90000)
            if (hod > 0)
                tim = tim - (hod * 3600);

            Min = Convert.ToInt32(tim / 60);
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

        public double GetDecTimeFromSeconds(double tim, double dblRetPrec = 0, int lngRoundToDecimals = 0)
        {
            if (lngRoundToDecimals == 0)
                lngRoundToDecimals = 3;
            // tim... časový úsek vyjádřený v sekundách
            if (tim == 0)
            {
                return 0;
            }

            int lng = 0;
            double dbl = 0;

            lng = System.Convert.ToInt32(tim);
            dbl = Math.Round(tim / 60 / 60, lngRoundToDecimals);
            dblRetPrec = Math.Round(tim / 60 / 60, 6);
            return dbl;
        }

        public string KolikZbyvaCasu(DateTime datFrom, DateTime datTo, int intMaxPocetUrovni, bool bolShowZeroUnits)
        {
            string s = "";
            int x = 0;
            TimeSpan dur = datFrom - datTo;
            if (bolShowZeroUnits || dur.TotalDays > 0)
            {
                s = dur.TotalDays.ToString() + "d";
                x += 1;
            }
            
            if (bolShowZeroUnits || (dur.TotalHours-dur.TotalDays*24) > 0)
            {
                
                s = (dur.TotalHours-dur.TotalDays*24).ToString() + "h";
                x += 1;
            }



            if (x < intMaxPocetUrovni)
            {
                if (bolShowZeroUnits || (dur.TotalMinutes-(dur.TotalHours*60)) > 0)
                {

                    s = (dur.TotalMinutes - dur.TotalHours * 60).ToString() + "m";
                    x += 1;
                }
               
            }

            return s.Trim();
        }

        public DateTime GetDateFromDecimal(double decHours)
        {
            double intMinutes = decHours * 60;
            return DateTime.Today.AddMinutes(intMinutes);
        }

        public double GetDecimalFromDate(DateTime dat)
        {
            double intHours = dat.Hour;
            double intMinutes = dat.Minute;
            return intHours + intMinutes / (double)60;
        }
    }

}
