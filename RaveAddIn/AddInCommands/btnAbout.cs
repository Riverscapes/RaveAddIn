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
                RaveException.HandleException(ex);
            }

            ArcMap.Application.CurrentTool = null;
        }
    }
}