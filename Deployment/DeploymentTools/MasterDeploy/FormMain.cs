using System;
using System.Windows.Forms;

namespace MasterDeploy
{
    public partial class FormMain : Form
    {
        
        public FormMain()
        {
            InitializeComponent();
            
        }

        


        private void FormMain_Load(object sender, EventArgs e)
        {
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            
        }

   
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                this.Close();
            }
        }

  

    }

}