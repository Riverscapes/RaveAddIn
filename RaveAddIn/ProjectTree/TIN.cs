using System.IO;

namespace RaveAddIn.ProjectTree
{
    class TIN : GISDataset, IGISLayer
    {
        public TIN(RaveProject project, string name, string path)
            : base(project, name, new DirectoryInfo(path), string.Empty, Directory.Exists(path) ? 6: 7)
        {

        }
    }
}
