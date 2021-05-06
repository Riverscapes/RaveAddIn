using System.Collections.Generic;
using System.IO;

namespace RaveAddIn.ProjectTree
{
    class Vector : GISDataset, IGISLayer
    {
        public Vector(RaveProject project, string name, string path, string symbology, short transparency, int imageIndex, string id, Dictionary<string, string> metadata)
            : base(project, name, new FileInfo(path), symbology, transparency, imageIndex, imageIndex, id, metadata)
        {

        }
    }
}
