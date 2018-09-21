using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RaveAddIn.ProjectTree;
using ESRI.ArcGIS.Carto;

namespace RaveAddIn
{
    partial class ucProjectExplorer
    {
        private void BuildRasterCMS()
        {
            if (cmsRaster != null)
                return;

            cmsRaster = new ContextMenuStrip(components);
            cmsRaster.Items.Add("Add Raster To Map", Properties.Resources.BrowseFolder, OnAddRaster);
        }

        public void OnAddRaster(object sender, EventArgs e)
        {
            TreeNode selNode = treProject.SelectedNode;
            IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(selNode);
            Raster raster = (Raster)selNode.Tag;
            ArcMapUtilities.AddToMap(raster.GISFileInfo, raster.Name, parentGrpLyr);
        }
    }
}
