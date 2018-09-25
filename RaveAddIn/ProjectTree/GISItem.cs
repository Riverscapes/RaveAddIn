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
        public FileInfo LayerFile { get; internal set; }

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
            
            string appDataFolder = Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), Properties.Resources.AppDataFolder, Properties.Resources.AppDataSymbologyFolder);
            string deployFolder = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Resources.AppDataSymbologyFolder);

            List<string> SearchFolders = new List<string>()
            {
                Project.Folder.FullName,
                Path.Combine(appDataFolder, Project.ProjectType),
                Path.Combine(appDataFolder, Properties.Resources.AppDataSymbologySharedFolder),
                Path.Combine(deployFolder, Project.ProjectType),
                Path.Combine(deployFolder, Properties.Resources.AppDataSymbologySharedFolder),
            };

            foreach (string folder in SearchFolders)
            {
                if (Directory.Exists(folder))
                {
                    string path = Path.ChangeExtension(Path.Combine(folder, SymbologyKey), "lyr");
                    if (File.Exists(path))
                    {
                        LayerFile = new FileInfo(path);
                        break;
                    }
                }
            }

        }
    }
}
