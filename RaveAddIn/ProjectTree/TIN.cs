using System.Collections.Generic;
using System.IO;

namespace RaveAddIn.ProjectTree
{
    class TIN : GISDataset, IGISLayer
    {
        public TIN(RaveProject project, string name, string path, short transparency, string id, Dictionary<string, string> metadata)
            : base(project, name, new DirectoryInfo(path), string.Empty, transparency, 2, 2, id, metadata)
        {

        }
    }
}
