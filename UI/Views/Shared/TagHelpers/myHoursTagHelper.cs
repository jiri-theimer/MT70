using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UI.Views.Shared.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("myhours")]
    public class myHoursTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("placeholder")]
        public string PlaceHolder { get; set; }

        [HtmlAttributeName("hoursentryformat")]
        public string HoursEntryFormat { get; set; }    //hodnoty: N/T/M  (dekadické číslo/hh:mm/minuty)

        [HtmlAttributeName("hoursentryflag")]
        public int HoursEntryFlag { get; set; }

        [HtmlAttributeName("explicitintervals")]
        public string ExplicitIntervals { get; set; }

        [HtmlAttributeName("showsettingbutton")]
        public bool ShowSettingButton { get; set; }



        private System.Text.StringBuilder _sb;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            _sb = new System.Text.StringBuilder();
            Handle_Intervals();

            if (this.ShowSettingButton)
            {
                sb("<div class='input-group'>");
            }
            
            string strControlID = this.For.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_");

            string strValue = null;
            if (this.For.Model != null)
            {
                strValue= this.For.Model.ToString();
            }
            if (strValue=="0" || strValue == "00:00" || string.IsNullOrEmpty(strValue))
            {
                strValue = null;
            }
            else
            {
                if (this.HoursEntryFormat == "T" && !strValue.Contains(":"))
                {
                    var ct = new BO.CLS.TimeSupport();
                    strValue = ct.ShowAsHHMM(strValue);
                }
                if (strValue.Length>5 && !strValue.Contains(":"))
                {
                    var ct = new BO.CLS.TimeSupport();
                    strValue = ct.ShowAsHHMM(strValue);
                }
            }

            sb(string.Format("<input id='{0}' class='form-control' placeholder='{1}' autocomplete='off' value='{2}' name='{3}'/>", strControlID, this.PlaceHolder, strValue, this.For.Name));

            if (this.ShowSettingButton)
            {
                sb("<button type='button' tabindex='-1' class='btn btn-sm btn-outline-secondary' onclick='myhours_setting()'><span class='k-icon k-i-clock'></span></button>");
                sb("</div>");
            }
            
            sb("");
            sb("");


            sb("<script type='text/javascript'>");
            _sb.Append(string.Format("var c{0}=", strControlID));
            _sb.Append("{");
            _sb.Append(string.Format("controlid: '{0}',entryformat: '{1}',intervals:'{2}'", strControlID, this.HoursEntryFormat,this.ExplicitIntervals));
            _sb.Append("};");

            sb("");
            sb(string.Format("myhours_init(c{0});", strControlID));

            sb("");
            sb("</script>");

            output.Content.AppendHtml(_sb.ToString());


        }

        private void Handle_Intervals()
        {
            if (!string.IsNullOrEmpty(this.ExplicitIntervals))
            {
                return;
            }
            this.ExplicitIntervals = "";

            var ct = new BO.CLS.TimeSupport();
            double dblStart = 30.00;double dblKrat = 8.00;
            if (BO.BAS.bit_compare_or(this.HoursEntryFlag, 2))
            {
                dblStart = 30.00; dblKrat = 8.00;
            }
            if (BO.BAS.bit_compare_or(this.HoursEntryFlag, 4))
            {
                dblStart = 60.00;dblKrat = 10.00;
            }
            if (BO.BAS.bit_compare_or(this.HoursEntryFlag, 8)) dblStart = 5.00;
            if (BO.BAS.bit_compare_or(this.HoursEntryFlag, 16)) dblStart = 10.00;
            if (BO.BAS.bit_compare_or(this.HoursEntryFlag, 32)) dblStart = 6.00;
            if (BO.BAS.bit_compare_or(this.HoursEntryFlag, 64))
            {
                dblStart = 15.00; dblKrat = 4.00;
            }
            
            if (dblStart == 5 || dblStart == 10 || dblStart == 6) dblKrat = 3.00;

            for (double i = dblStart; i <= dblKrat * 60; i += dblStart)
            {
                if (i > dblStart)
                {
                    this.ExplicitIntervals += "|";
                }
                switch (this.HoursEntryFormat)
                {
                    case "T":
                        this.ExplicitIntervals += ct.ShowAsHHMM((i / 60.00).ToString());
                        break;
                    case "M":
                        this.ExplicitIntervals += i.ToString();
                        break;
                    default:
                        this.ExplicitIntervals += (i / 60.00).ToString();
                        break;
                }
                
            }

        }


        private void sb(string s)
        {
            _sb.AppendLine(s);
        }
    }
}
