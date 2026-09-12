namespace ProCheck
{
    partial class network_request
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            timeLabel = new Label();
            panel1 = new Panel();
            statusLabel = new Label();
            panel2 = new Panel();
            methodLabel = new Label();
            urlLabel = new Label();
            copyLink = new Button();
            button1 = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // timeLabel
            // 
            timeLabel.AutoSize = true;
            timeLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            timeLabel.Location = new Point(3, 11);
            timeLabel.Name = "timeLabel";
            timeLabel.Size = new Size(97, 32);
            timeLabel.TabIndex = 0;
            timeLabel.Text = "9999ms";
            timeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.Controls.Add(statusLabel);
            panel1.Location = new Point(112, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(185, 46);
            panel1.TabIndex = 1;
            // 
            // statusLabel
            // 
            statusLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            statusLabel.Location = new Point(12, 7);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(163, 32);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "200 OK";
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            panel2.Controls.Add(methodLabel);
            panel2.Location = new Point(303, 4);
            panel2.Name = "panel2";
            panel2.Size = new Size(185, 46);
            panel2.TabIndex = 3;
            // 
            // methodLabel
            // 
            methodLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            methodLabel.Location = new Point(8, 7);
            methodLabel.Name = "methodLabel";
            methodLabel.Size = new Size(170, 32);
            methodLabel.TabIndex = 2;
            methodLabel.Text = "GET";
            methodLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // urlLabel
            // 
            urlLabel.AutoSize = true;
            urlLabel.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            urlLabel.Location = new Point(547, 11);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new Size(326, 32);
            urlLabel.TabIndex = 4;
            urlLabel.Text = "http://microsoft.com/pkjobs...";
            urlLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // copyLink
            // 
            copyLink.Location = new Point(506, 11);
            copyLink.Name = "copyLink";
            copyLink.Size = new Size(35, 32);
            copyLink.TabIndex = 5;
            copyLink.Text = "C";
            copyLink.UseVisualStyleBackColor = true;
            copyLink.Click += copyLink_Click;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ActiveCaption;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(941, 9);
            button1.Name = "button1";
            button1.Size = new Size(208, 38);
            button1.TabIndex = 6;
            button1.Text = "DETAILS";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // network_request
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(button1);
            Controls.Add(copyLink);
            Controls.Add(urlLabel);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(timeLabel);
            Name = "network_request";
            Size = new Size(1165, 55);
            Load += network_request_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label timeLabel;
        private Panel panel1;
        private Label statusLabel;
        private Panel panel2;
        private Label methodLabel;
        private Label urlLabel;
        private Button copyLink;
        private Button button1;
    }
}
