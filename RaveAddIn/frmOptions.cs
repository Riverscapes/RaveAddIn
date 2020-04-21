using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace RaveAddIn
{
    public partial class frmOptions : Form
    {
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
                foreach (string folder in new string[] { ucProjectExplorer.AppDataFolder.FullName, ucProjectExplorer.DeployFolder.FullName })
                {
                    string baseMapPath = Path.Combine(folder, "BaseMaps.xml");
                    if (System.IO.File.Exists(baseMapPath))
                    {
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
                                        if (!cboRegion.Items.Contains(name))
                                            cboRegion.Items.Add(name);
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
            }
            catch
            {
                // Do nothing proces
                Console.WriteLine("Error loading base map regions.");
            }

            for (int i = 0; i < cboRegion.Items.Count; i++)
            {
                if (string.Compare(cboRegion.Items[i].ToString(), Properties.Settings.Default.BaseMap, true) == 0)
                {
                    cboRegion.SelectedIndex = i;
                    break;
                }
            }

            if (cboRegion.SelectedIndex < 0 && cboRegion.Items.Count > 0)
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

            if (chkLoadBaseMaps.Checked && cboRegion.SelectedIndex >= 0)
                Properties.Settings.Default.BaseMap = cboRegion.Text;
            Properties.Settings.Default.Save();
        }

        private void cmdHelp_Click(object sender, EventArgs e)
        {

        }
    }
}
