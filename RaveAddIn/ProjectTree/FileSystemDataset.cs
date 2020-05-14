using System.IO;

namespace RaveAddIn.ProjectTree
{
     class FileSystemDataset : BaseDataset
    {
        public readonly RaveProject Project;
        public readonly  FileSystemInfo Path;

        public FileSystemDataset(RaveProject project, string name, FileSystemInfo fsInfo, int imageIndex)
            : base(name, imageIndex)
        {
            Project = project;
            Path = fsInfo;
        }
    }
}
