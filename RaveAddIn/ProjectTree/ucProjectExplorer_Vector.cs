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
        private void BuildVectorCMS()
        {
            if (cmsRaster != null)
                return;

            cmsProject = new ContextMenuStrip(components);
            cmsProject.Items.Add("Add Vector To Map", Properties.Resources.BrowseFolder, OnAddVector);
        }

        public void OnAddVector(object sender, EventArgs e)
        {
            Vector vector = (Vector)treProject.SelectedNode.Tag;
            MessageBox.Show(vector.FilePath.FullName, "Add To Map");
        }
    }
}
