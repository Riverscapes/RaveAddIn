using System;
using System.IO;

namespace RaveAddIn.ProjectTree
{
    public class GISItem
    {
        public readonly RaveProject Project;
        public readonly string Name;
        public readonly FileInfo GISFileInfo;
        public readonly string SymbologyKey;

        public GISItem(RaveProject project, FileInfo filePath, string name, string symbologyKey)
        {
            Project = project;
            GISFileInfo = filePath;
            Name = name;
            SymbologyKey = symbologyKey;
        }
    }
}
