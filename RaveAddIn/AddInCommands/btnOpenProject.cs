using System;
using System.IO;
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
                        ESRI.ArcGIS.esriSystem.IUID pUI = new ESRI.ArcGIS.esriSystem.UID();
                        pUI.Value = ThisAddIn.IDs.ucProjectExplorer;

                        ESRI.ArcGIS.Framework.IDockableWindow docWin = ArcMap.DockableWindowManager.GetDockableWindow((ESRI.ArcGIS.esriSystem.UID)pUI);
                        if (docWin is ESRI.ArcGIS.Framework.IDockableWindow)
                        {
                            // Try and refresh the project window.
                            ucProjectExplorer.AddinImpl winImpl = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID<ucProjectExplorer.AddinImpl>(ThisAddIn.IDs.ucProjectExplorer);
                            winImpl.UI.LoadProject(new System.IO.FileInfo(f.FileName));
                        }

                        //ProjectManager.OpenProject(new System.IO.FileInfo(f.FileName));
                        Properties.Settings.Default.LastUsedProjectFolder = System.IO.Path.GetDirectoryName(f.FileName);
                        Properties.Settings.Default.Save();

                        // This will cause the project tree to reload all open projects
                        btnProjectExplorer.ShowProjectExplorer(true);
                    }
                    catch (FileLoadException exFile)
                    {
                        MessageBox.Show(exFile.Message, "Invalid Business Logic File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error reading the project file '{0}'. Ensure that the file is a valid project file with valid and complete XML contents.\n\n{1}", f.FileName, ex.Message), Properties.Resources.ApplicationNameLong, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.frmException.HandleException(ex, "Error Opening Project", string.Empty);
            }

            ArcMap.Application.CurrentTool = null;
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }
}