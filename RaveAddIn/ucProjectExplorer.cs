using System;
using System.Windows.Forms;
using RaveAddIn.ProjectTree;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Carto;

namespace RaveAddIn
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class ucProjectExplorer : UserControl
    {
        private readonly ContextMenuStrip cmsProject;
        private readonly ContextMenuStrip cmsFolder;
        private readonly ContextMenuStrip cmsGIS;

        public ucProjectExplorer(object hook)
        {
            InitializeComponent();
            this.Hook = hook;

            cmsProject = new ContextMenuStrip(components);
            cmsProject.Items.Add("Expand All Child Nodes", Properties.Resources.expand, OnExpandChildren);
            cmsProject.Items.Add("-");
            cmsProject.Items.Add("Browse Project Folder", Properties.Resources.BrowseFolder, OnExplore);
            cmsProject.Items.Add("View Project MetaData", Properties.Resources.metadata, OnMetaData);
            cmsProject.Items.Add("Add All Layers To The Map", Properties.Resources.AddToMap, OnAddChildrenToMap);
            cmsProject.Items.Add("-");
            cmsProject.Items.Add("Refresh Project Hierarchy", Properties.Resources.refresh, OnRefreshProject);
            cmsProject.Items.Add("Customize Project Hierarchy", Properties.Resources.tree, OnCustomBusinessLogic);
            cmsProject.Items.Add("-");
            cmsProject.Items.Add("Close Project", null, OnClose);

            cmsFolder = new ContextMenuStrip(components);
            cmsFolder.Items.Add("Add All Layers To The Map", Properties.Resources.AddToMap, OnAddChildrenToMap);
            cmsFolder.Items.Add("Expand All Child Nodes", Properties.Resources.expand, OnExpandChildren);

            cmsGIS = new ContextMenuStrip(components);
            cmsGIS.Items.Add("Add To Map", Properties.Resources.AddToMap, OnAddGISToMap);
            cmsGIS.Items.Add("Browse Folder", Properties.Resources.BrowseFolder, OnExplore);
        }

        #region ERSI generated code 

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private ucProjectExplorer m_windowUI;

            public AddinImpl()
            {
            }

            public ucProjectExplorer UI { get { return m_windowUI; } }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new ucProjectExplorer(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        #endregion

        /// <summary>
        /// User's RAVE AppData Folder
        /// </summary>
        /// <remarks>
        /// C:\Users\USERNAME\AppData\Roaming\RAVE</remarks>
        public static DirectoryInfo AppDataFolder { get { return new DirectoryInfo(Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), Properties.Resources.AppDataFolder)); } }

        /// <summary>
        /// Software deployment folder
        /// </summary>
        public static DirectoryInfo DeployFolder { get { return new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)); } }

        public void LoadProject(FileInfo projectFile)
        {
            // Detect if project is already in tree and simply select the node and return;
            foreach (TreeNode rootNod in treProject.Nodes)
            {
                if (RaveProject.IsSame((RaveProject)rootNod.Tag, projectFile))
                {
                    treProject.SelectedNode = rootNod;
                    rootNod.Expand();
                    return;
                }
            }

            RaveProject newProject = new RaveProject(projectFile);
            TreeNode tnProject = newProject.LoadNewProject(treProject);

            AssignContextMenus(tnProject);
        }

        private void AssignContextMenus(TreeNode node)
        {
            if (node == null)
                return;

            if (node.Tag is GISLayer)
            {
                node.ContextMenuStrip = cmsGIS;
            }
            else if (node.Tag is RaveProject)
            {
                node.ContextMenuStrip = cmsProject;
            }
            else
            {
                node.ContextMenuStrip = cmsFolder;
            }

            // Assign context menus recursively
            foreach (TreeNode n in node.Nodes)
            {
                AssignContextMenus(n);
            }
        }

        /// <summary>
        /// Select the node that the user just right clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void treProject_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            TreeNode theNode = treProject.GetNodeAt(e.X, e.Y);
            if (theNode is TreeNode)
                treProject.SelectedNode = theNode;
        }


        public IGroupLayer BuildArcMapGroupLayers(TreeNode node)
        {
            IGroupLayer parentGrpLyr = null;

            if (node.Parent is TreeNode)
            {
                parentGrpLyr = BuildArcMapGroupLayers(node.Parent);
            }

            return ArcMapUtilities.GetGroupLayer(node.Text, parentGrpLyr);
        }

        public void OnAddChildrenToMap(object sender, EventArgs e)
        {
            AddChildrenToMap(treProject.SelectedNode);
        }

        private void AddChildrenToMap(TreeNode e)
        {
            e.Nodes.OfType<TreeNode>().ToList().ForEach(x => AddChildrenToMap(x));

            if (e.Tag is GISLayer)
            {
                GISLayer layer = (GISLayer)e.Tag;
                IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(e);
                ArcMapUtilities.AddToMap(layer.FilePath, layer.Name, parentGrpLyr);
            }
        }

        public void OnAddGISToMap(object sender, EventArgs e)
        {
            TreeNode selNode = treProject.SelectedNode;
            IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(selNode);
            GISLayer layer = (GISLayer)selNode.Tag;

            FileInfo symbology = GetSymbology(layer);

            ArcMapUtilities.AddToMap(layer.FilePath, layer.Name, parentGrpLyr, symbology);
        }

        /// <summary>
        /// Determine the location of the layer file for this GIS item
        /// </summary>
        /// <remarks>
        /// The following locations will be searched in order for a 
        /// file with the name SYMBOLOGY_KEY.lyr
        /// 
        /// 1. ProjectFolder
        /// 2. %APPDATA%\RAVE\MODEL
        /// 3. %APPDATA%\RAVE\Shared
        /// 4. SOFTWARE_DEPLOYMENT\MODEL
        /// 5. SOFTWARE_DEPLOYMENT\Shared
        /// 
        /// </remarks>
        private FileInfo GetSymbology(GISLayer layer)
        {
            string appDataFolder = Path.Combine(ucProjectExplorer.AppDataFolder.FullName, Properties.Resources.AppDataSymbologyFolder);
            string deployFolder = Path.Combine(ucProjectExplorer.DeployFolder.FullName, Properties.Resources.AppDataSymbologyFolder);

            List<string> SearchFolders = new List<string>()
            {
                layer.Project.Folder.FullName,
                Path.Combine(appDataFolder, layer.Project.ProjectType),
                Path.Combine(appDataFolder, Properties.Resources.AppDataSymbologySharedFolder),
                Path.Combine(deployFolder, layer.Project.ProjectType),
                Path.Combine(deployFolder, Properties.Resources.AppDataSymbologySharedFolder),
            };

            foreach (string folder in SearchFolders)
            {
                if (Directory.Exists(folder))
                {
                    string path = Path.ChangeExtension(Path.Combine(folder, layer.SymbologyKey), "lyr");
                    if (File.Exists(path))
                    {
                        return new FileInfo(path);
                    }
                }
            }

            return null;
        }

        public void OnExplore(object sender, EventArgs e)
        {
            FileInfo file = null;
            object tag = treProject.SelectedNode.Tag;

            if (tag is RaveProject)
            {
                file = ((RaveProject)tag).ProjectFile;
            }
            else if (tag is ProjectTree.GISLayer)
            {
                file = ((ProjectTree.GISLayer)tag).FilePath;
            }

            if (file is System.IO.FileInfo)
                System.Diagnostics.Process.Start(file.Directory.FullName);
        }

        public void OnClose(object sender, EventArgs e)
        {
            treProject.SelectedNode.Remove();
        }

        public void OnMetaData(object sender, EventArgs e)
        {
            RaveProject proj = (RaveProject)treProject.SelectedNode.Tag;

            MetaData.frmMetaData frm = new MetaData.frmMetaData("Riverscapes Project", proj.MetDataNode);

            //frm.MetaDataItems.Insert(0, new MetaData.MetaDataItem("Project Name", proj.Name));
            //frm.MetaDataItems.Insert(1, new MetaData.MetaDataItem("Project Type", proj.ProjectType));
            frm.MetaDataItems.Insert(2, new MetaData.MetaDataItem("Project File", proj.ProjectFile.FullName));
            frm.ShowDialog();
        }

        public void OnCustomBusinessLogic(object sender, EventArgs e)
        {
            // Retrieve the project object.
            TreeNode tnProject = treProject.SelectedNode;
            RaveProject proj = (RaveProject)tnProject.Tag;

            OpenFileDialog frm = new OpenFileDialog();
            frm.Title = "Select Riverscapes Business Logic XML";
            frm.InitialDirectory = proj.ProjectFile.DirectoryName;
            frm.Filter = "Riverscapes Business Logic XML files (*.xml)|*.xml";

            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Remove all the existing child nodes
                tnProject.Nodes.Clear();

                // Re-load the project tree using the selected business logic
                TreeNode nodProject = proj.LoadTree(tnProject, new System.IO.FileInfo(frm.FileName));
                if (nodProject is TreeNode)
                {
                    AssignContextMenus(nodProject);
                }
            }
        }

        public void OnRefreshProject(object sender, EventArgs e)
        {
            // Retrieve the project object.
            TreeNode tnProject = treProject.SelectedNode;
            RaveProject proj = (RaveProject)tnProject.Tag;

            // Load the project into this same node
            proj.LoadProjectIntoNode(tnProject);

            if (tnProject is TreeNode)
            {
                AssignContextMenus(tnProject);
            }
        }

        public void OnExpandChildren(object sender, EventArgs e)
        {
            treProject.SelectedNode.ExpandAll();
        }
    }
}
