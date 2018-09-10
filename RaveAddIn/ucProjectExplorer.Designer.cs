namespace RaveAddIn
{
    partial class ucProjectExplorer
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
            this.treProject = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treProject
            // 
            this.treProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treProject.Location = new System.Drawing.Point(0, 0);
            this.treProject.Name = "treProject";
            this.treProject.Size = new System.Drawing.Size(300, 300);
            this.treProject.TabIndex = 0;
            // 
            // ucProjectExplorer
            // 
            this.Controls.Add(this.treProject);
            this.Name = "ucProjectExplorer";
            this.Size = new System.Drawing.Size(300, 300);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treProject;
    }
}
