using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using RaveAddIn.ProjectTree;

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

        public void LoadTree()
        {
            ContextMenuStrip cms = BuildProjectCMS();
            ProjectManager.Projects.ForEach(x => LoadProject(x, cms));
        }

        private void LoadProject(RaveProject project, ContextMenuStrip cms)
        {
            // Detect if project is already in tree and simply select the node and return;
            foreach (TreeNode rootNod in treProject.Nodes)
            {
                if (RaveProject.IsSame((RaveProject)rootNod.Tag, project))
                {
                    treProject.SelectedNode = rootNod;
                    rootNod.Expand();
                    return;
                }
            }

            TreeNodeBase node = new ProjectTree.TreeNodeBase(string.Format("{0} ({1})", project.Name, project.ProjectType), "Project", "Projects", 0);
            node.ContextMenuStrip = cms;
            node.Tag = project;
            treProject.Nodes.Add(node);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(project.ProjectFile.FullName);
            AddInputsNode(node, xmlDoc.SelectSingleNode("Project/Inputs"));
            AddRealizations(node, xmlDoc.SelectSingleNode("Project/Realizations"));

            // Select and expand the project node
            treProject.SelectedNode = node;
            node.ExpandAll();
        }

        private TreeNode AddInputsNode(TreeNode parentNode, XmlNode xmlInputs)
        {
            TreeNode nodInputs = new ProjectTree.TreeNodeBase("Inputs", "Input", "Inputs", 1);
            parentNode.Nodes.Add(nodInputs);

            foreach (XmlNode xmlInput in xmlInputs.ChildNodes)
            {
                string relPath = xmlInput.SelectSingleNode("Path").InnerText;
                string name = xmlInput.SelectSingleNode("Name").InnerText;

                TreeNode nodInput = new ProjectTree.TreeNodeBase(name, "Input", "Inputs", 1);
                nodInputs.Nodes.Add(nodInput);
            }

            return nodInputs;
        }

        private TreeNode AddRealizations(TreeNode parentNode, XmlNode xmlRealizations)
        {
            TreeNode nodRealizations = new ProjectTree.TreeNodeBase("Realizations", "realization", "Realizations", 1);
            parentNode.Nodes.Add(nodRealizations);

            AddAnalyses(nodRealizations, xmlRealizations.SelectSingleNode("Analyses"));

            return nodRealizations;
        }

        private TreeNode AddAnalyses(TreeNode parentNode, XmlNode xmlAnalyses)
        {
            if (xmlAnalyses == null)
                return null;

            TreeNode nodAnalyses = new ProjectTree.TreeNodeBase("Analyses", "Analysis", "Analyses", 1);
            parentNode.Nodes.Add(nodAnalyses);

            foreach (XmlNode xmlItem in xmlAnalyses.ChildNodes)
            {
                string name = xmlItem.SelectSingleNode("Name").InnerText;

                TreeNode nodItem = new ProjectTree.TreeNodeBase(name, "Analysis", "Analyses", 1);
                nodAnalyses.Nodes.Add(nodItem);
            }

            return nodAnalyses;
        }

    }
}
