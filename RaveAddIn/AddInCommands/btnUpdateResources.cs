using System;
using System.Windows.Forms;

namespace RaveAddIn.AddInCommands
{
    public class btnUpdateResources : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to update the RAVE AddIn resources?", "Update Resources", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                      == DialogResult.Yes)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    ResourceUpdater rru = new ResourceUpdater(Properties.Resources.ResourcesURL, Properties.Resources.BusinessLogicXMLFolder, Properties.Resources.AppDataSymbologyFolder);

                    string appDataResources = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Resources.AppDataFolder);
                    ResourceUpdater.UpdateResults results = rru.Update(appDataResources);

                    Cursor.Current = Cursors.Default;
                    MessageBox.Show(string.Format("The RAVE resources were updated successfully.\n{0} resource files were updated.", results.business_logic.downloaded + results.symbology_lyrs.downloaded), "Resources Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                ErrorHandling.frmException.HandleException(ex, "Error Showing RAVE About Form", string.Empty);
            }

            ArcMap.Application.CurrentTool = null;
        }
    }

}
