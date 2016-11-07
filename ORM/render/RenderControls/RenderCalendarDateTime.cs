using System;
using System.Collections.Generic;
using System.Text;
using ORM.DBFields;

namespace ORM.render.RenderControls
{
    public class RenderCalendarDateTime: RenderDateTime
    {
        protected string _topFolder;

        public RenderCalendarDateTime(FDatetime fld): base(fld) {}
        public RenderCalendarDateTime(FDatetime fld, string topFolder) 
            : this(fld) 
        { 
            this.topFolder = topFolder; 
        }
        
        public string topFolder
        {
            get { return this._topFolder; }
            set { this._topFolder = value; }
        }

        //DATE
        public override Object renderReadWrite(RenderAttributes renderAttributes)
        {
            renderAttributes["id"] = renderAttributes.get("id", field.name);
            renderAttributes["isSetup"] = "false";
            
            string res = (string)base.renderReadWrite(renderAttributes);

            if (!field.isComputed)
            {
                //res += "<img src='" + this.topFolder + "artwork/js/jscalendar-1.0/img.gif' valign=middle id='" + renderAttributes["id"] + "_trigger_c' style='cursor: pointer; border: 1px solid green;' title='Date selector' onclick=\"if (!this.isSetup) { Calendar.setup({inputField: '" + renderAttributes["id"] + "', ifFormat:'%m/%d/%Y', button: '" + renderAttributes["id"] + "_trigger_c', align: 'Tl', showsTime: false, singleClick: true}); this.isSetup = true; }\" onmouseover=\"this.style.background='green';\" onmouseout=\"this.style.background=''\" />\n";                
                res += "<img src='" + this.topFolder + "artwork/js/jscalendar-1.0/img.gif' id='" + renderAttributes["id"] + "_trigger_c' class='trigger_c' title='Date selector' />\n";
            }
            return res;
        }

        
    }
}
