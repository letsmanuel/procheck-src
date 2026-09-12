namespace ProCheck
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            selectExecutableDialogue = new OpenFileDialog();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loadNewToolStripMenuItem = new ToolStripMenuItem();
            forceAnalysisToolStripMenuItem = new ToolStripMenuItem();
            analyzeNowToolStripMenuItem = new ToolStripMenuItem();
            securityToolStripMenuItem = new ToolStripMenuItem();
            bypassDefenderUNSAFEToolStripMenuItem = new ToolStripMenuItem();
            fileLoadProgressBar = new ProgressBar();
            statusLabel = new Label();
            copyright = new Label();
            networkingToolStripMenuItem = new ToolStripMenuItem();
            resetProxyToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // selectExecutableDialogue
            // 
            selectExecutableDialogue.Filter = "Executables|*.exe";
            selectExecutableDialogue.ShowHiddenFiles = true;
            selectExecutableDialogue.Title = "Select an Executable to Monitor";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(423, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadNewToolStripMenuItem, forceAnalysisToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadNewToolStripMenuItem
            // 
            loadNewToolStripMenuItem.Name = "loadNewToolStripMenuItem";
            loadNewToolStripMenuItem.Size = new Size(180, 22);
            loadNewToolStripMenuItem.Text = "Load new";
            loadNewToolStripMenuItem.Click += loadNewToolStripMenuItem_Click;
            // 
            // forceAnalysisToolStripMenuItem
            // 
            forceAnalysisToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { analyzeNowToolStripMenuItem, securityToolStripMenuItem, networkingToolStripMenuItem });
            forceAnalysisToolStripMenuItem.Name = "forceAnalysisToolStripMenuItem";
            forceAnalysisToolStripMenuItem.Size = new Size(180, 22);
            forceAnalysisToolStripMenuItem.Text = "Advanced";
            forceAnalysisToolStripMenuItem.Click += forceAnalysisToolStripMenuItem_Click;
            // 
            // analyzeNowToolStripMenuItem
            // 
            analyzeNowToolStripMenuItem.Name = "analyzeNowToolStripMenuItem";
            analyzeNowToolStripMenuItem.Size = new Size(180, 22);
            analyzeNowToolStripMenuItem.Text = "Force analyze";
            analyzeNowToolStripMenuItem.Click += analyzeNowToolStripMenuItem_Click;
            // 
            // securityToolStripMenuItem
            // 
            securityToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { bypassDefenderUNSAFEToolStripMenuItem });
            securityToolStripMenuItem.Name = "securityToolStripMenuItem";
            securityToolStripMenuItem.Size = new Size(180, 22);
            securityToolStripMenuItem.Text = "Security";
            // 
            // bypassDefenderUNSAFEToolStripMenuItem
            // 
            bypassDefenderUNSAFEToolStripMenuItem.Name = "bypassDefenderUNSAFEToolStripMenuItem";
            bypassDefenderUNSAFEToolStripMenuItem.Size = new Size(218, 22);
            bypassDefenderUNSAFEToolStripMenuItem.Text = "Bypass Defender (UNSAFE!)";
            bypassDefenderUNSAFEToolStripMenuItem.Click += bypassDefenderUNSAFEToolStripMenuItem_Click;
            // 
            // fileLoadProgressBar
            // 
            fileLoadProgressBar.Location = new Point(229, 215);
            fileLoadProgressBar.MarqueeAnimationSpeed = 50;
            fileLoadProgressBar.Maximum = 10000;
            fileLoadProgressBar.Name = "fileLoadProgressBar";
            fileLoadProgressBar.Size = new Size(182, 23);
            fileLoadProgressBar.Style = ProgressBarStyle.Continuous;
            fileLoadProgressBar.TabIndex = 1;
            fileLoadProgressBar.Value = 9999;
            // 
            // statusLabel
            // 
            statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            statusLabel.Location = new Point(12, 115);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(399, 15);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Select a file to continue.";
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // copyright
            // 
            copyright.AutoSize = true;
            copyright.ForeColor = Color.Gray;
            copyright.Location = new Point(12, 223);
            copyright.Name = "copyright";
            copyright.Size = new Size(116, 15);
            copyright.TabIndex = 3;
            copyright.Text = "© Paul Stiassny 2026";
            // 
            // networkingToolStripMenuItem
            // 
            networkingToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { resetProxyToolStripMenuItem });
            networkingToolStripMenuItem.Name = "networkingToolStripMenuItem";
            networkingToolStripMenuItem.Size = new Size(180, 22);
            networkingToolStripMenuItem.Text = "Networking";
            // 
            // resetProxyToolStripMenuItem
            // 
            resetProxyToolStripMenuItem.Name = "resetProxyToolStripMenuItem";
            resetProxyToolStripMenuItem.Size = new Size(180, 22);
            resetProxyToolStripMenuItem.Text = "Reset Proxy";
            resetProxyToolStripMenuItem.Click += resetProxyToolStripMenuItem_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(423, 247);
            Controls.Add(copyright);
            Controls.Add(statusLabel);
            Controls.Add(fileLoadProgressBar);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Main";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ProCheck";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog selectExecutableDialogue;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadNewToolStripMenuItem;
        private ProgressBar fileLoadProgressBar;
        private Label statusLabel;
        private Label copyright;
        private ToolStripMenuItem forceAnalysisToolStripMenuItem;
        private ToolStripMenuItem analyzeNowToolStripMenuItem;
        private ToolStripMenuItem securityToolStripMenuItem;
        private ToolStripMenuItem bypassDefenderUNSAFEToolStripMenuItem;
        private ToolStripMenuItem networkingToolStripMenuItem;
        private ToolStripMenuItem resetProxyToolStripMenuItem;
    }
}
