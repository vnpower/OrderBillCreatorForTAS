using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

namespace MYcustomExample
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string result = MyMessageBox.ShowBox("Do you want to exit?", "Exit");
            if (result.Equals("1"))
            {
                //MessageBox.Show("OK Button was Clicked");
                Application.Exit();
            }

            if (result.Equals("2"))
            {
                //MessageBox.Show("Cancel Button was Clicked");
            }
        }


        
    }
}
