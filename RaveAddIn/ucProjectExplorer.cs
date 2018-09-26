using System;
using System.Windows.Forms;
using RaveAddIn.ProjectTree;
using System.IO;
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
        #region ERSI generated code 
        public ucProjectExplorer(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
        }

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

        private ContextMenuStrip cmsProject;
        private ContextMenuStrip cmsFolder;
        private ContextMenuStrip cmsGIS;

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
            if (node.Tag is GISItem)
            {
                BuildGISCMS();
                node.ContextMenuStrip = cmsGIS;
            }
            else if (node.Tag is RaveProject)
            {
                BuildProjectCMS();
                node.ContextMenuStrip = cmsProject;
            }
            else
            {
                BuildFolderCMS();
                node.ContextMenuStrip = cmsFolder;
            }

            // Assign context menus recursively
            foreach (TreeNode n in node.Nodes)
            {
                AssignContextMenus(n);
            }
        }

        private void BuildFolderCMS()
        {
            if (cmsFolder != null)
                return;

            cmsFolder = new ContextMenuStrip(components);
            cmsFolder.Items.Add("Add All Layers To The Map", Properties.Resources.AddToMap, OnAddChildrenToMap);
            cmsFolder.Items.Add("Expand All Child Nodes", Properties.Resources.expand, OnExpandChildren);
        }

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

            if (e.Tag is GISItem)
            {
                GISItem layer = (GISItem) e.Tag;
                IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(e);
                ArcMapUtilities.AddToMap(layer.GISFileInfo, layer.Name, parentGrpLyr);
            }
        }
    }
}
