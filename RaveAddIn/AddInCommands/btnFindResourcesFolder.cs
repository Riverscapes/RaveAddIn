using System;
using System.IO;
using System.Windows.Forms;

namespace RaveAddIn.AddInCommands
{
    public class btnFindResourcesFolder : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            string app_data = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (Directory.Exists(app_data))
            {
                string rave_data = Path.Combine(app_data, Properties.Resources.AppDataFolder);
                if (Directory.Exists(rave_data))
                    System.Diagnostics.Process.Start(rave_data);
                else
                    System.Diagnostics.Process.Start(app_data);
            }
            else
                MessageBox.Show("The system APPData folder does not exist.", "AppData Folder Missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
