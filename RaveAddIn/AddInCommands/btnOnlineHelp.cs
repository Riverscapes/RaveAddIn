using System;

namespace RaveAddIn.AddInCommands
{
    public class btnOnlineHelp : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            try
            {
                System.Diagnostics.Process.Start(Properties.Resources.HelpBaseURL);
            }
            catch (Exception ex)
            {
                ErrorHandling.frmException.HandleException(ex, "Error Opening Help", string.Empty);
            }

            ArcMap.Application.CurrentTool = null;
        }
    }
}