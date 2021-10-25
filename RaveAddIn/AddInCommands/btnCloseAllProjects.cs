using System;

namespace RaveAddIn.AddInCommands
{
    public class btnCloseAllProjects : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
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
                    winImpl.UI.CloseAllProjects();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.frmException.HandleException(ex, "Error Closing RAVE Projects", string.Empty);
            }

            ArcMap.Application.CurrentTool = null;
        }
    }
}
