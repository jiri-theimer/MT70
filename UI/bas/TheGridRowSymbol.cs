using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;


namespace UI
{
    public static class TheGridRowSymbol
    {
        public static string p31_td_inner(System.Data.DataRow dbrow)
        {
            if (dbrow["p91ID"] == System.DBNull.Value)
            {
                if (dbrow["p71ID"] == System.DBNull.Value)
                {
                    if (dbrow["p72ID_AfterTrimming"] != System.DBNull.Value)
                    {
                        switch (Convert.ToInt32(dbrow["p72ID_AfterTrimming"]))
                        {
                            case 6: //korekce: paušál
                                return "<span class='k-icon k-i-arrow-down' style='color:pink;'></span>";
                            case 2: //korekce: viditelný odpis
                                return "<span class='k-icon k-i-arrow-down' style='color:red;'></span>";
                            case 3: //korekce: skrytý odpis
                                return "<span class='k-icon k-i-arrow-down' style='color:brown;'></span>";
                            case 4: //korekce: fakturovat
                                return "<span class='k-icon k-i-arrows-swap' style='color:green;'></span>";
                        }
                    }
                }
                else
                {
                    int intP71ID = Convert.ToInt32(dbrow["p71ID"]);
                    if (intP71ID == 1)
                    {
                        switch (Convert.ToInt32(dbrow["p72ID_AfterApprove"]))
                        {
                            case 4:
                                return "<span class='k-icon k-i-check-outline' style='color:green;'></span>";
                            case 6:
                                return "<span class='k-icon k-i-check-outline' style='color:pink;'></span>";
                            case 3:
                                return "<span class='k-icon k-i-check-outline' style='color:brown;'></span>";
                            case 2:
                                return "<span class='k-icon k-i-check-outline' style='color:red;'></span>";
                            case 7:
                                return "<span class='k-icon k-i-question' style='color:gold;'></span>";

                        }

                    }
                    if (intP71ID == 2)
                    {
                        return "<span class='k-icon k-i-minus-circle' style='color:red;'></span>"; //neschváleno
                    }

                }

            }
            else
            {
                if (Convert.ToBoolean(dbrow["p91IsDraft"]))
                {
                    return "<span class='k-icon k-i-pin'></span>";
                }
            }

            return null;
        }
        public static string p31_td_style(System.Data.DataRow dbrow)
        {
            if (dbrow["p91ID"] != System.DBNull.Value)
            {
                switch (Convert.ToInt32(dbrow["p70ID"]))
                {
                    case 4:
                        return "background-color:green;";
                    case 6:
                        return "background-color:pink;";
                    case 3:
                        return "background-color:brown;";
                    case 2:
                        return "background-color:red;";

                }
            }
            return null;
        }
        public static string p31_symbol(BO.p31Worksheet rec)    //symbol mimo grid z BO.p31Worksheet
        {
            return $"<div class='a{(int)rec.p70ID}'>" + p31_inner_symbol(rec) + "</div>";
        }
        private static string p31_inner_symbol(BO.p31Worksheet rec)
        {
            if (rec.p91ID == 0)
            {
                if (rec.p71ID==BO.p71IdENUM.Nic)
                {
                    if (rec.p72ID_AfterApprove != BO.p72IdENUM._NotSpecified)
                    {
                        switch (rec.p72ID_AfterApprove)
                        {
                            case BO.p72IdENUM.ZahrnoutDoPausalu: //korekce: paušál
                                return "<span class='k-icon k-i-arrow-down' style='color:pink;'></span>";
                            case BO.p72IdENUM.ViditelnyOdpis: //korekce: viditelný odpis
                                return "<span class='k-icon k-i-arrow-down' style='color:red;'></span>";
                            case BO.p72IdENUM.SkrytyOdpis: //korekce: skrytý odpis
                                return "<span class='k-icon k-i-arrow-down' style='color:brown;'></span>";
                            case BO.p72IdENUM.Fakturovat: //korekce: fakturovat
                                return "<span class='k-icon k-i-arrows-swap' style='color:green;'></span>";
                        }
                    }
                }
                else
                {
                    
                    if (rec.p71ID == BO.p71IdENUM.Schvaleno)
                    {
                        switch (rec.p72ID_AfterApprove)
                        {
                            case BO.p72IdENUM.Fakturovat:
                                return "<span class='k-icon k-i-check-outline' style='color:green;'></span>";
                            case BO.p72IdENUM.ZahrnoutDoPausalu:
                                return "<span class='k-icon k-i-check-outline' style='color:pink;'></span>";
                            case BO.p72IdENUM.SkrytyOdpis:
                                return "<span class='k-icon k-i-check-outline' style='color:brown;'></span>";
                            case BO.p72IdENUM.ViditelnyOdpis:
                                return "<span class='k-icon k-i-check-outline' style='color:red;'></span>";

                        }

                    }
                    if (rec.p71ID == BO.p71IdENUM.Neschvaleno)
                    {
                        return "<span class='k-icon k-i-minus-circle' style='color:red;'></span>"; //neschváleno
                    }

                }

            }
            else
            {
                
                if (rec.p91IsDraft)
                {
                    return "<span class='k-icon k-i-pin'></span>";
                }
            }

            return null;
        }
        public static string p91_td_inner(System.Data.DataRow dbrow)
        {
            int lockflag = Convert.ToInt32(dbrow["p91LockFlag"]);

            if (lockflag == 0)
            {
                if (Convert.ToBoolean(dbrow["p91IsDraft"]))
                {
                    return "<span class='k-icon k-i-pin'></span>";
                }
            }
            else
            {
                if (BO.BAS.bit_compare_or(lockflag, 8))
                {
                    return "<span class='k-icon k-i-lock' style='color:magenta;'></span>";

                }
                else
                {
                    if (BO.BAS.bit_compare_or(lockflag, 4))
                    {
                        return "<span class='k-icon k-i-lock' style='color:blue;'></span>";
                    }
                    else
                    {
                        if (BO.BAS.bit_compare_or(lockflag, 2))
                        {
                            return "<span class='k-icon k-i-lock'></span>";   //zámek ceny vyúčtování
                        }
                    }
                }
            }

            return null;

        }
        public static string p91_td_style(System.Data.DataRow dbrow)
        {
            if (!Convert.ToBoolean(dbrow["p91IsDraft"]))
            {
                if (Convert.ToDouble(dbrow["p91Amount_Debt"]) > 0)
                {
                    //existuje dluh
                    if (Convert.ToDateTime(dbrow["p91DateMaturity"]) < DateTime.Today)
                    {
                        //po splatnosti
                        if (Math.Abs(Convert.ToDouble(dbrow["p91Amount_TotalDue"]) - Convert.ToDouble(dbrow["p91Amount_Debt"])) < 10)
                        {
                            return "background-color:red;";     //zcela neuhrazeno
                        }
                        else
                        {
                            if (Math.Abs(Convert.ToDouble(dbrow["p91Amount_TotalDue"]) - Convert.ToDouble(dbrow["p91Amount_Debt"])) > 10)
                            {
                                return "background-color:pink;'";    //částečně uhrazeno
                            }
                        }
                    }
                    else
                    {
                        //ve splatnosti
                        if (Math.Abs(Convert.ToDouble(dbrow["p91Amount_TotalDue"]) - Convert.ToDouble(dbrow["p91Amount_Debt"])) < 10)
                        {
                            return "background-color:yellow;";
                        }
                    }
                }
            }

            return null;


        }
    }
}
