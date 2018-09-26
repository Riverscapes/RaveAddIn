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

        private static Dictionary<string, List<BusinessLogicXML>> _BusinessLogicXML;
        public static Dictionary<string, List<BusinessLogicXML>> BusinessLogicXML
        {
            get
            {
                if (_BusinessLogicXML == null)
                {
                    _BusinessLogicXML = new Dictionary<string, List<RaveAddIn.BusinessLogicXML>>();
                    LoadBusinessLogicXML();
                }

                return _BusinessLogicXML;
            }
        }
        
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

        private static void LoadBusinessLogicXML()
        {
            string folder = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "XML");

            foreach (string xmlPath in Directory.GetFiles(folder, "*.xml"))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlPath);

                    XmlNode nodProjectType = xmlDoc.SelectSingleNode("Project/ProjectType");
                    XmlNode nodName = xmlDoc.SelectSingleNode("Project/Name");

                    if (nodProjectType is XmlNode && !string.IsNullOrEmpty(nodProjectType.InnerText))
                    {
                        if (!BusinessLogicXML.ContainsKey(nodProjectType.InnerText))
                        {
                            BusinessLogicXML[nodProjectType.InnerText] = new List<BusinessLogicXML>();
                        }

                        BusinessLogicXML[nodProjectType.InnerText].Add(new BusinessLogicXML(nodName.InnerText, nodProjectType.InnerText, xmlPath));
                    }
                }
                catch (Exception)
                {
                    // Do nothing, continue onto the next business logic file
                }
            }
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

        public TreeNode LoadTree(TreeView treProject)
        {
            // Determine the type of project
            XmlDocument xmlProject = new XmlDocument();
            xmlProject.Load(ProjectFile.FullName);
            string projectType = xmlProject.SelectSingleNode("Project/ProjectType").InnerText;

            if (!BusinessLogicXML.ContainsKey(projectType))
            {
                MessageBox.Show(string.Format("Failed to load project because there is no business logic file for projects of type '{0}'.", projectType), "Failed To Load Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            FileInfo businessLogicXML = BusinessLogicXML[projectType].First().XMLPath;

            TreeNode tnProject = new TreeNode("TITLE_NOT_FOUND", 1, 1);
            tnProject.Tag = this;
            treProject.Nodes.Add(tnProject);

            return LoadTree(tnProject, businessLogicXML);
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

            return tnProject;
        }

        private void LoadTreeNode(TreeNode tnParent, XmlNode xmlBusiness, XmlNode xmlProject, string xPath)
        {
            if (xmlBusiness.NodeType == XmlNodeType.Comment)
                return;

            if (xmlBusiness.Name == "Repeater")
            {
                string label = GetLabel(xmlBusiness, xmlProject);
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

                    // Retrieve symbology key from business logic
                    string symbology = string.Empty;
                    XmlAttribute attSym = xmlBusiness.Attributes["symbology"];
                    if (attSym is XmlAttribute && !String.IsNullOrEmpty(attSym.InnerText))
                        symbology = attSym.InnerText;

                    // This some kind of file (vector, raster, tile, image etc)
                    AddGISNode(tnParent, attType.InnerText, gisNode, symbology);
                }
                else
                {
                    // Group Layer / Folder
                    string label = GetLabel(xmlBusiness, xmlProject);
                    TreeNode newNode = new TreeNode(label, 1, 1);
                    tnParent.Nodes.Add(newNode);
                    tnParent = newNode;
                }
            }

            // Loop over all child nodes if not a repeater (repeaters handle their own children above)
            if (string.Compare(xmlBusiness.Name, "Repeater", true) != 0)
            {
                xmlBusiness.ChildNodes.OfType<XmlNode>().ToList().ForEach(x => LoadTreeNode(tnParent, x, xmlProject, xPath));
            }
        }

        private void AddGISNode(TreeNode tnParent, string type, XmlNode nodGISNode, string symbology)
        {
            if (nodGISNode == null)
                return;

            // If the project node has a ref attribute then lookup the redirect to the inputs
            XmlAttribute attRef = nodGISNode.Attributes["ref"];
            if (attRef is XmlAttribute)
            {
                nodGISNode = nodGISNode.OwnerDocument.SelectSingleNode(string.Format("Project/Inputs/*[@id='{0}']", attRef.InnerText));
            }

            string name = nodGISNode.SelectSingleNode("Name").InnerText;
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

            TreeNode newNode = new TreeNode(name, imgIndex, imgIndex);
            newNode.Tag = new ProjectTree.GISItem(this, absPath, name, symbology);
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
            XmlAttribute attXPath = businessLogicNode.Attributes["xpathlabel"];
            if (attXPath is XmlAttribute && !string.IsNullOrEmpty(attXPath.InnerText))
            {
                XmlNode labelNode = projectNode.SelectSingleNode(attXPath.InnerText);
                if (labelNode is XmlNode && !string.IsNullOrEmpty(labelNode.InnerText))
                    return labelNode.InnerText;
            }

            XmlAttribute attLabel = businessLogicNode.Attributes["label"];
            if (attLabel is XmlAttribute && !string.IsNullOrEmpty(attLabel.InnerText))
            {
                return attLabel.InnerText;
            }

            return "TITLE_NOT_FOUND";
        }
    }
}