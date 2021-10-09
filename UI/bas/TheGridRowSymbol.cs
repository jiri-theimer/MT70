﻿using System;
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
                                return "<span class='material-icons-outlined-nosize' style='color:pink;'>south</span>";
                            case 2: //korekce: viditelný odpis
                                return "<span class='material-icons-outlined-nosize' style='color:red;'>south</span>";
                            case 3: //korekce: skrytý odpis
                                return "<span class='material-icons-outlined-nosize' style='color:brown;'>south</span>";
                            case 4: //korekce: fakturovat
                                return "<span class='material-icons-outlined-nosize' style='color:green;'>import_export</span>";
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
                                return "<span class='material-icons-outlined-nosize' style='color:green;'>approval</span>";
                            case 6:
                                return "<span class='material-icons-outlined-nosize' style='color:pink;'>approval</span>";
                            case 3:
                                return "<span class='material-icons-outlined-nosize' style='color:brown;'>approval</span>";
                            case 2:
                                return "<span class='material-icons-outlined-nosize' style='color:red;'>approval</span>";
                            case 7:
                                return "<span class='material-icons-outlined-nosize' style='color:gold;'>approval</span>";

                        }

                    }
                    if (intP71ID == 2)
                    {
                        return "<span class='material-icons-outlined-nosize' style='color:red;'>thumb_down_off_alt</span>"; //neschváleno
                    }

                }

            }
            else
            {
                if (Convert.ToBoolean(dbrow["p91IsDraft"]))
                {
                    return "<span class='material-icons-outlined-nosize'>push_pin</span>";
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
                                return "<span class='material-icons-outlined' style='color:pink;'>south</span>";
                            case BO.p72IdENUM.ViditelnyOdpis: //korekce: viditelný odpis
                                return "<span class='material-icons-outlined' style='color:red;'>south</span>";
                            case BO.p72IdENUM.SkrytyOdpis: //korekce: skrytý odpis
                                return "<span class='material-icons-outlined' style='color:brown;'>south</span>";
                            case BO.p72IdENUM.Fakturovat: //korekce: fakturovat
                                return "<span class='material-icons-outlined' style='color:green;'>import_export</span>";
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
                                return "<span class='material-icons-outlined' style='color:green;'>approval</span>";
                            case BO.p72IdENUM.ZahrnoutDoPausalu:
                                return "<span class='material-icons-outlined' style='color:pink;'>approval</span>";
                            case BO.p72IdENUM.SkrytyOdpis:
                                return "<span class='material-icons-outlined' style='color:brown;'>approval</span>";
                            case BO.p72IdENUM.ViditelnyOdpis:
                                return "<span class='material-icons-outlined' style='color:red;'>approval</span>";

                        }

                    }
                    if (rec.p71ID == BO.p71IdENUM.Neschvaleno)
                    {
                        return "<span class='material-icons-outlined' style='color:red;'>thumb_down_alt</span>"; //neschváleno
                    }

                }

            }
            else
            {
                
                if (rec.p91IsDraft)
                {
                    return "<span class='material-icons-outlined-nosize'>push_pin</span>";
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
                    return "<span class='material-icons-outlined-nosize'>push_pin</span>";
                }
            }
            else
            {
                if (BO.BAS.bit_compare_or(lockflag, 8))
                {
                    return "<span class='material-icons-outlined-nosize' style='color:magenta;'>lock</span>";

                }
                else
                {
                    if (BO.BAS.bit_compare_or(lockflag, 4))
                    {
                        return "<span class='material-icons-outlined-nosize' style='color:blue;'>lock</span>";
                    }
                    else
                    {
                        if (BO.BAS.bit_compare_or(lockflag, 2))
                        {
                            return "<span class='material-icons-outlined-nosize'>lock</span>";   //zámek ceny vyúčtování
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
