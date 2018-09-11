using System;
using System.Windows.Forms;

namespace RaveAddIn.AddInCommands
{
    class btnOpenProject : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            try
            {
                OpenFileDialog f = new OpenFileDialog();
                f.DefaultExt = "xml";
                f.Filter = "Riverscapes Project Files (*.rs.xml)|*.rs.xml";
                f.Title = "Open Existing Riverscapes Project";
                f.CheckFileExists = true;

                if (!string.IsNullOrEmpty(RaveAddIn.Properties.Settings.Default.LastUsedProjectFolder) && System.IO.Directory.Exists(RaveAddIn.Properties.Settings.Default.LastUsedProjectFolder))
                {
                    f.InitialDirectory = RaveAddIn.Properties.Settings.Default.LastUsedProjectFolder;

                    // Try and find the last used project in the folder
                    string[] fis = System.IO.Directory.GetFiles(RaveAddIn.Properties.Settings.Default.LastUsedProjectFolder, "*.rs.xml", System.IO.SearchOption.TopDirectoryOnly);
                    if (fis.Length > 0)
                    {
                        f.FileName = System.IO.Path.GetFileName(fis[0]);
                    }
                }

                if (f.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ProjectManager.OpenProject(new System.IO.FileInfo(f.FileName));
                        Properties.Settings.Default.LastUsedProjectFolder = System.IO.Path.GetDirectoryName(f.FileName);
                        Properties.Settings.Default.Save();

                        btnProjectExplorer.ShowProjectExplorer(true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error reading the GCD project file '{0}'. Ensure that the file is a valid GCD project file with valid and complete XML contents.\n\n{1}", f.FileName, ex.Message), Properties.Resources.ApplicationNameLong, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                RaveException.HandleException(ex);
            }

            ArcMap.Application.CurrentTool = null;
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }
}