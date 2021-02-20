using System.Collections.Generic;
using System.IO;

namespace RaveAddIn.ProjectTree
{
    class GISDataset : FileSystemDataset, IGISLayer
    {
        public string SymbologyKey { get; private set; }
        public short Transparency { get; private set; }

        public string GISPath { get { return Path.FullName; } }

        public readonly Dictionary<string, string> Metadata;

        public GISDataset(RaveProject project, string name, FileSystemInfo fsInfo, string symbologyKey, short transparency, int imageIndex_Exists, int imageIndex_Missing, string id, Dictionary<string, string> metadata)
            : base(project, name, fsInfo, imageIndex_Exists, imageIndex_Missing, id)
        {
            SymbologyKey = symbologyKey;
            Transparency = transparency;

            Metadata = metadata;
        }
    }
}
