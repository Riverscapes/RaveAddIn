using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RaveAddIn.ProjectTree;

namespace RaveAddIn
{
    partial class ucProjectExplorer
    {
        private void BuildRasterCMS()
        {
            if (cmsRaster != null)
                return;

            cmsProject = new ContextMenuStrip(components);
            cmsProject.Items.Add("Add Raster To Map", Properties.Resources.BrowseFolder, OnAddRaster);
        }

        public void OnAddRaster(object sender, EventArgs e)
        {
            ProjectTree.Raster raster = (Raster)treProject.SelectedNode.Tag;
            MessageBox.Show(raster.FilePath.FullName, "Add To Map");
        }
    }
}
