using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MYcustomExample
{
    public partial class CustomMessenger : Form
    {
        static CustomMessenger newMessageBox;
        static string Button_id = "2";

        public CustomMessenger()
        {
            InitializeComponent();
        }

        public static string ShowBox(string txtMessage)
        {
            newMessageBox = new CustomMessenger();
            newMessageBox.lblMessage.Text = txtMessage;
            newMessageBox.ShowDialog();
            return Button_id;
        }

        public static string ShowBox(string txtMessage, string txtTitle)
        {
            newMessageBox = new CustomMessenger();
            newMessageBox.label1.Text = txtMessage;
            newMessageBox.Text = txtTitle;
            newMessageBox.ShowDialog();
            return Button_id;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Button_id = "1";
            newMessageBox.Dispose(); 
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Button_id = "2";
            newMessageBox.Dispose(); 
        } 

    }
}
