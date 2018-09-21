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
        private void BuildVectorCMS()
        {
            if (cmsVector != null)
                return;

            cmsVector = new ContextMenuStrip(components);
            cmsVector.Items.Add("Add Vector To Map", Properties.Resources.BrowseFolder, OnAddVector);
        }

        public void OnAddVector(object sender, EventArgs e)
        {
            TreeNode selNode = treProject.SelectedNode;
            IGroupLayer parentGrpLyr = BuildArcMapGroupLayers(selNode);
            Vector vector = (Vector)selNode.Tag;
            ArcMapUtilities.AddToMap(vector.GISFileInfo, vector.Name, parentGrpLyr);
        }
    }
}
