namespace RaveAddIn.ErrorHandling
{
    partial class frmException
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
            this.lblException = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnDetails = new System.Windows.Forms.Button();
            this.grbDetails = new System.Windows.Forms.GroupBox();
            this.txtErrorMessage = new System.Windows.Forms.TextBox();
            this.cmdSend = new System.Windows.Forms.Button();
            this.grbDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblException
            // 
            this.lblException.AutoSize = true;
            this.lblException.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblException.Location = new System.Drawing.Point(10, 10);
            this.lblException.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblException.Name = "lblException";
            this.lblException.Size = new System.Drawing.Size(182, 20);
            this.lblException.TabIndex = 0;
            this.lblException.Text = "An exception occured";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(456, 8);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 22);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnDetails
            // 
            this.btnDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDetails.Location = new System.Drawing.Point(456, 35);
            this.btnDetails.Margin = new System.Windows.Forms.Padding(2);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(80, 22);
            this.btnDetails.TabIndex = 0;
            this.btnDetails.Text = "Details >>";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // grbDetails
            // 
            this.grbDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbDetails.Controls.Add(this.cmdSend);
            this.grbDetails.Controls.Add(this.txtErrorMessage);
            this.grbDetails.Location = new System.Drawing.Point(14, 69);
            this.grbDetails.Margin = new System.Windows.Forms.Padding(2);
            this.grbDetails.Name = "grbDetails";
            this.grbDetails.Padding = new System.Windows.Forms.Padding(2);
            this.grbDetails.Size = new System.Drawing.Size(522, 194);
            this.grbDetails.TabIndex = 3;
            this.grbDetails.TabStop = false;
            this.grbDetails.Text = "Details";
            // 
            // txtErrorMessage
            // 
            this.txtErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrorMessage.Location = new System.Drawing.Point(2, 42);
            this.txtErrorMessage.Margin = new System.Windows.Forms.Padding(2);
            this.txtErrorMessage.Multiline = true;
            this.txtErrorMessage.Name = "txtErrorMessage";
            this.txtErrorMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErrorMessage.Size = new System.Drawing.Size(518, 150);
            this.txtErrorMessage.TabIndex = 0;
            this.txtErrorMessage.TabStop = false;
            // 
            // cmdSend
            // 
            this.cmdSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSend.Location = new System.Drawing.Point(332, 14);
            this.cmdSend.Name = "cmdSend";
            this.cmdSend.Size = new System.Drawing.Size(188, 23);
            this.cmdSend.TabIndex = 1;
            this.cmdSend.Text = "Send Error To Development Team";
            this.cmdSend.UseVisualStyleBackColor = true;
            this.cmdSend.Click += new System.EventHandler(this.cmdSend_Click);
            // 
            // frmException
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(546, 272);
            this.Controls.Add(this.grbDetails);
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblException);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(495, 112);
            this.Name = "frmException";
            this.ShowIcon = false;
            this.Text = "Handled Exception";
            this.Load += new System.EventHandler(this.ExceptionForm_Load);
            this.grbDetails.ResumeLayout(false);
            this.grbDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblException;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.GroupBox grbDetails;
        private System.Windows.Forms.TextBox txtErrorMessage;
        private System.Windows.Forms.Button cmdSend;
    }
}