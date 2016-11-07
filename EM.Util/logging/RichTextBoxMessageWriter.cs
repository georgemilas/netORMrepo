using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace EM.Logging
{
    public class RichTextBoxMessageWriter: MessageWriter
    {
        public RichTextBox txtMessageBox;
        public Control form;

        public RichTextBoxMessageWriter(RichTextBox txtMessageBox, Control form)
            :base()
        {
            this.txtMessageBox = txtMessageBox;
            this.form = form;

            this.writer = delegate(string txt)
            {
                //fix "Cross-thread operation not valid" exception AND 
                //even if Cross-thread is not thrown
                //strange COM memory violation errors do occure if ran from a diferent thread 
                //even if they say the rich text box is thread safe?
                if (this.form.InvokeRequired)  
                {
                    //run in the thread where the control was created
                    this.form.Invoke(new ThreadStart( delegate()
                    {
                        writeToTextBox(txt);
                    }));
                }
                else
                {
                    writeToTextBox(txt);
                }
            };

            this.colorSetter = delegate(Color color)
            {
                if (this.form.InvokeRequired)
                {
                    //run in the thread where the control was created
                    this.form.Invoke(new ThreadStart(delegate()
                    {
                        setColorOnTextBox(color);
                    }));
                }
                else
                {
                    setColorOnTextBox(color);
                }                
            };
        }
                
        protected virtual void writeToTextBox(string txt)
        {
            this.txtMessageBox.AppendText(txt);
            this.txtMessageBox.SelectionLength = 0;
            this.txtMessageBox.SelectionStart = this.txtMessageBox.Text.Length;
            this.txtMessageBox.ScrollToCaret();
            this.txtMessageBox.Refresh();
            this.form.PerformLayout();
        }

        protected virtual void setColorOnTextBox(Color color)
        {
            this.txtMessageBox.SelectionColor = color;
            this.form.PerformLayout();
        }

    }
}
