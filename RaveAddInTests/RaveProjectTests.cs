using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaveAddIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaveAddIn.Tests
{
    [TestClass()]
    public class RaveProjectTests
    {
        [TestMethod()]
        public void LoadTreeTest()
        {
            TreeView treProject = new TreeView();

            RaveProject proj = new RaveProject(new System.IO.FileInfo(@"C:\Users\philip\RiverscapesData\CRB\Watershed\Asotin\Network\VBET\project.rs.xml"));

            proj.LoadTree(treProject);

            treProject.Nodes.OfType<TreeNode>().ToList().ForEach(x => PrintTree(x, 0));
        }

        private void PrintTree(TreeNode node, int indent)
        {
            System.Diagnostics.Debug.Print("{0}{1}", new String('\t', indent), node.Text);
            indent += 1;
            node.Nodes.OfType<TreeNode>().ToList().ForEach(x => PrintTree(x, indent));
        }
    }
}