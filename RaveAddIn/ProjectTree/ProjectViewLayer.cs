using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaveAddIn.ProjectTree
{
    public class ProjectViewLayer
    {
        public readonly TreeNode LayerNode;
        public readonly bool Visible;

        public ProjectViewLayer(TreeNode layerNode, bool visible)
        {
            LayerNode = layerNode;
            Visible = visible;
        }
    }
}
