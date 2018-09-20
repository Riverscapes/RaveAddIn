using System;

namespace RaveAddIn.AddInCommands
{
    public class btnCloseProject : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            try
            {
                throw new NotImplementedException("Cannot close projects any more. Remove this button.");
                //ProjectManager.CloseCurrentProject();

                btnProjectExplorer.ShowProjectExplorer(false);
            }
            catch (Exception ex)
            {
                RaveException.HandleException(ex);
            }

            ArcMap.Application.CurrentTool = null;
        }

        protected override void OnUpdate()
        {
            Enabled = false;// ProjectManager.Project != null;
        }
    }
}
