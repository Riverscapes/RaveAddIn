using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaveAddIn.ProjectTree
{
    public class GISItem
    {
        public readonly RaveProject Project;
        public readonly string Name;
        public readonly FileInfo GISFileInfo;
        public readonly string SymbologyKey;
        //public FileInfo LayerFile { get; internal set; }

        /// <summary>
        /// Determine the location of the layer file for this GIS item
        /// </summary>
        /// <remarks>
        /// The following locations will be searched in order for a 
        /// file with the name SYMBOLOGY_KEY.lyr
        /// 
        /// 1. ProjectFolder
        /// 2. %APPDATA%\RAVE\MODEL
        /// 3. %APPDATA%\RAVE\Shared
        /// 4. SOFTWARE_DEPLOYMENT\MODEL
        /// 5. SOFTWARE_DEPLOYMENT\Shared
        /// 
        /// </remarks>
        public GISItem(RaveProject project, FileInfo filePath, string name, string symbologyKey)
        {
            Project = project;
            GISFileInfo = filePath;
            Name = name;
            SymbologyKey = symbologyKey;
            
            

        }
    }
}
