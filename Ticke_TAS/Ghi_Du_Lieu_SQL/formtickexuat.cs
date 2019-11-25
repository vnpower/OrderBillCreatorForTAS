using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

//--- cho connect đến SQL ---
using System.Data.SqlClient;

namespace Ghi_Du_Lieu_SQL
{
    public partial class formtickexuat : Form
    {
        public formtickexuat()
        {
            InitializeComponent();
        }

        private void formtickexuat_Load(object sender, EventArgs e)
        {
            string App_path = Application.StartupPath + "\\tickexuat.rpt";
            ReportDocument cryRpt = new ReportDocument();
            cryRpt.Load(App_path);
            ConnectionInfo connectionInfo = new ConnectionInfo();
            connectionInfo.DatabaseName = "3_ngay44";
            connectionInfo.UserID = "sa";
            //connectionInfo.Password = "vinh@123";
            connectionInfo.Password = "psbinh";
            SetDBLogonForReport(connectionInfo, cryRpt);
            crystalReportViewer1.ReportSource = cryRpt;
            crystalReportViewer1.Refresh();   
        }
        private void SetDBLogonForReport(ConnectionInfo connectionInfo, ReportDocument reportDocument)
        {
            Tables tables = reportDocument.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(tableLogonInfo);
            }
        }
    }
}
