using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using EM.Logging;

namespace DeploymentTools.Controls
{
    public class RichTextBoxMessageWriterWithTracking : RichTextBoxMessageWriter
    {
        public bool hasMessages { get; set; }
        public RichTextBoxMessageWriterWithTracking(RichTextBox txtMessageBox, Control form)
            :base(txtMessageBox, form)
        {
            hasMessages = false;
        }
                
        protected override void writeToTextBox(string txt)
        {
            base.writeToTextBox(txt);
            hasMessages = true;
        }        

    }
}
