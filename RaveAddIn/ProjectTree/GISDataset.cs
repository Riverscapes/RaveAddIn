using System.IO;

namespace RaveAddIn.ProjectTree
{
    class GISDataset : FileSystemDataset, IGISLayer
    {
        public string SymbologyKey { get; private set; }
        public short Transparency { get; private set; }

        public string GISPath { get { return Path.FullName; } }

        public GISDataset(RaveProject project, string name, FileSystemInfo fsInfo, string symbologyKey, short transparency, int imageIndex)
            : base(project, name, fsInfo, imageIndex)
        {
            SymbologyKey = symbologyKey;
            Transparency = transparency;
        }
    }
}
