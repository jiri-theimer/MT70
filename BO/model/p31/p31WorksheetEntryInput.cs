﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class p31WorksheetEntryInput:BaseBO
    {
        public int p41ID { get; set; }
        public int p56ID { get; set; }
        public int j02ID { get; set; }
        public int p32ID { get; set; }
        public int p34ID { get; set; }
        public int p28ID_Supplier { get; set; }
        public int j02ID_ContactPerson { get; set; }

        public string Value_Orig { get; set; }     // vstupní čas nebo počet kusovníku
        public string Value_Trimmed { get; set; }  // po korekci vstupní čas nebo počet kusovníku
        public p31HoursEntryFlagENUM p31HoursEntryflag { get; set; }
        public DateTime p31Date { get; set; }
        public DateTime? p31DateUntil { get; set; }
        public string TimeFrom { get; set; }
        public string TimeUntil { get; set; }
        public string p31Text { get; set; }
        public double VatRate_Orig { get; set; }
        public BO.x15IdEnum x15ID { get; set; } = x15IdEnum.Nic;

        public string Value_Orig_Entried { get; set; }
        public double ManualFee { get; set; }
        public double p31Calc_Pieces { get; set; }
        public double p31Calc_PieceAmount { get; set; }
        public int p35ID { get; set; }
        public string p31Code { get; set; }

        public int p49ID { get; set; }
        public int j19ID { get; set; }
        
        private double _p31Value_Orig { get; set; }
        public DateTime? p31ValidUntil { get; set; }
        public double p31MarginHidden { get; set; }   // nové pole verze 6
        public double p31MarginTransparent { get; set; }  // nové pole verze 6
        public string p31PostRecipient { get; set; }  // nové pole verze 6
        public string p31PostCode { get; set; }   // nové pole verze 6
        public int p31PostFlag { get; set; }
        public DateTime? p31TimerTimestamp { get; set; }
        public int p31RecordSourceFlag { get; set; }   // 1-záznam pořízený z mobilní aplikace
        public double Value_OffBilling { get; set; }

        public double p31Value_Orig
        {
            get
            {
                return _p31Value_Orig;
            }
        }
        private double _p31Value_Trimmed { get; set; }
        public double p31Value_Trimmed
        {
            get
            {
                return _p31Value_Trimmed;
            }
        }
        private string _p31HHMM_Orig { get; set; }
        public string p31HHMM_Orig
        {
            get
            {
                return _p31HHMM_Orig;
            }
        }
        public string p31HHMM_Trimmed { get; set; }

        public BO.p72IdENUM p72ID_AfterTrimming { get; set; } = p72IdENUM._NotSpecified;

        private string _Error { get; set; }
        public string ErrorMessage
        {
            get
            {
                return _Error;
            }
        }

        public void SetError(string errormessage)
        {
            _Error = errormessage;
        }

        private int _p31Minutes_Orig { get; set; }
        public int p31Minutes_Orig
        {
            get
            {
                return _p31Minutes_Orig;
            }
        }
        private int _p31Minutes_Trimmed { get; set; }
        public int p31Minutes_Trimmed
        {
            get
            {
                return _p31Minutes_Trimmed;
            }
        }

        private DateTime? _p31DateTimeFrom_Orig { get; set; }
        public DateTime? p31DateTimeFrom_Orig
        {
            get
            {
                return _p31DateTimeFrom_Orig;
            }
        }
        private DateTime? _p31DateTimeUntil_Orig { get; set; }
        public DateTime? p31DateTimeUntil_Orig
        {
            get
            {
                return _p31DateTimeUntil_Orig;
            }
        }

        public double Amount_WithoutVat_Orig { get; set; }
        public double Amount_WithVat_Orig { get; set; }
        public double Amount_Vat_Orig { get; set; }

        public int j27ID_Billing_Orig { get; set; }

        private double _p31Amount_WithoutVat_Orig { get; set; }
        public double p31Amount_WithoutVat_Orig
        {
            get
            {
                return _p31Amount_WithoutVat_Orig;
            }
        }
        private double _p31Amount_WithVat_Orig { get; set; }
        public double p31Amount_WithVat_Orig
        {
            get
            {
                return _p31Amount_WithVat_Orig;
            }
        }
        private double _p31Amount_Vat_Orig { get; set; }
        public double p31Amount_Vat_Orig
        {
            get
            {
                return _p31Amount_Vat_Orig;
            }
        }

        private int _pid { get; set; }
        public int PID
        {
            get
            {
                return _pid;
            }
        }

        public void SetPID(int intPID)
        {
            _pid = intPID;
        }

        public bool ValidateEntryKusovnik()
        {
            // kusovníkový úkon
            _p31Value_Orig = BO.BAS.InDouble(this.Value_Orig);
            
            if (_p31Value_Orig == 0)
            {
                _Error = "Počet nesmí být NULA.";
                return false;
            }
            return true;
        }
        public bool ValidateTrimming(BO.p72IdENUM status, string strValue, BO.p33IdENUM p33id)
        {
            this.p72ID_AfterTrimming = status;
            switch (this.p72ID_AfterTrimming)
            {
                case p72IdENUM.Fakturovat:
                    {
                        switch (p33id)
                        {
                            case p33IdENUM.Cas:
                                {
                                    
                                    _p31Minutes_Trimmed = BO.basTime.ConvertTimeToSeconds(strValue) / (int)60;
                                    if (_p31Minutes_Trimmed == 0)
                                    {
                                        _Error = "Pro korekci statusu [Fakturovat] musí být hodiny větší než nula.";
                                        return false;
                                    }
                                    _p31Value_Trimmed = _p31Minutes_Trimmed / (double)60;
                                    this.p31HHMM_Trimmed = BO.basTime.ShowAsHHMM(strValue);
                                    break;
                                }

                            case p33IdENUM.PenizeBezDPH:
                            case p33IdENUM.PenizeVcDPHRozpisu:
                                {
                                    if (BO.BAS.InDouble(strValue) == 0)
                                    {
                                        _Error = "Pro korekci statusu [Fakturovat] nesmí být částka k fakturaci nulová!";
                                        return false;
                                    }
                                    _p31Value_Trimmed = BO.BAS.InDouble(strValue);
                                    break;
                                }
                        }

                        return true;
                    }

                default:
                    {
                        _p31Minutes_Trimmed = 0;
                        _p31Value_Trimmed = 0;
                        this.p31HHMM_Trimmed = "";
                        this.Value_Trimmed = "";
                        return true;
                    }
            }
        }
        public bool ValidateEntryTime(int intRound2Minutes)
        {
            int intSeconds_Orig = 0;
            // časový úkon
            
            switch (this.p31HoursEntryflag)
            {
                case BO.p31HoursEntryFlagENUM.Hodiny:
                    {
                        if (this.p31RecordSourceFlag > 0)
                            // hodiny zadané mimo web aplikaci - desetinná čárka nebo tečka podle regional settings web.config                   
                            intSeconds_Orig = BO.basTime.ConvertTimeToSeconds(this.Value_Orig);
                        else
                            intSeconds_Orig = BO.basTime.ConvertTimeToSeconds(this.Value_Orig);


                        if (!(string.IsNullOrEmpty(this.TimeFrom) || this.TimeFrom == "00:00" || string.IsNullOrEmpty(this.TimeUntil) || this.TimeUntil == "00:00"))
                        {
                            if (this.p31RecordSourceFlag == 0)
                            {
                                // pouze ve výchozí aplikaci, mobilní aplikace si to dělá sama
                                intSeconds_Orig = BO.basTime.ConvertTimeToSeconds(this.TimeUntil) - BO.basTime.ConvertTimeToSeconds(this.TimeFrom);
                                if (intSeconds_Orig == 0)
                                {
                                    intSeconds_Orig = BO.basTime.ConvertTimeToSeconds(this.Value_Orig);
                                    if (intSeconds_Orig == 0)
                                    {
                                        _Error = "Chybí [Hodiny].";
                                        
                                        return false;
                                    }
                                    this.TimeFrom = ""; this.TimeUntil = "";
                                }
                                if (intSeconds_Orig < 0)
                                {
                                    _Error = "[Čas do] je menší než [Čas od].";
                                   
                                    return false;
                                }
                            }


                            if (!string.IsNullOrEmpty(this.TimeFrom) && !string.IsNullOrEmpty(this.TimeUntil))
                            {
                                _p31DateTimeFrom_Orig = this.p31Date.AddSeconds(BO.basTime.ConvertTimeToSeconds(this.TimeFrom));
                                _p31DateTimeUntil_Orig = this.p31Date.AddSeconds(BO.basTime.ConvertTimeToSeconds(this.TimeUntil));
                            }
                            else
                            {
                                _p31DateTimeFrom_Orig = null; _p31DateTimeUntil_Orig = null;
                            }
                        }

                        break;
                    }

                case BO.p31HoursEntryFlagENUM.Minuty:
                    {
                        intSeconds_Orig = BO.BAS.InInt(this.Value_Orig) * 60;
                        break;
                    }
            }
            if (intSeconds_Orig == 0 & (this.Value_Orig == "0" || this.Value_Orig == "" || this.Value_Orig == "00:00"))
            {
                _Error = "Čas úkonu nesmí být NULA.";               
                return false;
            }
            if (intSeconds_Orig == 0 && this.Value_Orig != "")
            {
                _Error = "Zadaný výraz není podporovaný zápis času.";               
                return false;
            }

            intSeconds_Orig = BO.basTime.RoundSeconds(intSeconds_Orig, 60 * intRound2Minutes);  // zaokrouhlení nahoru
            _p31Minutes_Orig = Convert.ToInt32(intSeconds_Orig / (double)60);
            _p31HHMM_Orig = BO.basTime.GetTimeFromSeconds(intSeconds_Orig);
            _p31Value_Orig = System.Convert.ToDouble(intSeconds_Orig / (double)60 / 60);

            return true;
        }

        public void RecalcEntryAmount(double dblWithoutVat, double dblValidatedVatRate)
        {
            _p31Amount_WithoutVat_Orig = dblWithoutVat; _p31Value_Orig = dblWithoutVat;

            _p31Amount_Vat_Orig = _p31Amount_WithoutVat_Orig * dblValidatedVatRate / 100;
            _p31Amount_WithVat_Orig = _p31Amount_WithoutVat_Orig + _p31Amount_Vat_Orig;
        }

        public void SetAmounts()
        {
            if (this.Amount_WithoutVat_Orig == 0 & _p31Value_Orig != 0)
                this.Amount_WithoutVat_Orig = _p31Value_Orig;

            if (this.Amount_WithoutVat_Orig != 0 & _p31Value_Orig == 0)
                _p31Value_Orig = this.Amount_WithoutVat_Orig;

            _p31Amount_WithoutVat_Orig = this.Amount_WithoutVat_Orig;
            _p31Amount_Vat_Orig = this.Amount_Vat_Orig;
            _p31Amount_WithVat_Orig = this.Amount_WithVat_Orig;
        }

        public double p31Hours_Orig
        {
            get
            {
                return System.Convert.ToDouble(_p31Minutes_Orig) / 60;
            }
        }
        public double p31Hours_Trimmed
        {
            get
            {
                return System.Convert.ToDouble(_p31Minutes_Trimmed) / 60;
            }
        }
    }
}
