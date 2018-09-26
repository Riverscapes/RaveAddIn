using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace RaveAddIn
{
    public class RaveExtension : ESRI.ArcGIS.Desktop.AddIns.Extension
    {
        public RaveExtension()
        {
        }

        protected override void OnStartup()
        {
            //
            // TODO: Uncomment to start listening to document events
            //
            // WireDocumentEvents();
        }

        private void WireDocumentEvents()
        {
            //
            // TODO: Sample document event wiring code. Change as needed
            //

            // Named event handler
            ArcMap.Events.NewDocument += delegate () { ArcMap_NewDocument(); };

            // Anonymous event handler
            ArcMap.Events.BeforeCloseDocument += delegate ()
            {
                // Return true to stop document from closing
                ESRI.ArcGIS.Framework.IMessageDialog msgBox = new ESRI.ArcGIS.Framework.MessageDialogClass();
                return msgBox.DoModal("BeforeCloseDocument Event", "Abort closing?", "Yes", "No", ArcMap.Application.hWnd);
            };
        }

        void ArcMap_NewDocument()
        {
            // TODO: Handle new document event
        }
    }
}
