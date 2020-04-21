using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace RaveAddIn
{
    public partial class frmOptions : Form
    {
        private class BaseMapRegion
        {
            public readonly string Name;
            public readonly bool IsSystem;

            public override string ToString()
            {
                return string.Format("{0} ({1})", Name, IsSystem ? "system" : "user");
            }

            public BaseMapRegion(string name, bool system)
            {
                Name = name;
                IsSystem = system;
            }
        }

        public frmOptions()
        {
            InitializeComponent();
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            chkLoadBaseMaps.Checked = Properties.Settings.Default.LoadBaseMaps;
            UpdateControls(sender, e);

            try
            {
                List<Tuple<string, bool>> searchFolders = new List<Tuple<string, bool>>()
                {
                    new Tuple<string, bool>(ucProjectExplorer.AppDataFolder.FullName, false),
                    new Tuple<string, bool>(ucProjectExplorer.DeployFolder.FullName, true)
                };

                foreach (Tuple<string, bool> location in searchFolders)
                {
                    string baseMapPath = Path.Combine(location.Item1, "BaseMaps.xml");
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(baseMapPath);
                    foreach (XmlNode nodRegion in xmlDoc.SelectNodes("BaseMaps/Region"))
                    {
                        try
                        {
                            XmlAttribute attName = nodRegion.Attributes["name"];
                            if (attName != null)
                            {
                                string name = nodRegion.Attributes["name"].InnerText;
                                if (!string.IsNullOrEmpty(name))
                                {
                                    int index = cboRegion.Items.Add(new BaseMapRegion(name, location.Item2));
                                    if (string.Compare(name, Properties.Settings.Default.BaseMap, true) == 0)
                                        cboRegion.SelectedIndex = index;
                                }
                            }
                        }
                        catch
                        {
                            // Do nothing. proceed to next region
                            Console.WriteLine("Error loading base map region.");
                        }
                    }
                }
            }
            catch
            {
                // Do nothing proces
                Console.WriteLine("Error loading base map regions.");
            }

            if (cboRegion.Items.Count > 0 && cboRegion.SelectedIndex < 0)
                cboRegion.SelectedIndex = 0;

            chkLoadBaseMaps.Enabled = cboRegion.Items.Count > 0;
        }

        private void UpdateControls(object sender, EventArgs e)
        {
            lblRegion.Enabled = chkLoadBaseMaps.Checked;
            cboRegion.Enabled = chkLoadBaseMaps.Checked;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LoadBaseMaps = chkLoadBaseMaps.Checked;

            if (chkLoadBaseMaps.Checked && cboRegion.SelectedItem is BaseMapRegion)
                Properties.Settings.Default.BaseMap = ((BaseMapRegion)cboRegion.SelectedItem).Name;
            Properties.Settings.Default.Save();
        }

        private void cmdHelp_Click(object sender, EventArgs e)
        {

        }
    }
}
