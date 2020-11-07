using System.IO;

namespace RaveAddIn.ProjectTree
{
    class Raster : GISDataset
    {
        public Raster(RaveProject project, string name, string path, string symbology, short transparency)
            : base(project, name, new FileInfo(path), symbology, transparency, 2, 4)
        {

        }
    }
}
