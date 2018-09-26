using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RaveAddIn.ProjectTree;
using ESRI.ArcGIS.Carto;
using System.IO;

namespace RaveAddIn
{
    partial class ucProjectExplorer
    {
        private void BuildGISCMS()
        {
            if (cmsGIS != null)
                return;

            cmsGIS = new ContextMenuStrip(components);
            cmsGIS.Items.Add("Add To Map", Properties.Resources.AddToMap, OnAddGISToMap);
        }

        public void OnAddGISToMap(object sender, EventArgs e)
        {
            TreeNode selNode = treProject.SelectedNode;
            IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(selNode);
            GISItem layer = (GISItem)selNode.Tag;

            FileInfo symbology = GetSymbology(layer);

            ArcMapUtilities.AddToMap(layer.GISFileInfo, layer.Name, parentGrpLyr, symbology);
        }

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
        private FileInfo GetSymbology(GISItem layer)
        {
            string appDataFolder = Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), Properties.Resources.AppDataFolder, Properties.Resources.AppDataSymbologyFolder);
            string deployFolder = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Resources.AppDataSymbologyFolder);

            List<string> SearchFolders = new List<string>()
            {
                layer.Project.Folder.FullName,
                Path.Combine(appDataFolder, layer.Project.ProjectType),
                Path.Combine(appDataFolder, Properties.Resources.AppDataSymbologySharedFolder),
                Path.Combine(deployFolder, layer.Project.ProjectType),
                Path.Combine(deployFolder, Properties.Resources.AppDataSymbologySharedFolder),
            };

            foreach (string folder in SearchFolders)
            {
                if (Directory.Exists(folder))
                {
                    string path = Path.ChangeExtension(Path.Combine(folder, layer.SymbologyKey), "lyr");
                    if (File.Exists(path))
                    {
                        return new FileInfo(path);
                    }
                }
            }

            return null;
        }
    }
}
