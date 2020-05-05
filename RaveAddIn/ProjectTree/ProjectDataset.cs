using System.IO;

namespace RaveAddIn.ProjectTree
{
    public class ProjectDataset
    {
        public readonly RaveProject Project;
        public readonly string Name;
        public readonly FileInfo FilePath;

        public ProjectDataset(RaveProject project, FileInfo filePath, string name)
        {
            Project = project;
            FilePath = filePath;
            Name = name;
        }
    }
}
