namespace ProCheck
{
    partial class MonitorScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            proCheckToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            exitMonitorToolStripMenuItem = new ToolStripMenuItem();
            closeApplicationToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { proCheckToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1860, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // proCheckToolStripMenuItem
            // 
            proCheckToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem, exitMonitorToolStripMenuItem, closeApplicationToolStripMenuItem });
            proCheckToolStripMenuItem.Name = "proCheckToolStripMenuItem";
            proCheckToolStripMenuItem.Size = new Size(70, 20);
            proCheckToolStripMenuItem.Text = "ProCheck";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(167, 22);
            aboutToolStripMenuItem.Text = "About";
            // 
            // exitMonitorToolStripMenuItem
            // 
            exitMonitorToolStripMenuItem.Name = "exitMonitorToolStripMenuItem";
            exitMonitorToolStripMenuItem.Size = new Size(167, 22);
            exitMonitorToolStripMenuItem.Text = "Exit Monitor";
            exitMonitorToolStripMenuItem.Click += exitMonitorToolStripMenuItem_Click;
            // 
            // closeApplicationToolStripMenuItem
            // 
            closeApplicationToolStripMenuItem.Name = "closeApplicationToolStripMenuItem";
            closeApplicationToolStripMenuItem.Size = new Size(167, 22);
            closeApplicationToolStripMenuItem.Text = "Close Application";
            closeApplicationToolStripMenuItem.Click += closeApplicationToolStripMenuItem_Click;
            // 
            // MonitorScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1860, 908);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "MonitorScreen";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ProCheck - LOADING";
            Load += MonitorScreen_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem proCheckToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem exitMonitorToolStripMenuItem;
        private ToolStripMenuItem closeApplicationToolStripMenuItem;
    }
}