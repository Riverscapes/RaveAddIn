using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace RaveAddIn
{
    public class RaveExtension : ESRI.ArcGIS.Desktop.AddIns.Extension
    {
        public readonly Dictionary<string, List<BusinessLogicXML>> BusinessLogicXML;

        public RaveExtension()
        {
            BusinessLogicXML = new Dictionary<string, List<BusinessLogicXML>>();
        }

        protected override void OnStartup()
        {
            //
            // TODO: Uncomment to start listening to document events
            //
            // WireDocumentEvents();

            LoadBusinessLogicXML();
        }

        private void LoadBusinessLogicXML()
        {
            string folder = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "XML");

            foreach (string xmlPath in Directory.GetFiles(folder, "*.xml"))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlPath);

                    XmlNode nodProjectType = xmlDoc.SelectSingleNode("Project/ProjectType");
                    XmlNode nodName = xmlDoc.SelectSingleNode("Project/Name");

                    if (nodProjectType is XmlNode && !string.IsNullOrEmpty(nodProjectType.InnerText))
                    {
                        if (!BusinessLogicXML.ContainsKey(nodProjectType.InnerText))
                        {
                            BusinessLogicXML[nodProjectType.InnerText] = new List<BusinessLogicXML>();
                        }

                        BusinessLogicXML[nodProjectType.InnerText].Add(new BusinessLogicXML(nodName.InnerText, nodProjectType.InnerText, xmlPath));
                    }
                }
                catch (Exception)
                {
                    // Do nothing, continue onto the next business logic file
                }
            }
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
