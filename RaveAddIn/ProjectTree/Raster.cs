using System.IO;

namespace RaveAddIn.ProjectTree
{
    class Raster : GISDataset
    {
        public Raster(RaveProject project, string name, string path, string symbology, short transparency)
            : base(project, name, new FileInfo(path), symbology, transparency, File.Exists(path) ? 2 : 4)
        {

        }
    }
}
