namespace RaveAddIn
{
    partial class frmAbout
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.lblProductName = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpSupport = new System.Windows.Forms.GroupBox();
            this.lnkIssues = new System.Windows.Forms.LinkLabel();
            this.lnkWebSite = new System.Windows.Forms.LinkLabel();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.grpAcknowledgements = new System.Windows.Forms.GroupBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grpSupport.SuspendLayout();
            this.grpAcknowledgements.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RaveAddIn.Properties.Resources.RaveAddIn;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(131, 122);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(417, 311);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "Close";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductName.Location = new System.Drawing.Point(168, 12);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(66, 24);
            this.lblProductName.TabIndex = 2;
            this.lblProductName.Text = "label1";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(270, 49);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(52, 13);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "lblVersion";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(222, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Version";
            // 
            // grpSupport
            // 
            this.grpSupport.Controls.Add(this.lnkIssues);
            this.grpSupport.Controls.Add(this.lnkWebSite);
            this.grpSupport.Controls.Add(this.Label5);
            this.grpSupport.Controls.Add(this.Label3);
            this.grpSupport.Location = new System.Drawing.Point(168, 68);
            this.grpSupport.Name = "grpSupport";
            this.grpSupport.Size = new System.Drawing.Size(337, 66);
            this.grpSupport.TabIndex = 11;
            this.grpSupport.TabStop = false;
            this.grpSupport.Text = "Support";
            // 
            // lnkIssues
            // 
            this.lnkIssues.AutoSize = true;
            this.lnkIssues.Location = new System.Drawing.Point(102, 42);
            this.lnkIssues.Name = "lnkIssues";
            this.lnkIssues.Size = new System.Drawing.Size(219, 13);
            this.lnkIssues.TabIndex = 6;
            this.lnkIssues.TabStop = true;
            this.lnkIssues.Text = "http://rave.riverscapes.xyz/known-bugs.html";
            this.lnkIssues.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkIssues_LinkClicked);
            // 
            // lnkWebSite
            // 
            this.lnkWebSite.AutoSize = true;
            this.lnkWebSite.Location = new System.Drawing.Point(102, 20);
            this.lnkWebSite.Name = "lnkWebSite";
            this.lnkWebSite.Size = new System.Drawing.Size(134, 13);
            this.lnkWebSite.TabIndex = 4;
            this.lnkWebSite.TabStop = true;
            this.lnkWebSite.Text = "http://rave.riverscapes.xyz";
            this.lnkWebSite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWebSite_LinkClicked);
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(62, 42);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(37, 13);
            this.Label5.TabIndex = 3;
            this.Label5.Text = "Issues";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(50, 20);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(49, 13);
            this.Label3.TabIndex = 1;
            this.Label3.Text = "Web site";
            // 
            // grpAcknowledgements
            // 
            this.grpAcknowledgements.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAcknowledgements.Controls.Add(this.webBrowser1);
            this.grpAcknowledgements.Location = new System.Drawing.Point(12, 140);
            this.grpAcknowledgements.Name = "grpAcknowledgements";
            this.grpAcknowledgements.Size = new System.Drawing.Size(480, 165);
            this.grpAcknowledgements.TabIndex = 12;
            this.grpAcknowledgements.TabStop = false;
            this.grpAcknowledgements.Text = "Acknowledgements";
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(3, 16);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(474, 146);
            this.webBrowser1.TabIndex = 0;
            // 
            // frmAbout
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdOK;
            this.ClientSize = new System.Drawing.Size(504, 346);
            this.Controls.Add(this.grpAcknowledgements);
            this.Controls.Add(this.grpSupport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAbout";
            this.ShowIcon = false;
            this.Text = "frmAbout";
            this.Load += new System.EventHandler(this.frmAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grpSupport.ResumeLayout(false);
            this.grpSupport.PerformLayout();
            this.grpAcknowledgements.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.GroupBox grpSupport;
        internal System.Windows.Forms.LinkLabel lnkIssues;
        internal System.Windows.Forms.LinkLabel lnkWebSite;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label3;
        private System.Windows.Forms.GroupBox grpAcknowledgements;
        private System.Windows.Forms.WebBrowser webBrowser1;
    }
}