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
               RaveException.HandleException(ex);
            }

            ArcMap.Application.CurrentTool = null;
        }
    }
}