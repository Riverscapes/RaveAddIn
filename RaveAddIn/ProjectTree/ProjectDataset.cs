using System.IO;

namespace RaveAddIn.ProjectTree
{
    public class ProjectDataset
    {
        public readonly RaveProject Project;
        public readonly string Name;
        public readonly FileSystemInfo FilePath; // TINs

        public ProjectDataset(RaveProject project, FileSystemInfo filePath, string name)
        {
            filePath
            Project = project;
            FilePath = filePath;
            Name = name;
        }
    }
}
