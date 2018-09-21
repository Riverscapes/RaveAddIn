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
        //public readonly string Name;
        //public readonly string ProjectType;

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


        public RaveProject(FileInfo projectFile)//, string name, string projectType)
        {
            ProjectFile = projectFile;
            //Name = name;
            //ProjectType = projectType;
        }

        public static bool IsSame(RaveProject proj1, RaveProject proj2)
        {
            return IsSame(proj1, proj2.ProjectFile);
        }

        public static bool IsSame(RaveProject proj1, FileInfo projectFile)
        {
            return string.Compare(proj1.ProjectFile.FullName, projectFile.FullName) == 0;
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

        public void LoadTree(TreeView treProject, ContextMenuStrip cmsProject)
        {
            XmlDocument xmlProject = new XmlDocument();
            xmlProject.Load(ProjectFile.FullName);

            string projectType = xmlProject.SelectSingleNode("Project/ProjectType").InnerText;

            if (!BusinessLogicXML.ContainsKey(projectType))
            {
                MessageBox.Show(string.Format("Failed to load project because there is no business logic file for projects of type '{0}'.", projectType), "Failed To Load Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            XmlDocument xmlBusiness = new XmlDocument();
            xmlBusiness.Load(BusinessLogicXML[projectType].First().XMLPath.FullName);

            XmlNode nodBLRoot = xmlBusiness.SelectSingleNode("Project/Node");
            if (!(nodBLRoot is XmlNode))
            {
                MessageBox.Show("Business logic XML file does not contain 'Project/Node' XPath.", "Business Logic Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            XmlNode projectXMLRoot = xmlProject.SelectSingleNode("Project");
            string projectName = GetLabel(nodBLRoot, projectXMLRoot);

            TreeNode tnProject = new TreeNode(projectName, 1, 1);
            treProject.Nodes.Add(tnProject);
            tnProject.ContextMenuStrip = cmsProject;

            // Loop over all child nodes of the business logic XML and load them to the tree
            nodBLRoot.ChildNodes.OfType<XmlNode>().ToList().ForEach(x => LoadTreeNode(tnProject, x, projectXMLRoot, string.Empty));

            // Expand the project tree node now that all the items have been added
            tnProject.ExpandAll();
        }

        private static void LoadTreeNode(TreeNode tnParent, XmlNode xmlBusiness, XmlNode xmlProject, string xPath)
        {
            if (xmlBusiness.NodeType == XmlNodeType.Comment)
                return;

            //if (xmlBusiness.Name != "Children")
            //{
            //    System.Diagnostics.Debug.Print("---------------");
            //    System.Diagnostics.Debug.Print(tnParent.Text);
            //    System.Diagnostics.Debug.Print(xmlBusiness.Name);
            //    System.Diagnostics.Debug.Print(xmlProject.Name);
            //    System.Diagnostics.Debug.Print(xPath);
            //}

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
                PrintBusinessNode(xmlBusiness);

                xPath = GetXPath(xmlBusiness, xPath);

                XmlAttribute attType = xmlBusiness.Attributes["type"];
                if (attType is XmlAttribute)
                {
                    // This some kind of file (vector, raster, tile, image etc)
                    AddGISNode(tnParent, attType.InnerText, xmlProject.SelectSingleNode(xPath));

                }
                else
                {
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

        private static void AddGISNode(TreeNode tnParent, string type, XmlNode nodGISNode)
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

            int imgIndex = 0; // Default is riverscapes logo
            switch (type.ToLower())
            {
                case "raster": imgIndex = 2; break;
                case "vector": imgIndex = 3; break;
            }

            TreeNode newNode = new TreeNode(name, imgIndex, imgIndex);
            tnParent.Nodes.Add(newNode);

        }

        private static void PrintBusinessNode(XmlNode nod)
        {
            System.Diagnostics.Debug.Print("\nBL Node: " + nod.Name);
            PrintAttributes(nod, "xpathlabel");
            PrintAttributes(nod, "label");
            PrintAttributes(nod, "xpath");
            PrintAttributes(nod, "type");
        }

        private static void PrintAttributes(XmlNode nod, string attribute)
        {
            XmlAttribute attXPath = nod.Attributes[attribute];
            if (attXPath is XmlAttribute)
            {
                if (string.IsNullOrEmpty(attXPath.InnerText))
                {
                    System.Diagnostics.Debug.Print(string.Format("{0}: <Empty>", attribute));
                }
                else
                {
                    System.Diagnostics.Debug.Print(string.Format("{0}: {1}", attribute, attXPath.InnerText));
                }
            }
            else
            {
                System.Diagnostics.Debug.Print(string.Format("{0}: <Missing>", attribute));
            }

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