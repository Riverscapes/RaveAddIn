using System;

namespace RaveAddIn.AddInCommands
{
    public class btnAbout : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            try
            {
                frmAbout frm = new frmAbout();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorHandling.frmException.HandleException(ex, "Error Showing RAVE About Form", string.Empty);
            }

            ArcMap.Application.CurrentTool = null;
        }
    }
}