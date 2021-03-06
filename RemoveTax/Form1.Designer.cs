﻿namespace WindowsFormsApplication3
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lSelected = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btBrowse = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.cbYear = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btRemove = new System.Windows.Forms.Button();
            this.TestingLabel = new System.Windows.Forms.Label();
            this.checkTesting = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lSelected
            // 
            this.lSelected.AutoSize = true;
            this.lSelected.Location = new System.Drawing.Point(93, 35);
            this.lSelected.Name = "lSelected";
            this.lSelected.Size = new System.Drawing.Size(29, 13);
            this.lSelected.TabIndex = 11;
            this.lSelected.Text = "label";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Active Folder :";
            // 
            // btBrowse
            // 
            this.btBrowse.Location = new System.Drawing.Point(276, 11);
            this.btBrowse.Name = "btBrowse";
            this.btBrowse.Size = new System.Drawing.Size(81, 21);
            this.btBrowse.TabIndex = 9;
            this.btBrowse.Text = "Browse...";
            this.btBrowse.UseVisualStyleBackColor = true;
            this.btBrowse.Click += new System.EventHandler(this.btBrowse_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(12, 12);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(242, 20);
            this.txtPath.TabIndex = 8;
            this.txtPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // cbYear
            // 
            this.cbYear.FormattingEnabled = true;
            this.cbYear.Location = new System.Drawing.Point(15, 76);
            this.cbYear.Name = "cbYear";
            this.cbYear.Size = new System.Drawing.Size(121, 21);
            this.cbYear.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Year to Remove";
            // 
            // btRemove
            // 
            this.btRemove.Location = new System.Drawing.Point(276, 60);
            this.btRemove.Name = "btRemove";
            this.btRemove.Size = new System.Drawing.Size(81, 23);
            this.btRemove.TabIndex = 14;
            this.btRemove.Text = "Remove Year";
            this.btRemove.UseVisualStyleBackColor = true;
            this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
            // 
            // TestingLabel
            // 
            this.TestingLabel.AutoSize = true;
            this.TestingLabel.Location = new System.Drawing.Point(12, 109);
            this.TestingLabel.Name = "TestingLabel";
            this.TestingLabel.Size = new System.Drawing.Size(377, 39);
            this.TestingLabel.TabIndex = 15;
            this.TestingLabel.Text = "Test ctx.ini written to \"ctx.iniT\".  \r\nApplication files are copied, and not dele" +
    "ted.\r\nClient folders will need to be moved back to original location in WFX32\\\\C" +
    "lient.\r\n";
            // 
            // checkTesting
            // 
            this.checkTesting.AutoSize = true;
            this.checkTesting.Checked = true;
            this.checkTesting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkTesting.Location = new System.Drawing.Point(276, 89);
            this.checkTesting.Name = "checkTesting";
            this.checkTesting.Size = new System.Drawing.Size(61, 17);
            this.checkTesting.TabIndex = 16;
            this.checkTesting.Text = "Testing";
            this.checkTesting.UseVisualStyleBackColor = true;
            this.checkTesting.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 167);
            this.Controls.Add(this.checkTesting);
            this.Controls.Add(this.TestingLabel);
            this.Controls.Add(this.btRemove);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbYear);
            this.Controls.Add(this.lSelected);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btBrowse);
            this.Controls.Add(this.txtPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Remove Tax Year";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lSelected;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btBrowse;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ComboBox cbYear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btRemove;
        private System.Windows.Forms.Label TestingLabel;
        private System.Windows.Forms.CheckBox checkTesting;
    }
}

