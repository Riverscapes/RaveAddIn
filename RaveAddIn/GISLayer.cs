using System.IO;

namespace RaveAddIn.ProjectTree
{
    public class GISLayer
    {
        public readonly RaveProject Project;
        public readonly string Name;
        public readonly FileInfo FilePath;
        public readonly string SymbologyKey;

        public GISLayer(RaveProject project, FileInfo filePath, string name, string symbologyKey)
        {
            Project = project;
            FilePath = filePath;
            Name = name;
            SymbologyKey = symbologyKey;
        }
    }
}
