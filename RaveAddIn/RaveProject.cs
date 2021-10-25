using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Linq;

namespace RaveAddIn
{
    public class RaveProject
    {
        public readonly FileInfo ProjectFile;
        public DirectoryInfo Folder { get { return ProjectFile.Directory; } }
        public readonly string ProjectType;

        public RaveProject(FileInfo projectFile)
        {
            ProjectFile = projectFile;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(projectFile.FullName);

                string xPath = "Project/ProjectType";
                XmlNode nodProjectType = xmlDoc.SelectSingleNode(xPath);
                if (nodProjectType == null)
                    throw new Exception("Missing XML node at " + xPath);

                if (string.IsNullOrEmpty(nodProjectType.InnerText))
                    throw new Exception(string.Format("The project type at XPath '{0}' contains no value. This XPath cannot be empty.", xPath));

                ProjectType = nodProjectType.InnerText;
            }
            catch (Exception ex)
            {
                ex.Data["Project File"] = projectFile.FullName;
                throw;
            }
        }

        public static bool IsSame(RaveProject proj1, FileInfo projectFile)
        {
            return string.Compare(proj1.ProjectFile.FullName, projectFile.FullName) == 0;
        }

        //private FileInfo AbsolutePath(string relativePath)
        //{
        //    return new FileInfo(Path.Combine(ProjectFile.DirectoryName, relativePath));
        //}

        /// <summary>
        /// Determine the location of the business lofic XML file for this project
        /// </summary>
        /// <remarks>
        /// The following locations will be searched in order for a 
        /// file with an XPath of /Project/ProjectType that matches (case insenstive)
        /// the ProjectType of this project object.
        /// 
        /// 1. ProjectFolder
        /// 2. %APPDATA%\RAVE\XML
        /// 3. SOFTWARE_DEPLOYMENT\XML
        /// 
        /// </remarks>
        private FileInfo LoadBusinessLogicXML()
        {
            List<string> SearchFolders = new List<string>()
            {
                ProjectFile.DirectoryName,
                Path.Combine(ucProjectExplorer.AppDataFolder.FullName, Properties.Resources.BusinessLogicXMLFolder),
                Path.Combine(ucProjectExplorer.DeployFolder.FullName, Properties.Resources.BusinessLogicXMLFolder),
            };

            foreach (string folder in SearchFolders)
            {
                string xmlPath = Path.ChangeExtension(Path.Combine(folder, ProjectType), "xml");
                if (File.Exists(xmlPath))
                {
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(xmlPath);
                        System.Diagnostics.Debug.Print(string.Format("Using business logic at {0}", xmlPath));
                        return new FileInfo(xmlPath);
                    }
                    catch (Exception ex)
                    {
                        Exception ex2 = new FileLoadException(string.Format("Error Loading business logic from the following path." +
                            " Remove or rename this file to allow RAVE to continue searching for alternative {0} business logic files.\n\n{1}",
                            ProjectType, xmlPath), ex);
                        ex2.Data["FilePath"] = xmlPath;
                        throw ex2;
                    }
                }
            }

