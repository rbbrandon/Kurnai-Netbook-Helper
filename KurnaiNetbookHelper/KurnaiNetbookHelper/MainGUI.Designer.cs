namespace KurnaiNetbookHelper
{
    partial class MainGUI
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainGUI));
            this.repairListBox = new System.Windows.Forms.ListBox();
            this.collectionListBox = new System.Windows.Forms.ListBox();
            this.fixedButton = new System.Windows.Forms.Button();
            this.brokenButton = new System.Windows.Forms.Button();
            this.collectedButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.repairGroupBox = new System.Windows.Forms.GroupBox();
            this.collectionGroupBox = new System.Windows.Forms.GroupBox();
            this.addJobButton = new System.Windows.Forms.Button();
            this.loginButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.colourGroupBox = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.enableButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.campusComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.repairGroupBox.SuspendLayout();
            this.collectionGroupBox.SuspendLayout();
            this.colourGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // repairListBox
            // 
            this.repairListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.repairListBox.FormattingEnabled = true;
            this.repairListBox.HorizontalScrollbar = true;
            this.repairListBox.Location = new System.Drawing.Point(6, 20);
            this.repairListBox.Name = "repairListBox";
            this.repairListBox.ScrollAlwaysVisible = true;
            this.repairListBox.Size = new System.Drawing.Size(317, 290);
            this.repairListBox.TabIndex = 0;
            // 
            // collectionListBox
            // 
            this.collectionListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.collectionListBox.FormattingEnabled = true;
            this.collectionListBox.HorizontalScrollbar = true;
            this.collectionListBox.Location = new System.Drawing.Point(6, 21);
            this.collectionListBox.Name = "collectionListBox";
            this.collectionListBox.ScrollAlwaysVisible = true;
            this.collectionListBox.Size = new System.Drawing.Size(317, 290);
            this.collectionListBox.TabIndex = 0;
            // 
            // fixedButton
            // 
            this.fixedButton.Enabled = false;
            this.fixedButton.Location = new System.Drawing.Point(347, 155);
            this.fixedButton.Name = "fixedButton";
            this.fixedButton.Size = new System.Drawing.Size(75, 23);
            this.fixedButton.TabIndex = 1;
            this.fixedButton.Text = "------->";
            this.buttonToolTip.SetToolTip(this.fixedButton, "Mark a selected job as \"Awaiting Collection\".");
            this.fixedButton.UseVisualStyleBackColor = true;
            this.fixedButton.Click += new System.EventHandler(this.fixedButton_Click);
            // 
            // brokenButton
            // 
            this.brokenButton.Enabled = false;
            this.brokenButton.Location = new System.Drawing.Point(347, 185);
            this.brokenButton.Name = "brokenButton";
            this.brokenButton.Size = new System.Drawing.Size(75, 23);
            this.brokenButton.TabIndex = 2;
            this.brokenButton.Text = "<-------";
            this.brokenButton.UseVisualStyleBackColor = true;
            this.brokenButton.Click += new System.EventHandler(this.brokenButton_Click);
            // 
            // collectedButton
            // 
            this.collectedButton.Location = new System.Drawing.Point(763, 155);
            this.collectedButton.Name = "collectedButton";
            this.collectedButton.Size = new System.Drawing.Size(75, 23);
            this.collectedButton.TabIndex = 4;
            this.collectedButton.Text = "Collected";
            this.collectedButton.UseVisualStyleBackColor = true;
            this.collectedButton.Click += new System.EventHandler(this.collectedButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(761, 335);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "Close";
            this.buttonToolTip.SetToolTip(this.closeButton, "Close this program.");
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // repairGroupBox
            // 
            this.repairGroupBox.Controls.Add(this.repairListBox);
            this.repairGroupBox.Location = new System.Drawing.Point(12, 12);
            this.repairGroupBox.Name = "repairGroupBox";
            this.repairGroupBox.Size = new System.Drawing.Size(329, 317);
            this.repairGroupBox.TabIndex = 0;
            this.repairGroupBox.TabStop = false;
            this.repairGroupBox.Text = "Netbooks to be Repaired:";
            // 
            // collectionGroupBox
            // 
            this.collectionGroupBox.Controls.Add(this.collectionListBox);
            this.collectionGroupBox.Location = new System.Drawing.Point(428, 12);
            this.collectionGroupBox.Name = "collectionGroupBox";
            this.collectionGroupBox.Size = new System.Drawing.Size(329, 317);
            this.collectionGroupBox.TabIndex = 3;
            this.collectionGroupBox.TabStop = false;
            this.collectionGroupBox.Text = "Netbooks Awaiting Collection:";
            // 
            // addJobButton
            // 
            this.addJobButton.Enabled = false;
            this.addJobButton.Location = new System.Drawing.Point(601, 335);
            this.addJobButton.Name = "addJobButton";
            this.addJobButton.Size = new System.Drawing.Size(75, 23);
            this.addJobButton.TabIndex = 8;
            this.addJobButton.Text = "Add Job";
            this.buttonToolTip.SetToolTip(this.addJobButton, "Add a repair/warranty job.");
            this.addJobButton.UseVisualStyleBackColor = true;
            this.addJobButton.Click += new System.EventHandler(this.addJobButton_Click);
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(682, 335);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(75, 23);
            this.loginButton.TabIndex = 6;
            this.loginButton.Text = "Login";
            this.buttonToolTip.SetToolTip(this.loginButton, "Login as a Tech.");
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(347, 73);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 8;
            this.updateButton.Text = "<- Update ->";
            this.buttonToolTip.SetToolTip(this.updateButton, "Update the repair/collect lists. (Done automatically every 10 seconds)");
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // timer
            // 
            this.timer.Interval = 10000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // colourGroupBox
            // 
            this.colourGroupBox.Controls.Add(this.label7);
            this.colourGroupBox.Controls.Add(this.label6);
            this.colourGroupBox.Controls.Add(this.label5);
            this.colourGroupBox.Controls.Add(this.label4);
            this.colourGroupBox.Controls.Add(this.label3);
            this.colourGroupBox.Controls.Add(this.label2);
            this.colourGroupBox.Controls.Add(this.label1);
            this.colourGroupBox.Location = new System.Drawing.Point(13, 331);
            this.colourGroupBox.Name = "colourGroupBox";
            this.colourGroupBox.Size = new System.Drawing.Size(487, 46);
            this.colourGroupBox.TabIndex = 9;
            this.colourGroupBox.TabStop = false;
            this.colourGroupBox.Text = "Colour Key:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(365, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "= Call Office to check.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(215, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "= DO loan/return.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(45, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "= DO NOT loan/return.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(131, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(202, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "(Right-click an entry to change its status.)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(313, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "BLACK";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(170, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "BLUE";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(6, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "RED";
            // 
            // enableButton
            // 
            this.enableButton.Location = new System.Drawing.Point(763, 185);
            this.enableButton.Name = "enableButton";
            this.enableButton.Size = new System.Drawing.Size(75, 23);
            this.enableButton.TabIndex = 10;
            this.enableButton.Text = "Enable";
            this.buttonToolTip.SetToolTip(this.enableButton, "Enable a student account without marking it as \"Completed\".");
            this.enableButton.UseVisualStyleBackColor = true;
            this.enableButton.Click += new System.EventHandler(this.enableButton_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Enabled = false;
            this.label8.Location = new System.Drawing.Point(671, 366);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(172, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Special Thanks: Ian Doherty, LWT";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // campusComboBox
            // 
            this.campusComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.campusComboBox.FormattingEnabled = true;
            this.campusComboBox.Location = new System.Drawing.Point(347, 28);
            this.campusComboBox.Name = "campusComboBox";
            this.campusComboBox.Size = new System.Drawing.Size(75, 21);
            this.campusComboBox.TabIndex = 45;
            this.campusComboBox.SelectedIndexChanged += new System.EventHandler(this.campusComboBox_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(344, 12);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 13);
            this.label9.TabIndex = 46;
            this.label9.Text = "Campus:";
            // 
            // MainGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 383);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.campusComboBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.enableButton);
            this.Controls.Add(this.colourGroupBox);
            this.Controls.Add(this.addJobButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.collectionGroupBox);
            this.Controls.Add(this.repairGroupBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.collectedButton);
            this.Controls.Add(this.brokenButton);
            this.Controls.Add(this.fixedButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kurnai Netbook Helper - © 2013, Robert Brandon";
            this.Load += new System.EventHandler(this.MainGUI_Load);
            this.repairGroupBox.ResumeLayout(false);
            this.collectionGroupBox.ResumeLayout(false);
            this.colourGroupBox.ResumeLayout(false);
            this.colourGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox repairListBox;
        private System.Windows.Forms.ListBox collectionListBox;
        private System.Windows.Forms.Button fixedButton;
        private System.Windows.Forms.Button brokenButton;
        private System.Windows.Forms.Button collectedButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.GroupBox repairGroupBox;
        private System.Windows.Forms.GroupBox collectionGroupBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button addJobButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.GroupBox colourGroupBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button enableButton;
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.ComboBox campusComboBox;
        private System.Windows.Forms.Label label9;
    }
}

