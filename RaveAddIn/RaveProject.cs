using System.IO;
using System.Xml;

namespace RaveAddIn
{
    public class RaveProject
    {
        public readonly FileInfo ProjectFile;
        public readonly FileInfo BusinessLogic;
        public readonly string Name;
        public readonly string ProjectType;

        public RaveProject(FileInfo projectFile, FileInfo businessLogic, string name, string projectType)
        {
            ProjectFile = projectFile;
            BusinessLogic = businessLogic;
            Name = name;
            ProjectType = projectType;
        }

        public static bool IsSame(RaveProject proj1, RaveProject proj2)
        {
            return string.Compare(proj1.ProjectFile.FullName, proj2.ProjectFile.FullName) == 0;
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
    }
}