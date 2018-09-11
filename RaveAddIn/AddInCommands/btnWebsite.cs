using System;

namespace RaveAddIn.AddInCommands
{
    public class btnWebSite : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            try
            {
                System.Diagnostics.Process.Start(Properties.Resources.WebSiteURL);
            }
            catch (Exception ex)
            {
                RaveException.HandleException(ex);
            }

            ArcMap.Application.CurrentTool = null;
        }
    }
}