            return null;
        }

        public XmlNode MetDataNode
        {
            get
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(ProjectFile.FullName);

                return xmlDoc.SelectSingleNode("Project/MetaData");
            }
        }

        public Uri WarehouseReference
        {
            get
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(ProjectFile.FullName);

                    XmlNode nodProgram = xmlDoc.SelectSingleNode("Project/Warehouse/Meta[@name='program']");
                    XmlNode nodID = xmlDoc.SelectSingleNode("Project/Warehouse/Meta[@name='id']");

                    if (nodProgram is XmlNode && nodID is XmlNode && !string.IsNullOrEmpty(nodProgram.InnerText) && !string.IsNullOrEmpty(nodID.InnerText))
                    {
                        Uri baseUri = new Uri(Properties.Resources.DataWarehouseURL);
                        Uri projectUri = new Uri(baseUri, string.Format("#/{0}/{1}", nodProgram.InnerText, nodID.InnerText));
                        return projectUri;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error attempting to build warehouse URL for project.");
                }

                return null;
            }
        }

        /// <summary>
        /// Load a project into the tree that doesn't already exist
        /// </summary>
        /// <param name="treProject"></param>
        /// <returns></returns>
        public TreeNode LoadNewProject(TreeView treProject, ContextMenuStrip cmsProjectView)
        {
            TreeNode tnProject = new TreeNode("TITLE_NOT_FOUND", 1, 1);
            tnProject.Tag = this;
            treProject.Nodes.Insert(0, tnProject);

            // Assign the project CMS here so that it is available if anything else crashes or goes wrong.
            // Allows the user to unload the partially loaded project.
            treProject.ContextMenuStrip = cmsProjectView;

            TreeNode tnResult = LoadProjectIntoNode(tnProject);

            // If nothing returned then something went wrong. Remove the temporary node.
            if (tnResult == null)
            {
                tnProject.Remove();
                MessageBox.Show(string.Format("Failed to load project because there is no valid business logic XML file for projects of type '{0}'.", ProjectType), "Missing Business Logic XML File", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return tnResult;
        }

        public TreeNode LoadProjectIntoNode(TreeNode tnProject)
        {
            // Remove all the existing child nodes (required if refreshing existing tree node)
            tnProject.Nodes.Clear();

            FileInfo businessLogic = LoadBusinessLogicXML();
            if (businessLogic == null)
            {
                return null;
            }

            return LoadTree(tnProject, businessLogic);
        }

        public TreeNode LoadTree(TreeNode tnProject, FileInfo businessLogicXML)
        {
            // Determine the type of project
            XmlDocument xmlProject = new XmlDocument();
            xmlProject.Load(ProjectFile.FullName);
            XmlNode projectXMLRoot = xmlProject.SelectSingleNode("Project");

            // Load the business logic XML file and retrieve the root node
            XmlDocument xmlBusiness = new XmlDocument();
            xmlBusiness.Load(businessLogicXML.FullName);
            XmlNode nodBLRoot = xmlBusiness.SelectSingleNode("Project/Node");
            if (!(nodBLRoot is XmlNode))
            {
                MessageBox.Show("Business logic XML file does not contain 'Project/Node' XPath.", "Business Logic Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            // Retrieve and apply the project name to the parent node
            tnProject.Text = GetLabel(nodBLRoot, projectXMLRoot);

            // The parent node might specify the starting project node (e.g. Riverscapes Context and VBET)
            XmlAttribute attXPath = nodBLRoot.Attributes["xpath"];
            if (attXPath is XmlAttribute && !string.IsNullOrEmpty(attXPath.InnerText))
            {
                XmlNode xmlProjectChild = projectXMLRoot.SelectSingleNode(attXPath.InnerText);
                if (xmlProjectChild is XmlNode)
                    projectXMLRoot = xmlProjectChild;
            }

            // Loop over all child nodes of the business logic XML and load them to the tree
            nodBLRoot.ChildNodes.OfType<XmlNode>().ToList().ForEach(x => LoadTreeNode(tnProject, projectXMLRoot, x));

            LoadProjectViews(tnProject, xmlBusiness);

            // Expand the project tree node now that all the items have been added
            tnProject.ExpandAll();

            // Loop over all tree nodes and collapse any group layers.
            // This has to be done last once all the nodes have their children
            List<TreeNode> allNodes = new List<TreeNode>();
            foreach (TreeNode node in tnProject.Nodes)
                GetAllNodes(allNodes, node);
            allNodes.Where(x => x.Tag is ProjectTree.GroupLayer && ((ProjectTree.GroupLayer)x.Tag).Collapse).ToList().ForEach(x => x.Collapse());

            return tnProject;
        }

        private TreeNode LoadProjectViews(TreeNode tnProject, XmlNode xmlBusiness)
        {
            XmlNode nodViews = xmlBusiness.SelectSingleNode("Project/Views");
            if (nodViews == null)
                return null;

            XmlAttribute attDefault = nodViews.Attributes["default"];
            TreeNode defaultView = null;
            string defaultViewName = string.Empty;
            if (attDefault is XmlAttribute)
            {
                defaultViewName = attDefault.InnerText;
            }

            TreeNode tnViews = null;

            foreach (XmlNode nodView in nodViews.SelectNodes("View"))
            {
                XmlAttribute attName = nodView.Attributes["name"];
                if (attName == null || string.IsNullOrEmpty(attName.InnerText))
                    continue;

                string viewId = string.Empty;
                XmlAttribute viewAttId = nodView.Attributes["id"];
                if (viewAttId is XmlAttribute && !string.IsNullOrEmpty(viewAttId.InnerText))
                    viewId = viewAttId.InnerText;

                bool IsDefaultView = !string.IsNullOrEmpty(defaultViewName) && string.Compare(defaultViewName, viewId) == 0;

                string viewName = nodView.Attributes["name"].InnerText;
                ProjectTree.ProjectView view = new ProjectTree.ProjectView(viewId, viewName, IsDefaultView);


                foreach (XmlNode nodLayer in nodView.SelectNodes("Layers/Layer"))
                {
                    XmlAttribute attId = nodLayer.Attributes["id"];
                    if (attId == null || string.IsNullOrEmpty(attId.InnerText))
                        continue;

                    bool isVisible = true;
                    XmlAttribute attVisible = nodLayer.Attributes["visible"];
                    if (attVisible is XmlAttribute && !string.IsNullOrEmpty(attVisible.InnerText))
                    {
                        bool.TryParse(attVisible.InnerText, out isVisible);
                    }

                    TreeNode tnLayer = FindTreeNodeById(tnProject, attId.InnerText);
                    if (tnLayer is TreeNode)
                    {
                        view.Layers.Add(new ProjectTree.ProjectViewLayer(tnLayer, isVisible));
                    }
                }

                if (view.Layers.Count > 0)
                {
                    // Create the project tree branch that will contain the views
                    if (tnViews == null)
                    {
                        tnViews = new TreeNode("Project Views", 1, 1);
                        tnViews.Tag = new ProjectTree.GroupLayer("Project Views", true, string.Empty);
                        tnProject.Nodes.Add(tnViews);
                    }

                    TreeNode tnView = new TreeNode(viewName, 8, 8);
                    tnView.Tag = view;
                    tnViews.Nodes.Add(tnView);

                    // Check if this is the default view
                    if (view.IsDefaultView)
                        defaultView = tnView;
                }
            }

            return defaultView;
        }

        private TreeNode FindTreeNodeById(TreeNode parent, string id)
        {
            string nodeId = string.Empty;
            if (parent.Tag is ProjectTree.BaseDataset)
            {
                ProjectTree.BaseDataset ds = parent.Tag as ProjectTree.BaseDataset;
                nodeId = ds.Id;
            }
            else if (parent.Tag is ProjectTree.GroupLayer)
            {
                ProjectTree.GroupLayer ds = parent.Tag as ProjectTree.GroupLayer;
                nodeId = ds.Id;
            }

            if (!string.IsNullOrEmpty(nodeId))
            {
                if (string.Compare(nodeId, id, true) == 0)
                    return parent;
            }

            TreeNode result = null;
            foreach (TreeNode child in parent.Nodes)
            {
                result = FindTreeNodeById(child, id);
                if (result is TreeNode)
                    return result;
            }

            return null;
        }

        private void LoadTreeNode(TreeNode tnParent, XmlNode xmlProject, XmlNode xmlBusiness)
        {
            if (xmlBusiness.NodeType == XmlNodeType.Comment)
                return;

            if (string.Compare(xmlBusiness.Name, "Repeater", true) == 0)
            {
                // Add the repeater label
                XmlAttribute attLabel = xmlBusiness.Attributes["label"];
                if (attLabel is XmlAttribute && !string.IsNullOrEmpty(attLabel.InnerText))
                {
                    TreeNode newNode = new TreeNode(attLabel.InnerText, 1, 1);
                    tnParent.Nodes.Add(newNode);

                    // Repeat the business logic items inside the repeater for all items in the xPath
                    XmlAttribute attXPath = xmlBusiness.Attributes["xpath"];
                    if (attXPath is XmlAttribute && !string.IsNullOrEmpty(attXPath.InnerText))
                    {
                        foreach (XmlNode xmlProjectChild in xmlProject.SelectNodes(attXPath.InnerText))
                        {
                            foreach (XmlNode xmlBusinessChild in xmlBusiness.ChildNodes)
                            {
                                LoadTreeNode(newNode, xmlProjectChild, xmlBusinessChild);
                            }
                        }
                    }
                }
            }
            else if (string.Compare(xmlBusiness.Name, "Children", true) == 0)
            {
                foreach (XmlNode xmlBusinessNode in xmlBusiness.ChildNodes)
                {
                    LoadTreeNode(tnParent, xmlProject, xmlBusinessNode);
                }
            }
            else if (string.Compare(xmlBusiness.Name, "Node", true) == 0)
            {
                // First the new project node referred to by the XPath
                XmlAttribute attXPath = xmlBusiness.Attributes["xpath"];
                if (attXPath is XmlAttribute && !string.IsNullOrEmpty(attXPath.InnerText))
                {
                    xmlProject = xmlProject.SelectSingleNode(attXPath.InnerText);
                    if (xmlProject == null)
                        return;
                }

                // Now get the label. First Try the XPath, then the label
                string label = "No Label Provided";
                XmlAttribute attXPathLabel = xmlBusiness.Attributes["xpathlabel"];
                if (attXPathLabel is XmlAttribute && !string.IsNullOrEmpty(attXPathLabel.InnerText))
                {
                    XmlNode xmlLabel = xmlProject.SelectSingleNode(attXPathLabel.InnerText);
                    if (xmlLabel is XmlNode && !string.IsNullOrEmpty(xmlLabel.InnerText))
                    {
                        label = xmlLabel.InnerText;
                    }
                }
                else
                {
                    XmlAttribute attLabel = xmlBusiness.Attributes["label"];
                    if (attLabel is XmlAttribute && !string.IsNullOrEmpty(attLabel.InnerText))
                    {
                        label = attLabel.InnerText;
                    }
                }

                // Get the ID used for associated nodes with project views
                string id = string.Empty;
                XmlAttribute attId = xmlBusiness.Attributes["id"];
                if (attId is XmlAttribute && !string.IsNullOrEmpty(attId.InnerText))
                    id = attId.InnerText;

                XmlAttribute attType = xmlBusiness.Attributes["type"];
                if (attType is XmlAttribute)
                {
                    //This is a GIS Node!

                    // Retrieve symbology key from business logic
                    string symbology = string.Empty;
                    XmlAttribute attSym = xmlBusiness.Attributes["symbology"];
                    if (attSym is XmlAttribute && !String.IsNullOrEmpty(attSym.InnerText))
                        symbology = attSym.InnerText;

                    // Retrieve the transparency from the business logic
                    short transparency = 0;
                    XmlAttribute attTransparency = xmlBusiness.Attributes["transparency"];
                    if (attTransparency is XmlAttribute && !string.IsNullOrEmpty(attTransparency.InnerText))
                    {
                        if (!short.TryParse(attTransparency.InnerText, out transparency))
                            System.Diagnostics.Debug.Print(string.Format("Invalid layer transparency for {0}: {1}", label, transparency));
                    }

                    AddGISNode(tnParent, attType.InnerText, xmlProject, symbology, label, transparency, id);
                }
                else
                {
                    // Static label node

                    // First check if there are children to this node and if so where collapsed is specified
                    bool collapsed = true;
                    XmlNode xmlChildren = xmlBusiness.SelectSingleNode("Children");
                    if (xmlChildren is XmlNode)
                    {
                        XmlAttribute attCollapsed = xmlChildren.Attributes["collapsed"];
                        if (attCollapsed is XmlAttribute && !string.IsNullOrEmpty(attCollapsed.InnerText))
                        {
                            bool.TryParse(attCollapsed.InnerText, out collapsed);
                        }
                    }

                    TreeNode newNode = new TreeNode(label, 1, 1);
                    newNode.Tag = new ProjectTree.GroupLayer(label, collapsed, id);
                    tnParent.Nodes.Add(newNode);
                    tnParent = newNode;
                }

                // Finally process all child nodes
                xmlBusiness.ChildNodes.OfType<XmlNode>().ToList().ForEach(x => LoadTreeNode(tnParent, xmlProject, x));
            }
        }

        public static void GetAllNodes(List<TreeNode> nodes, TreeNode node)
        {
            // Add the current node to the list
            nodes.Add(node);
            foreach (TreeNode child in node.Nodes)
                GetAllNodes(nodes, child);
        }

        private void AddGISNode(TreeNode tnParent, string type, XmlNode nodGISNode, string symbology, string label, short transparency, string id)
        {
            if (nodGISNode == null)
                return;


            // If the project node has a ref attribute then lookup the redirect to the inputs
            XmlAttribute attRef = nodGISNode.Attributes["ref"];
            if (attRef is XmlAttribute)
            {
                nodGISNode = nodGISNode.OwnerDocument.SelectSingleNode(string.Format("Project/Inputs/*[@id='{0}']", attRef.InnerText));
            }

            if (string.IsNullOrEmpty(label))
                label = nodGISNode.SelectSingleNode("Name").InnerText;



            string path = nodGISNode.SelectSingleNode("Path").InnerText;

            if (string.Compare(nodGISNode.ParentNode.Name, "layers", true) == 0)
            {
                XmlNode nodGeoPackage = nodGISNode.SelectSingleNode("../../Path");
                if (nodGeoPackage is XmlNode)
                {
                    path = nodGeoPackage.InnerText + "/" + path;
                }
                else
                {
                    throw new MissingMemberException("Unable to find GeoPackage file path");
                }
            }

            string absPath = Path.Combine(ProjectFile.DirectoryName, path);

            // Load the layer metadata
            Dictionary<string, string> metadata = null;
            XmlNode nodMetadata = nodGISNode.SelectSingleNode("MetaData");
            if (nodMetadata is XmlNode && nodMetadata.HasChildNodes)
            {
                metadata = new Dictionary<string, string>();
                foreach (XmlNode nodMeta in nodMetadata.SelectNodes("Meta"))
                {
                    XmlAttribute attName = nodMeta.Attributes["name"];
                    if (attName is XmlAttribute && !string.IsNullOrEmpty(attName.InnerText))
                    {
                        if (!string.IsNullOrEmpty(nodMeta.InnerText))
                        {
                            metadata.Add(attName.InnerText, nodMeta.InnerText);
                        }
                    }
                }
            }

            ProjectTree.FileSystemDataset dataset = null;
            switch (type.ToLower())
            {
                case "file":
                case "report":
                    {
                        dataset = new ProjectTree.FileSystemDataset(this, label, new FileInfo(absPath), 0, 0, id);
                        break;
                    }

                case "raster":
                    {
                        dataset = new ProjectTree.Raster(this, label, absPath, symbology, transparency, id, metadata);
                        break;
                    }

                case "vector":
                case "line":
                case "point":
                case "polygon":
                    {
                        dataset = new ProjectTree.Vector(this, label, absPath, symbology, transparency, id, metadata);
                        break;
                    }

                case "tin":
                    {
                        dataset = new ProjectTree.TIN(this, label, absPath, transparency, id, metadata);
                        break;
                    }

                default:
                    throw new Exception(string.Format("Unhandled Node type attribute string '{0}'", type));
            }

            TreeNode newNode = new TreeNode(label, dataset.ImageIndex, dataset.ImageIndex);
            newNode.Tag = dataset;
            tnParent.Nodes.Add(newNode);
        }

        private static string GetXPath(XmlNode businessLogicNode, string xPath)
        {

            XmlAttribute attXPath = businessLogicNode.Attributes["xpath"];
            try
            {
                if (attXPath is XmlAttribute && !String.IsNullOrEmpty(attXPath.InnerText))
                {
                    if (!string.IsNullOrEmpty(xPath))
                        xPath += @"/";

                    xPath += attXPath.InnerText;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error attempting get the XPath", ex);
            }

            return xPath;
        }

        private static string GetLabel(XmlNode businessLogicNode, XmlNode projectNode)
        {
            try
            {
                if (businessLogicNode.Attributes != null)
                {
                    // See if the business logic has a label attribute.
                    XmlAttribute attLabel = businessLogicNode.Attributes["label"];
                    if (attLabel is XmlAttribute && !string.IsNullOrEmpty(attLabel.InnerText))
                    {
                        return attLabel.InnerText;
                    }

                    // See if the project node has a child Name node with valid inner text.
                    if (projectNode is XmlNode)
                    {
                        XmlNode nodName = projectNode.SelectSingleNode("Name");
                        if (nodName is XmlNode && !string.IsNullOrEmpty(nodName.InnerText))
                            return nodName.InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error attempting to get node label", ex);
            }

            return string.Empty;
        }
    }
}