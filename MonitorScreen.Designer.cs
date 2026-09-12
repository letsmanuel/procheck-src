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
            idleToolStripMenuItem = new ToolStripMenuItem();
            panel1 = new Panel();
            label1 = new Label();
            networkPanel = new FlowLayoutPanel();
            panel2 = new Panel();
            launchPanel = new Panel();
            pauseButton = new Button();
            killButton = new Button();
            launchButton = new Button();
            menuStrip1.SuspendLayout();
            panel1.SuspendLayout();
            launchPanel.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { proCheckToolStripMenuItem, idleToolStripMenuItem });
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
            // idleToolStripMenuItem
            // 
            idleToolStripMenuItem.Name = "idleToolStripMenuItem";
            idleToolStripMenuItem.Size = new Size(47, 20);
            idleToolStripMenuItem.Text = "idle...";
            idleToolStripMenuItem.Click += idleToolStripMenuItem_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(211, 206, 230);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(networkPanel);
            panel1.Location = new Point(0, 615);
            panel1.Name = "panel1";
            panel1.Size = new Size(1412, 293);
            panel1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 8);
            label1.Name = "label1";
            label1.Size = new Size(161, 20);
            label1.TabIndex = 1;
            label1.Text = "NETWORK REQUESTS";
            // 
            // networkPanel
            // 
            networkPanel.AutoScroll = true;
            networkPanel.FlowDirection = FlowDirection.TopDown;
            networkPanel.Location = new Point(12, 31);
            networkPanel.Name = "networkPanel";
            networkPanel.Size = new Size(1393, 262);
            networkPanel.TabIndex = 0;
            networkPanel.WrapContents = false;
            networkPanel.Paint += networkPanel_Paint;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(245, 248, 252);
            panel2.Location = new Point(1411, 27);
            panel2.Name = "panel2";
            panel2.Size = new Size(449, 881);
            panel2.TabIndex = 2;
            // 
            // launchPanel
            // 
            launchPanel.Controls.Add(pauseButton);
            launchPanel.Controls.Add(killButton);
            launchPanel.Controls.Add(launchButton);
            launchPanel.Location = new Point(453, 195);
            launchPanel.Name = "launchPanel";
            launchPanel.Size = new Size(459, 233);
            launchPanel.TabIndex = 3;
            // 
            // pauseButton
            // 
            pauseButton.Location = new Point(190, 67);
            pauseButton.Name = "pauseButton";
            pauseButton.Size = new Size(100, 100);
            pauseButton.TabIndex = 2;
            pauseButton.Text = "{pause}";
            pauseButton.UseVisualStyleBackColor = true;
            pauseButton.Click += pauseButton_Click;
            // 
            // killButton
            // 
            killButton.Location = new Point(324, 67);
            killButton.Name = "killButton";
            killButton.Size = new Size(100, 100);
            killButton.TabIndex = 1;
            killButton.Text = "{close}";
            killButton.UseVisualStyleBackColor = true;
            killButton.Click += killButton_Click;
            // 
            // launchButton
            // 
            launchButton.Location = new Point(53, 67);
            launchButton.Name = "launchButton";
            launchButton.Size = new Size(100, 100);
            launchButton.TabIndex = 0;
            launchButton.Text = "{launch}";
            launchButton.UseVisualStyleBackColor = true;
            launchButton.Click += launchButton_Click;
            // 
            // MonitorScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1860, 908);
            Controls.Add(launchPanel);
            Controls.Add(panel2);
            Controls.Add(panel1);
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
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            launchPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem proCheckToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem exitMonitorToolStripMenuItem;
        private ToolStripMenuItem closeApplicationToolStripMenuItem;
        private Panel panel1;
        private Panel panel2;
        private Panel launchPanel;
        private Button killButton;
        private Button launchButton;
        private Button pauseButton;
        private Label label1;
        private FlowLayoutPanel networkPanel;
        private ToolStripMenuItem idleToolStripMenuItem;
    }
}