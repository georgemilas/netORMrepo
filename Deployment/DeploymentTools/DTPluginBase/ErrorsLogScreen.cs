using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EM.Logging;
using EM.Collections;
using EM.DB;
using System.Threading;

namespace DeploymentTools.Controls
{
    public partial class ErrorsLogScreen : Form
    {
        public ILogger log { get; set; }
        private RichTextBoxMessageWriterWithTracking writer { get; set; }
        public ErrorsLogScreen()
        {
            InitializeComponent();           
        }

        public ErrorsLogScreen(ILogger log) : this()
        {
            this.log = log;
            writer = new RichTextBoxMessageWriterWithTracking(this.txtErrors, this);
            writer.level = new LogLevel(Level.ERROR);
            ((Logger)this.log).register(writer);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        public bool HasErrors 
        {
            get
            {
               return this.writer.hasMessages;               
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (writer != null)
            {
                ((Logger)this.log).unregister(writer);                
            }
        }
    }
}
