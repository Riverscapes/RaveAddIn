using System;
using System.Windows.Forms;
using RaveAddIn.ProjectTree;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Carto;
using System.Xml;

namespace RaveAddIn
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class ucProjectExplorer : UserControl
    {
        public enum NodeInsertModes
        {
            Add,
            Insert
        };

        private const string BaseMapsLabel = "Basemaps";
        private const string BaseMapsTag = "BASEMAPS";

        private readonly ContextMenuStrip cmsProject;
        private readonly ContextMenuStrip cmsFolder;
        private readonly ContextMenuStrip cmsGIS;
        private readonly ContextMenuStrip cmsWMS;
        private readonly ContextMenuStrip cmsFile;
        private readonly ContextMenuStrip cmsView;

        public ucProjectExplorer(object hook)
        {
            InitializeComponent();
            this.Hook = hook;

            cmsProject = new ContextMenuStrip(components);
            cmsProject.Items.Add("Expand All Child Nodes", Properties.Resources.expand, OnExpandChildren);
            cmsProject.Items.Add("-");
            cmsProject.Items.Add("Browse Project Folder", Properties.Resources.BrowseFolder, OnExplore);
            cmsProject.Items.Add("View Project MetaData", Properties.Resources.metadata, OnMetaData);
            cmsProject.Items.Add("View In Warehouse", Properties.Resources.RAVE, OnWarehouse);
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
            ToolStripItem ts = cmsGIS.Items.Add("View Layer MetaData", Properties.Resources.metadata, OnGISMetadata);
            ts.Name = "tsiViewLayerMetadata";

            ToolStripItem tss = cmsGIS.Items.Add("View Source Riverscapes Project", Properties.Resources.RAVE, OnGISSource);
            tss.Name = "tsiViewLayerSourceProject";

            cmsGIS.Items.Add("Browse Folder", Properties.Resources.BrowseFolder, OnExplore);
            //cmsGIS.Opening += onGISMenuOpening;

            cmsWMS = new ContextMenuStrip(components);
            cmsWMS.Items.Add("Add To Map", Properties.Resources.AddToMap, OnAddWMSToMap);

            cmsFile = new ContextMenuStrip(components);
            cmsFile.Items.Add("Open", Properties.Resources.RaveAddIn, OnOpenFile);
            cmsFile.Items.Add("Browse Folder", Properties.Resources.BrowseFolder, OnExplore);

            cmsView = new ContextMenuStrip(components);
            cmsView.Items.Add("Add All Layers To The Map", Properties.Resources.AddToMap, OnAddChildrenToMap);

            this.treProject.AfterSelect += AfterTreeNodeSelect;
        }

        public void AfterTreeNodeSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is GISDataset)
            {
                ToolStripItem[] matches = e.Node.ContextMenuStrip.Items.Find("tsiViewLayerMetadata", false);
                if (matches.Length == 1)
                {
                    matches[0].Enabled = ((GISDataset)e.Node.Tag).Metadata != null;
                }

                ToolStripItem[] matches2 = e.Node.ContextMenuStrip.Items.Find("tsiViewLayerSourceProject", false);
                if (matches2.Length == 1)
                {
                    matches2[0].Enabled = ((GISDataset)e.Node.Tag).HasWarehouseRefernce;
                }
            }
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
        public static DirectoryInfo AppDataFolder { get { return new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Resources.AppDataFolder)); } }

        /// <summary>
        /// Software deployment folder
        /// </summary>
        public static DirectoryInfo DeployFolder { get { return new DirectoryInfo(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "RiverscapesXML")); } }

        public void LoadProject(FileInfo projectFile)
        {
            // Detect if project is already in tree and simply select the node and return;
            foreach (TreeNode rootNod in treProject.Nodes)
            {
                if (rootNod.Tag is RaveProject && RaveProject.IsSame((RaveProject)rootNod.Tag, projectFile))
                {
                    treProject.SelectedNode = rootNod;
                    rootNod.Expand();
                    return;
                }
            }

            RaveProject newProject = new RaveProject(projectFile);
            TreeNode tnProject = newProject.LoadNewProject(treProject, cmsProject);
            tnProject.Text = GetUniqueProjectName(newProject, tnProject.Text);

            // Load default project view
            if (Properties.Settings.Default.LoadDefaultProjectView)
            {
                try
                {
                    // Find the default project view among all the tree nodes
                    List<TreeNode> allNodes = new List<TreeNode>();
                    foreach (TreeNode node in tnProject.Nodes)
                        RaveProject.GetAllNodes(allNodes, node);

                    TreeNode nodDefault = allNodes.FirstOrDefault(x => x.Tag is ProjectTree.ProjectView && ((ProjectTree.ProjectView)x.Tag).IsDefaultView);
                    if (nodDefault is TreeNode)
                    {
                        AddChildrenToMap(nodDefault);
                    }
                }
                catch (Exception ex)
                {
                    // Loading the default project view is optional. Do nothing in production
                    System.Diagnostics.Debug.Assert(false, ex.Message);
                }
            }

            AssignContextMenus(tnProject);
        }


        public void CloseAllProjects()
        {

            // Detect if project is already in tree and simply select the node and return;
            List<TreeNode> projects = new List<TreeNode>();
            foreach (TreeNode rootNod in treProject.Nodes)
            {
                if (rootNod.Tag is RaveProject)
                {
                    projects.Add(rootNod);

                    // Remove the project from the map. SearchRecursive = False
                    // will ensure it only looks at the top level of the map ToC
                    ArcMapUtilities.RemoveGroupLayer(rootNod.Text, false);
                }
            }

            foreach (TreeNode rootNod in projects)
            {
                rootNod.Remove();
            }
        }

        private void AssignContextMenus(TreeNode node)
        {
            if (node == null)
                return;

            if (node.Tag is IGISLayer)
            {
                node.ContextMenuStrip = cmsGIS;
            }
            else if (node.Tag is FileSystemDataset)
            {
                node.ContextMenuStrip = cmsFile;
            }
            else if (node.Tag is RaveProject)
            {
                node.ContextMenuStrip = cmsProject;
            }
            else if (node.Tag is ProjectView)
            {
                node.ContextMenuStrip = cmsView;
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


        public IGroupLayer BuildArcMapGroupLayers(TreeNode node, NodeInsertModes topLevelMode = NodeInsertModes.Insert)
        {
            IGroupLayer parentGrpLyr = null;

            if (node.Parent is TreeNode)
            {
                parentGrpLyr = BuildArcMapGroupLayers(node.Parent, topLevelMode);
            }

            if (node.Tag is IGISLayer)
                return parentGrpLyr;
            else
                return ArcMapUtilities.GetGroupLayer(node.Text, parentGrpLyr, topLevelMode);
        }

        public void OnAddChildrenToMap(object sender, EventArgs e)
        {
            AddChildrenToMap(treProject.SelectedNode);
        }

        private void AddChildrenToMap(TreeNode e)
        {
            e.Nodes.OfType<TreeNode>().ToList().ForEach(x => AddChildrenToMap(x));

            GISDataset ds = null;

            if (e.Tag is GISDataset)
            {
                ds = e.Tag as GISDataset;
            }
            else if (e.Tag is ProjectView)
            {
                ((ProjectView)e.Tag).Layers.ForEach(x => AddChildrenToMap(x.LayerNode));
            }

            if (ds is GISDataset)
            {
                GISDataset layer = (GISDataset)e.Tag;
                IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(e);
                FileInfo symbology = GetSymbology(layer);
                ArcMapUtilities.AddToMap(layer, layer.Name, parentGrpLyr, symbology, transparency: layer.Transparency);
                Cursor.Current = Cursors.Default;
            }
        }

        public void OnAddGISToMap(object sender, EventArgs e)
        {
            TreeNode selNode = treProject.SelectedNode;
            IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(selNode);
            GISDataset layer = (GISDataset)selNode.Tag;

            FileInfo symbology = GetSymbology(layer);

            try
            {
                ArcMapUtilities.AddToMap(layer, layer.Name, parentGrpLyr, symbology, transparency: layer.Transparency);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", ex.Message, layer.Path.FullName), "Error Adding Dataset To Map", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public void OnGISMetadata(object sender, EventArgs e)
        {
            try
            {
                TreeNode selNode = treProject.SelectedNode;
                IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(selNode);
                GISDataset layer = (GISDataset)selNode.Tag;

                if (layer.Metadata != null && layer.Metadata.Count > 0)
                {
                    MetaData.frmMetaData frm = new MetaData.frmMetaData(string.Format("{0} Layer", layer.Name), layer.Metadata);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Viewing Layer Metadata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void OnGISSource(object sender, EventArgs e)
        {
            try
            {
                TreeNode selNode = treProject.SelectedNode;
                IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(selNode);
                GISDataset layer = (GISDataset)selNode.Tag;

                if (layer.HasWarehouseRefernce)
                    System.Diagnostics.Process.Start(layer.WarehouseReference.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Determining Source Project", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void OnAddWMSToMap(object sender, EventArgs e)
        {
            TreeNode selNode = treProject.SelectedNode;
            IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(selNode, NodeInsertModes.Add);
            ProjectTree.WMSLayer layer = (ProjectTree.WMSLayer)selNode.Tag;

            try
            {
                ArcMapUtilities.AddWMSTopMap(layer.Name, layer.URL, parentGrpLyr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error adding the Web Mapping Service to the map: {0}", ex.Message), Properties.Resources.ApplicationNameLong, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void OnOpenFile(object sender, EventArgs e)
        {
            FileSystemDataset ds = ((FileSystemDataset)treProject.SelectedNode.Tag);
            try
            {
                System.Diagnostics.Process.Start(ds.Path.FullName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error opening project item: {0}\n{1}", ds.Path.FullName, ex.Message), Properties.Resources.ApplicationNameLong, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Determine the location of the layer file for this GIS item
        /// </summary>
        /// <remarks>
        /// The following locations will be searched in order for a 
        /// file with the name SYMBOLOGY_KEY.lyr
        /// 
        /// 1. ProjectFolder
        /// 2. %APPDATA%\RAVE\Symbology\esri\MODEL
        /// 3. %APPDATA%\RAVE\Symbology\esrsi\Shared
        /// 4. SOFTWARE_DEPLOYMENT\Symbology\esri\MODEL
        /// 5. SOFTWARE_DEPLOYMENT\Symbology\esri\Shared
        /// 
        /// </remarks>
        private FileInfo GetSymbology(GISDataset layer)
        {
            if (string.IsNullOrEmpty(layer.SymbologyKey))
                return null;

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
            FileSystemInfo file = null;
            object tag = treProject.SelectedNode.Tag;

            if (tag is RaveProject)
            {
                file = ((RaveProject)tag).ProjectFile;
            }
            else if (tag is FileSystemDataset)
            {
                file = ((FileSystemDataset)tag).Path;
            }

            if (file is System.IO.FileInfo)
                System.Diagnostics.Process.Start(((FileInfo)file).Directory.FullName);
            else
                System.Diagnostics.Process.Start(((DirectoryInfo)file).Parent.FullName);

        }

        public void RefreshBaseMaps()
        {
            // Remove the existing base maps
            if (treProject.Nodes.Count > 0 && string.Compare(treProject.Nodes[treProject.Nodes.Count - 1].Tag.ToString(), BaseMapsLabel, true) != 0)
            {
                treProject.Nodes.Remove(treProject.Nodes[treProject.Nodes.Count - 1]);
            }

            // Exit if no base maps are required
            if (!Properties.Settings.Default.LoadBaseMaps || string.IsNullOrEmpty(Properties.Settings.Default.BaseMap))
            {
                return;
            }

            List<string> searchFolders = new List<string>() {
                ucProjectExplorer.AppDataFolder.FullName,
                ucProjectExplorer.DeployFolder.FullName,
            };

            foreach (string folder in searchFolders)
            {
                string baseMapPath = Path.Combine(folder, "BaseMaps.xml");
                if (File.Exists(baseMapPath))
                {
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(baseMapPath);
                        XmlNode nodRegion = xmlDoc.SelectSingleNode(string.Format("BaseMaps/Region[@name='{0}']", Properties.Settings.Default.BaseMap));
                        if (nodRegion is XmlNode)
                        {
                            if (treProject.Nodes.Count < 1 || string.Compare(treProject.Nodes[treProject.Nodes.Count - 1].Tag.ToString(), BaseMapsLabel, true) != 0)
                            {
                                TreeNode baseMapsNode = new TreeNode(BaseMapsLabel, 1, 1);
                                baseMapsNode.Tag = BaseMapsTag;
                                treProject.Nodes.Insert(treProject.Nodes.Count, baseMapsNode);
                            }

                            LoadBaseMapsFromXML(treProject.Nodes[treProject.Nodes.Count - 1], nodRegion);
                            return;
                        }
                    }
                    catch
                    {
                        // Do nothing. Proceed to next base map file
                    }
                }
            }
        }

        private void LoadBaseMapsFromXML(TreeNode nodParent, XmlNode nodXML)
        {
            foreach (XmlNode node in nodXML.ChildNodes)
            {
                try
                {
                    if (string.Compare(node.Name, "GroupLayer", true) == 0)
                    {
                        TreeNode groupNode = new TreeNode(node.Attributes["name"].InnerText, 1, 1);
                        LoadBaseMapsFromXML(groupNode, node);

                        if (groupNode.Nodes.Count > 0)
                            nodParent.Nodes.Add(groupNode);
                    }
                    else if (string.Compare(node.Name, "Layer", true) == 0)
                    {
                        // Skip all but WMS layers
                        XmlAttribute attType = node.Attributes["type"];
                        if (attType is XmlAttribute && !string.IsNullOrEmpty(attType.InnerText))
                        {
                            if (string.Compare("wms", attType.InnerText, true) != 0)
                                continue;
                        }

                        TreeNode newNode = nodParent.Nodes.Add(node.Attributes["name"].InnerText);
                        newNode.Tag = new ProjectTree.WMSLayer(newNode.Text, node.Attributes["url"].InnerText, 0, string.Empty);
                        newNode.ContextMenuStrip = cmsWMS;
                    }
                }
                catch (Exception ex)
                {
                    // Do nothing. Proceed to next XML node
                }
            }
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

        public void OnWarehouse(object sender, EventArgs e)
        {
            RaveProject proj = (RaveProject)treProject.SelectedNode.Tag;
            Uri warehouseURL = proj.WarehouseReference;
            if (warehouseURL is Uri)
            {
                System.Diagnostics.Process.Start(warehouseURL.ToString());
            }
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

        private void ucProjectExplorer_Load(object sender, EventArgs e)
        {
            RefreshBaseMaps();
        }


        /// <summary>
        /// Get a unique name for a project suitable for use in project tree
        /// </summary>
        /// <param name="originalName">The name of the project from the XML</param>
        /// <returns>If a project with the same name exists in the project tree
        /// already then this method will return the original name plus a unique suffix</returns>
        private string GetUniqueProjectName(RaveProject proj, string originalName)
        {
            int occurences = 0;
            foreach (TreeNode nod in treProject.Nodes)
            {
                if (nod.Tag is RaveProject && nod.Tag != proj)
                {
                    if (nod.Text.StartsWith(originalName))
                        occurences++;
                }
            }

            if (occurences > 0)
                return string.Format("{0} Copy {1}", originalName, occurences);
            else
                return originalName;
        }
    }
}
