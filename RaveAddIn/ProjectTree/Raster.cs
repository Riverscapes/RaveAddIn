using System.Collections.Generic;
using System.IO;

namespace RaveAddIn.ProjectTree
{
    class Raster : GISDataset
    {
        public Raster(RaveProject project, string name, string path, string symbology, short transparency, string id, Dictionary<string, string> metadata)
            : base(project, name, new FileInfo(path), symbology, transparency, 8, 8, id, metadata)
        {

        }
    }
}
