using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public static class basMenu
    {
        //vrátí HTML post menu


        public static string FlushResult_UL(List<MenuItem> menuitems, bool bolSupportIcons, bool bolRenderUlContainer,string source=null)
        {
            var sb = new System.Text.StringBuilder();

            if (bolRenderUlContainer)
            {
                sb.AppendLine("<ul style='border:0px;'>");
            }
            var qry = menuitems.Where(p => p.ParentID == null);
            if (source == "recpage")
            {
                qry = qry.Where(p => p.IsHeader == false);
            }
            foreach (var c in qry)
            {
                if (c.IsDivider == true)
                {
                    if (c.Name == null)
                    {
                        sb.Append("<li><hr></li>");  //divider
                    }
                    else
                    {
                        sb.Append("<div class='hr-text'>" + c.Name + "</div>");
                    }

                }
                else
                {
                    if (c.IsHeader)
                    {
                        sb.Append("<div style='color:silver;font-style: italic;'>" + c.Name + "</div>");
                    }
                    else
                    {
                        string strStyle = null;
                        string strImg = "<span style='margin-left:10px;'></span>";
                        string strCssClass = "dropdown-item";
                        if (c.CssClass != null)
                        {
                            strCssClass = c.CssClass;
                        }
                        if (bolSupportIcons)
                        {
                            strImg = "<span class='k-icon' style='width:30px;'></span>";
                            if (c.Icon != null)
                            {
                                if (c.Icon.Length == 1)
                                {
                                    strImg = "<span style='margin-left:10px;margin-right:5px;font-size:150%;color:royalblue;'>" + c.Icon + "</span>";     //1 unicode character
                                }
                                else
                                {
                                    strImg = string.Format("<span class='k-icon {0}' style='width:30px;color:#2D89EF;'></span>", c.Icon);
                                }
                                
                            }
                        }

                        if (c.IsActive == true)
                        {
                            strStyle = " style='background-color: #ADD8E6;' id='menu_active_item'";
                        }
                        bool bolHasChilds = false;
                        if (c.ID != null && menuitems.Where(p => p.ParentID == c.ID).Count() > 0)
                        {
                            bolHasChilds = true;
                            c.Name += "<span class='k-icon k-i-arrow-60-right' style='float:right;'></span>";
                        }


                        if (c.Url == null)
                        {
                            if (bolHasChilds)
                            {
                                sb.Append(string.Format("<li{0}><a><span class='k-icon' style='width:20px;'></span>{1}</a>", strStyle, c.Name));
                            }
                            else
                            {
                                sb.Append(string.Format("<li{0}><a>{1}</a>", strStyle, c.Name));
                            }
                            
                        }
                        else
                        {
                            if (c.Target != null) c.Target = " target='" + c.Target + "'";
                            sb.Append($"<li{strStyle}><a class='{strCssClass} px-0' href=\"{c.Url}\"{c.Target}>{strImg}{c.Name}</a>");


                        }
                        if (bolHasChilds)
                        {
                            //podřízené nabídky -> druhá úroveň »
                            sb.Append("<ul class='cm_submenu'>");
                            foreach (var cc in menuitems.Where(p => p.ParentID == c.ID))
                            {
                                if (cc.IsDivider)
                                {
                                    sb.Append("<li><hr></li>");  //divider
                                }
                                else
                                {
                                    if (cc.Target != null) cc.Target = " target='" + cc.Target + "'";
                                    sb.Append(string.Format("<li><a class='dropdown-item' href=\"{0}\"{1}>{2}</a></li>", cc.Url, cc.Target, cc.Name));
                                }

                            }
                            sb.Append("</ul>");
                        }

                        sb.Append("</li>");
                    }

                }

            }

            if (bolRenderUlContainer)
            {
                sb.Append("</ul>");
            }
            return sb.ToString();
        }
    }
}
