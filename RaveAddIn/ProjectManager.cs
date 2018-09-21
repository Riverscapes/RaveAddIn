using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace RaveAddIn
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Base class for the GCD project manager. This should only contain
    /// members needed by both console and UI applications. The GCD still uses the inherited
    /// GCDProjectManager class that has more members that are specific to
    /// the desktop software.</remarks>
    public class ProjectManager
    {
        public const string RasterExtension = "tif";

        public static List<RaveProject> Projects { get; internal set; }

 
        // Simplifies the path combinations above
        public static DirectoryInfo CombinePaths(DirectoryInfo parentDir, string subDir) { return new DirectoryInfo(Path.Combine(parentDir.FullName, subDir)); }

        public static RaveProject OpenProject(FileInfo projectFile)
        {
            if (Projects == null)
                Projects = new List<RaveProject>();

            if (BusinessLogicXML == null)
            {
                BusinessLogicXML = new Dictionary<string, List<BusinessLogicXML>>();
                LoadBusinessLogicXML();
            }

            if (Projects.Any(x => string.Compare(x.ProjectFile.FullName, projectFile.FullName, true) == 0))
            {
                return Projects.First(x => string.Compare(x.ProjectFile.FullName, projectFile.FullName, true) == 0);
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(projectFile.FullName);

            string projectName = xmlDoc.SelectSingleNode("Project/Name").InnerText;
            string projectType = xmlDoc.SelectSingleNode("Project/ProjectType").InnerText;

            if (!ProjectManager.BusinessLogicXML.ContainsKey(projectType))
            {
                MessageBox.Show(string.Format("Failed to load project because there is no business logic file for projects of type '{0}'.", projectType), "Failed To Load Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            RaveProject project = new RaveProject(projectFile, projectName, projectType);
            Projects.Add(project);
            return project;
        }

      


    }
}