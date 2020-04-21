using System;

namespace RaveAddIn.AddInCommands
{
    public class btnOptions : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            try
            {
                frmOptions frm = new frmOptions();
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ESRI.ArcGIS.esriSystem.IUID pUI = new ESRI.ArcGIS.esriSystem.UID();
                    pUI.Value = ThisAddIn.IDs.ucProjectExplorer;

                    ESRI.ArcGIS.Framework.IDockableWindow docWin = ArcMap.DockableWindowManager.GetDockableWindow((ESRI.ArcGIS.esriSystem.UID)pUI);
                    if (docWin is ESRI.ArcGIS.Framework.IDockableWindow)
                    {
                        try
                        {
                            // Try and refresh the project window.
                            ucProjectExplorer.AddinImpl winImpl = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID<ucProjectExplorer.AddinImpl>(ThisAddIn.IDs.ucProjectExplorer);
                            winImpl.UI.RefreshBaseMaps();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandling.frmException.HandleException(ex, "Error showing project explorer.", string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.frmException.HandleException(ex, "Error Showing RAVE Options Form", string.Empty);
            }

            ArcMap.Application.CurrentTool = null;
        }
    }
}