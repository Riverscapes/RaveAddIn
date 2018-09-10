using System;

namespace RaveAddIn.AddInCommands
{
    public class btnCloseProject : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            try
            {
                ProjectManager.CloseCurrentProject();

                btnProjectExplorer.ShowProjectExplorer(false);
            }
            catch (Exception ex)
            {
                GCDCore.GCDException.HandleException(ex);
            }

            ArcMap.Application.CurrentTool = null;
        }

        protected override void OnUpdate()
        {
            Enabled = ProjectManager.Project != null;
        }
    }
}
