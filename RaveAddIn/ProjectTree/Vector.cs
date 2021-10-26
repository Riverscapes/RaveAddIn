using System.Collections.Generic;
using System.IO;

namespace RaveAddIn.ProjectTree
{
    class Vector : GISDataset, IGISLayer
    {
        public readonly string DefinitionQuery;

        public Vector(RaveProject project, string name, string path, string symbology, short transparency, string id, Dictionary<string, string> metadata, string def_query)
            : base(project, name, new FileInfo(path), symbology, transparency, 3, 5, id, metadata)
        {
            DefinitionQuery = def_query;
        }
    }
}
