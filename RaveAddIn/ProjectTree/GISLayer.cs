using System.IO;

namespace RaveAddIn.ProjectTree
{
    public class GISLayer : ProjectDataset
    {
        public readonly string SymbologyKey;

        public GISLayer(RaveProject project, FileInfo filePath, string name, string symbologyKey)
            : base(project, filePath, name)
        {
            SymbologyKey = symbologyKey;
        }
    }
}
