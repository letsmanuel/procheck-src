using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProCheck
{
    public partial class MonitorScreen : Form
    {
        private Main mainForm;
        public string monitorFilePath = string.Empty;
        public MonitorScreen(string filePath, Main mainForm)
        {
            InitializeComponent();
            monitorFilePath = filePath;
            this.mainForm = mainForm;
        }

        private void GoBackToMain()
        {
            mainForm.Show();
            this.Close();
        }

        private void MonitorScreen_Load(object sender, EventArgs e)
        {
           this.Text = "ProCheck - Monitoring: " + Path.GetFileName(monitorFilePath);
        }

        private void closeApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void exitMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoBackToMain();
        }
    }
}
