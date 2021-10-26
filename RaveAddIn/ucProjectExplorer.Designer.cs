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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucProjectExplorer));
            this.treProject = new System.Windows.Forms.TreeView();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treProject
            // 
            this.treProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treProject.ImageIndex = 0;
            this.treProject.ImageList = this.imgList;
            this.treProject.Location = new System.Drawing.Point(0, 0);
            this.treProject.Name = "treProject";
            this.treProject.SelectedImageIndex = 0;
            this.treProject.Size = new System.Drawing.Size(346, 326);
            this.treProject.TabIndex = 0;
            this.treProject.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treProject_NodeMouseClick);
            this.treProject.DoubleClick += new System.EventHandler(this.treProject_DoubleClick);
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "RaveAddIn_16px.png");
            this.imgList.Images.SetKeyName(1, "BrowseFolder.ico");
            this.imgList.Images.SetKeyName(2, "raster.png");
            this.imgList.Images.SetKeyName(3, "vector.png");
            this.imgList.Images.SetKeyName(4, "vector_missing.png");
            this.imgList.Images.SetKeyName(5, "raster_missing.png");
            this.imgList.Images.SetKeyName(6, "tin.png");
            this.imgList.Images.SetKeyName(7, "no_tin.png");
            this.imgList.Images.SetKeyName(8, "project_view.png");
            // 
            // ucProjectExplorer
            // 
            this.Controls.Add(this.treProject);
            this.Name = "ucProjectExplorer";
            this.Size = new System.Drawing.Size(346, 326);
            this.Load += new System.EventHandler(this.ucProjectExplorer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treProject;
        private System.Windows.Forms.ImageList imgList;
    }
}
