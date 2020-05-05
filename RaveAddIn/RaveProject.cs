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

        private FileInfo AbsolutePath(string relativePath)
        {
            return new FileInfo(Path.Combine(ProjectFile.DirectoryName, relativePath));
        }

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
                if (!Directory.Exists(folder))
                    continue;

                foreach (string xmlPath in Directory.GetFiles(folder, "*.xml"))
                {
                    // Ignore project files that also end in *.xml
                    if (xmlPath.ToLower().EndsWith(".rs.xml"))
                        continue;

                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(xmlPath);

                        XmlNode nodProjectType = xmlDoc.SelectSingleNode("Project/ProjectType");
                        XmlNode nodName = xmlDoc.SelectSingleNode("Project/Name");

                        if (nodProjectType is XmlNode && !string.IsNullOrEmpty(nodProjectType.InnerText))
                        {
                            if (string.Compare(nodProjectType.InnerText, ProjectType, true) == 0)
                            {
                                return new FileInfo(xmlPath);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Do nothing, continue onto the next business logic file
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

        /// <summary>
        /// Load a project into the tree that doesn't already exist
        /// </summary>
        /// <param name="treProject"></param>
        /// <returns></returns>
        public TreeNode LoadNewProject(TreeView treProject)
        {
            TreeNode tnProject = new TreeNode("TITLE_NOT_FOUND", 1, 1);
            tnProject.Tag = this;
            treProject.Nodes.Insert(0, tnProject);

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

            // Loop over all child nodes of the business logic XML and load them to the tree
            nodBLRoot.ChildNodes.OfType<XmlNode>().ToList().ForEach(x => LoadTreeNode(tnProject, x, projectXMLRoot, string.Empty));

            // Expand the project tree node now that all the items have been added
            tnProject.ExpandAll();

            // Loop over all tree nodes and collapse any group layers.
            // This has to be done last once all the nodes have their children
            List<TreeNode> allNodes = new List<TreeNode>();
            foreach (TreeNode node in tnProject.Nodes)
                GetAllNodes(allNodes, node);
            allNodes.Where(x => x.Tag is string && string.Compare(x.Tag.ToString(), "Collapse", true) == 0).ToList().ForEach(x => x.Collapse());

            return tnProject;
        }

        private void LoadTreeNode(TreeNode tnParent, XmlNode xmlBusiness, XmlNode xmlProject, string xPath)
        {
            if (xmlBusiness.NodeType == XmlNodeType.Comment)
                return;

            string label = GetLabel(xmlBusiness, xmlProject);

            if (xmlBusiness.Name == "Repeater")
            {
                TreeNode newNode = new TreeNode(label, 1, 1);
                tnParent.Nodes.Add(newNode);
                tnParent = newNode;
                xPath = GetXPath(xmlBusiness, string.Empty);

                foreach (XmlNode projChild in xmlProject.SelectNodes(xPath))
                {
                    foreach (XmlNode busChild in xmlBusiness.ChildNodes)
                    {
                        LoadTreeNode(tnParent, busChild, projChild, string.Empty);
                    }
                }
            }
            else if (xmlBusiness.Name == "Node")
            {
                xPath = GetXPath(xmlBusiness, xPath);

                XmlAttribute attType = xmlBusiness.Attributes["type"];
                if (attType is XmlAttribute)
                {
                    XmlNode gisNode = xmlProject;
                    if (!string.IsNullOrEmpty(xPath))
                        gisNode = xmlProject.SelectSingleNode(xPath);

                    label = GetLabel(xmlBusiness, gisNode);

                    // Retrieve symbology key from business logic
                    string symbology = string.Empty;
                    XmlAttribute attSym = xmlBusiness.Attributes["symbology"];
                    if (attSym is XmlAttribute && !String.IsNullOrEmpty(attSym.InnerText))
                        symbology = attSym.InnerText;

                    // This some kind of file (vector, raster, tile, image etc)
                    AddGISNode(tnParent, attType.InnerText, gisNode, symbology, label);
                }
                else
                {
                    // Group Layer / Folder
                    TreeNode newNode = new TreeNode(label, 1, 1);
                    tnParent.Nodes.Add(newNode);
                    tnParent = newNode;

                    XmlAttribute attCollapsed = xmlBusiness.Attributes["collapsed"];
                    if (attCollapsed is XmlAttribute)
                    {
                        bool collapsed = false;
                        if (bool.TryParse(attCollapsed.InnerText, out collapsed))
                            if (collapsed)
                                newNode.Tag = "Collapse";
                    }
                }
            }

            // Loop over all child nodes if not a repeater (repeaters handle their own children above)
            if (string.Compare(xmlBusiness.Name, "Repeater", true) != 0)
            {
                xmlBusiness.ChildNodes.OfType<XmlNode>().ToList().ForEach(x => LoadTreeNode(tnParent, x, xmlProject, xPath));
            }
        }

        private void GetAllNodes(List<TreeNode> nodes, TreeNode node)
        {
            // Add the current node to the list
            nodes.Add(node);
            foreach (TreeNode child in node.Nodes)
                GetAllNodes(nodes, child);
        }

        private void AddGISNode(TreeNode tnParent, string type, XmlNode nodGISNode, string symbology, string label)
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
            FileInfo absPath = AbsolutePath(path);

            int imgIndex = 0; // Default is riverscapes logo
            switch (type.ToLower())
            {
                case "raster":
                    imgIndex = absPath.Exists ? 2 : 4;
                    break;

                case "vector":
                    imgIndex = absPath.Exists ? 3 : 5;
                    break;                    
            }

            TreeNode newNode = new TreeNode(label, imgIndex, imgIndex);

            if (string.Compare(type, "file", true) == 0)
            {
                newNode.Tag = new ProjectTree.ProjectDataset(this, absPath, label);
            }
            else
            {
                newNode.Tag = new ProjectTree.GISLayer(this, absPath, label, symbology);
            }

            tnParent.Nodes.Add(newNode);
        }

        private static string GetXPath(XmlNode businessLogicNode, string xPath)
        {
            XmlAttribute attXPath = businessLogicNode.Attributes["xpath"];
            if (attXPath is XmlAttribute && !String.IsNullOrEmpty(attXPath.InnerText))
            {
                if (!string.IsNullOrEmpty(xPath))
                    xPath += @"/";

                xPath += attXPath.InnerText;
            }

            return xPath;
        }

        private static string GetLabel(XmlNode businessLogicNode, XmlNode projectNode)
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

            return string.Empty;
        }
    }
}