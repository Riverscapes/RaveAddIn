using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.GISClient;
using System.IO;
using RaveAddIn.ProjectTree;

namespace RaveAddIn
{
    public struct ArcMapUtilities
    {


        public enum eEsriLayerTypes
        {
            Esri_DataLayer, //{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}
            Esri_GeoFeatureLayer, //{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}
            Esri_GraphicsLayer, //{34B2EF81-F4AC-11D1-A245-080009B6F22B}
            Esri_FDOGraphicsLayer, //{5CEAE408-4C0A-437F-9DB3-054D83919850}
            Esri_CoverageAnnotationLayer, //{0C22A4C7-DAFD-11D2-9F46-00C04F6BC78E}
            Esri_GroupLayer, //{EDAD6644-1810-11D1-86AE-0000F8751720}
            Esri_AnyLayer
        }

        public static ILayer AddToMap(FileSystemDataset dataset, string sLayerName, IGroupLayer pGroupLayer, FileInfo fiSymbologyLayerFile = null, bool bAddToMapIfPresent = false, short transparency = 0)
        {
            if (!dataset.Exists)
                return null;

            // Only add if it doesn't exist already
            ILayer pResultLayer = GetLayerBySource(dataset.Path);
            if ((pResultLayer is ILayer && string.Compare(pResultLayer.Name, sLayerName, true) == 0) && !bAddToMapIfPresent)
                return pResultLayer;

            // Confirm that the symbology layer file exists
            if (fiSymbologyLayerFile != null && !fiSymbologyLayerFile.Exists)
            {
                Exception ex = new Exception("A symbology layer file was provided, but the file does not exist");
                ex.Data["Data Source"] = dataset.Path.FullName;
                ex.Data["Layer file"] = fiSymbologyLayerFile.FullName;
                throw ex;
            }

            IWorkspace pWorkspace = GetWorkspace(dataset);
            switch (dataset.WorkspaceType)
            {
                case FileSystemDataset.GISDataStorageTypes.RasterFile:
                    IRasterDataset pRDS = ((IRasterWorkspace)pWorkspace).OpenRasterDataset(dataset.Name);
                    IRasterLayer pRLResult = new RasterLayer();
                    pRLResult.CreateFromDataset(pRDS);
                    pResultLayer = pRLResult;
                    break;

                case FileSystemDataset.GISDataStorageTypes.CAD:
                    string sFile = Path.GetFileName(Path.GetDirectoryName(dataset.Path.FullName));
                    string sFC = sFile + ":" + Path.GetFileName(dataset.Path.FullName);
                    IFeatureClass pFC = ((IFeatureWorkspace)pWorkspace).OpenFeatureClass(sFC);
                    pResultLayer = new FeatureLayer();
                    ((IFeatureLayer)pResultLayer).FeatureClass = pFC;
                    break;

                case FileSystemDataset.GISDataStorageTypes.ShapeFile:
                    IFeatureWorkspace pWS = (IFeatureWorkspace)ArcMapUtilities.GetWorkspace(dataset);
                    IFeatureClass pShapeFile = pWS.OpenFeatureClass(Path.GetFileNameWithoutExtension(dataset.Path.FullName));
                    pResultLayer = new FeatureLayer();
                    ((IFeatureLayer)pResultLayer).FeatureClass = pShapeFile;
                    break;

                case FileSystemDataset.GISDataStorageTypes.TIN:
                    ITin pTIN = ((ITinWorkspace)pWorkspace).OpenTin(System.IO.Path.GetFileName(dataset.Path.FullName));
                    pResultLayer = new TinLayer();
                    ((ITinLayer)pResultLayer).Dataset = pTIN;
                    pResultLayer.Name = dataset.Name;
                    break;

                case FileSystemDataset.GISDataStorageTypes.GeoPackage:
                    IFeatureWorkspace pGPKGWS = (IFeatureWorkspace)ArcMapUtilities.GetWorkspace(dataset);
                    IFeatureClass pGPKGFC = pGPKGWS.OpenFeatureClass(System.IO.Path.GetFileName(dataset.Path.FullName));
                    pResultLayer = new FeatureLayer();
                    ((IFeatureLayer)pResultLayer).FeatureClass = pGPKGFC;
                    break;

                default:
                    Exception ex = new Exception("Unhandled GIS dataset type");
                    ex.Data["FullPath Path"] = dataset.Path.FullName;
                    ex.Data["Storage Type"] = dataset.WorkspaceType.ToString();
                    throw ex;
            }

            if (!string.IsNullOrEmpty(sLayerName))
            {
                pResultLayer.Name = sLayerName;
            }

            try
            {
                ApplySymbology(pResultLayer, fiSymbologyLayerFile);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("symbology"))
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                    // DO Nothing
                }
                else
                {
                    throw;
                }
            }

            if (transparency > 0)
            {
                ILayerEffects pLayerEffects = (ILayerEffects)pResultLayer;
                pLayerEffects.Transparency = transparency;
            }

            if (pGroupLayer == null)
            {
                ((IMapLayers)ArcMap.Document.FocusMap).InsertLayer(pResultLayer, true, 0);
            }
            else
            {
                ((IMapLayers)ArcMap.Document.FocusMap).InsertLayerInGroup(pGroupLayer, pResultLayer, true, 0);
            }

            ArcMap.Document.UpdateContents();
            ArcMap.Document.ActiveView.Refresh();
            ArcMap.Document.CurrentContentsView.Refresh(null);

            return pResultLayer;
        }

        private static void ApplySymbology(ILayer layer, FileInfo symbology)
        {
            if (symbology == null || !symbology.Exists)
                return;

            IGxLayer pGXLayer = new GxLayer();
            IGxFile pGXFile = (IGxFile)pGXLayer;
            pGXFile.Path = symbology.FullName;


            if (layer is IRasterLayer)
            {
                if (pGXLayer.Layer is IRasterLayer)
                {
                    IRasterLayer prlayer = (IRasterLayer)pGXLayer.Layer;
                    ((IRasterLayer)layer).Renderer = prlayer.Renderer;
                }
                else
                {
                    throw new Exception(string.Format("Cannot apply symbology file to raster layer {0}. file: {1}", layer.Name, symbology.FullName));
                }
            }
            else if (layer is IGeoFeatureLayer)
            {
                if (pGXLayer.Layer is IGeoFeatureLayer)
                {
                    IGeoFeatureLayer pGFLayer = (IGeoFeatureLayer)pGXLayer.Layer;
                    ((IGeoFeatureLayer)layer).Renderer = pGFLayer.Renderer;

                    // Copy labels
                    if (pGFLayer.DisplayAnnotation)
                    {
                        try
                        {
                            ((IGeoFeatureLayer)layer).DisplayAnnotation = true;
                            ((IGeoFeatureLayer)layer).AnnotationProperties = pGFLayer.AnnotationProperties;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Print(ex.Message);
                            System.Diagnostics.Debug.Assert(false, "Error applying labels from layer file.");
                        }
                    }

                    // Make sure the correct layer properties dialog appears
                    IRendererPropertyPage pRendererPropPage = GetVectorPropertyPage(pGFLayer.Renderer);
                    if (pRendererPropPage != null)
                    {
                        ((IGeoFeatureLayer)layer).RendererPropertyPageClassID = pRendererPropPage.ClassID;
                    }
                }
            }
        }

        public static void AddWMSTopMap(string title, string wmsURL, IGroupLayer parentGrpLayer)
        {
            // WMS GetCapabilities URL
            IPropertySet propSet = new PropertySet();
            propSet.SetProperty("URL", wmsURL);

            IWMSGroupLayer wmsMapLayer = new WMSMapLayer() as IWMSGroupLayer;
            IWMSConnectionName connName = new WMSConnectionName();

            // set the property of the WMS layer to the Google Maps Engine
            connName.ConnectionProperties = propSet;

            // create a new data layer for the WMS layer to use
            IDataLayer dataLayer = wmsMapLayer as IDataLayer;
            dataLayer.Connect((IName)connName);

            // create a new ArcMap layer and add the WMS.  Set the name.
            IWMSServiceDescription serviceDesc = wmsMapLayer.WMSServiceDescription;
            ILayer layer = wmsMapLayer as ILayer;
            layer.Name = title;
            layer.Visible = true;
            //layer.Name = serviceDesc.WMSTitle;

            // add the WMS layer to the ArcMap session
            ((IMapLayers)ArcMap.Document.FocusMap).InsertLayerInGroup(parentGrpLayer, layer, true, 0);
            //((IMapLayers)ArcMap.Document.FocusMap).InsertLayer(layer, true, 0);

            if (wmsMapLayer is ICompositeLayer)
            {
                ChangeGroupLayerVisibility((ICompositeLayer)wmsMapLayer, true);
            }

            ArcMap.Document.UpdateContents();
            ArcMap.Document.ActiveView.Refresh();
            ArcMap.Document.CurrentContentsView.Refresh(null);
        }

        private static void ChangeGroupLayerVisibility(ICompositeLayer grpLayer, bool visible)
        {
            for (int i = 0; i < grpLayer.Count; i++)
            {
                grpLayer.Layer[i].Visible = visible;
                if (grpLayer.Layer[i] is ICompositeLayer)
                {
                    ChangeGroupLayerVisibility((ICompositeLayer)grpLayer.Layer[i], visible);
                }
            }
        }

        private static IRendererPropertyPage GetVectorPropertyPage(IFeatureRenderer renderer)
        {
            if (renderer is ClassBreaksRenderer) return (IRendererPropertyPage)new GraduatedColorPropertyPage();
            else if (renderer is SimpleRenderer) return (IRendererPropertyPage)new SingleSymbolPropertyPage();
            else if (renderer is UniqueValueRenderer) return (IRendererPropertyPage)new StackedChartPropertyPage();
            return null;

            //if (renderer is BarChartPropertyPage) return (IRendererPropertyPage)new BarChartPropertyPage();
            //else if (renderer is BiUniqueValuePropertyPage) return (IRendererPropertyPage)new BiUniqueValuePropertyPage();
            //else if (renderer is CadUniqueValuePropertyPage) return (IRendererPropertyPage)new CadUniqueValuePropertyPage();
            //else if (renderer is CombiUniqueValuePropertyPage) return (IRendererPropertyPage)new CombiUniqueValuePropertyPage();
            //else if (renderer is StackedChartPropertyPage) return (IRendererPropertyPage)new StackedChartPropertyPage();
            //else if (renderer is GraduatedSymbolPropertyPage) return (IRendererPropertyPage)new GraduatedSymbolPropertyPage();
            //else if (renderer is LookupSymbolPropertyPage) return (IRendererPropertyPage)new LookupSymbolPropertyPage();
            //else if (renderer is MultiDotDensityPropertyPage) return (IRendererPropertyPage)new MultiDotDensityPropertyPage();
            //else if (renderer is PieChartPropertyPage) return (IRendererPropertyPage)new PieChartPropertyPage();
            //else if (renderer is ProportionalSymbolPropertyPage) return (IRendererPropertyPage)new ProportionalSymbolPropertyPage();
        }

        public static ILayer GetLayerBySource(FileSystemInfo fiFullPath)
        {
            if (!fiFullPath.Exists)
                return null;

            IMapLayers mapLayers = (IMapLayers)ArcMap.Document.FocusMap;
            UID pID = new UIDClass();
            pID.Value = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}"; // eEsriLayerTypes.Esri_DataLayer
            IEnumLayer pEnumLayer = ((IMapLayers)ArcMap.Document.FocusMap).Layers[pID, true];
            ILayer pLayer = pEnumLayer.Next();
            while (pLayer != null)
            {
                if (pLayer is IGeoFeatureLayer)
                {
                    IGeoFeatureLayer pGFL = (IGeoFeatureLayer)pLayer;
                    string sPath = ((IDataset)pGFL).Workspace.PathName;
                    if (pGFL.FeatureClass.FeatureDataset is IFeatureDataset)
                    {
                        sPath = Path.Combine(sPath, pGFL.FeatureClass.FeatureDataset.Name);
                    }

                    sPath = Path.Combine(sPath, ((IDataset)pGFL.FeatureClass).Name);

                    if (((IDataset)pGFL.FeatureClass).Workspace.Type == esriWorkspaceType.esriFileSystemWorkspace)
                    {
                        sPath = Path.ChangeExtension(sPath, "shp");
                    }

                    if (string.Compare(fiFullPath.FullName, sPath, true) == 0)
                    {
                        return pLayer;
                    }
                }
                else if (pLayer is IRasterLayer)
                {
                    if (string.Compare(((IRasterLayer)pLayer).FilePath, fiFullPath.FullName, true) == 0)
                    {
                        return pLayer;
                    }
                }

                pLayer = pEnumLayer.Next();
            }

            return null;
        }

        /// <summary>
        /// Gets all layers from ArcMap ToC that possess the specified name (case insensitive) and optionally of specified type 
        /// </summary>
        /// <param name="sLayerName">Name of the layer</param>
        /// <param name="pArcMap">ArcMap</param>
        /// <param name="eType">Optional constraint to look for a layer of a certain type. Pass Nothing to look for any type.</param>
        /// <returns>ILayer if found, otherwise nothing</returns>
        /// <remarks>Code taken from EDN on Jul 10 2007. Retrieves all layers from the current focus map that
        /// have the type specified by eType. Note that this code was enhanced from the copy taken off
        /// the internet. The method pMap.Layers() throws an exception when there are no layers in the map.
        /// 
        /// PGB - 27 - Jul 2007 - For some reason, the Layers() call throws an exception when it is called for
        /// a group layer and there are feature layers in the legend, but not group layers. It is
        /// commented out for now.</remarks>
        public static ILayer GetLayerByName(string sLayerName, eEsriLayerTypes eType)
        {
            if (string.IsNullOrEmpty(sLayerName))
                return null;

            if (ArcMap.Document.FocusMap.LayerCount < 1)
                return null;

            UID pID = new UIDClass();
            switch (eType)
            {
                case eEsriLayerTypes.Esri_DataLayer:
                    pID.Value = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}";
                    break;
                case eEsriLayerTypes.Esri_GeoFeatureLayer:
                    pID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                    break;
                case eEsriLayerTypes.Esri_GraphicsLayer:
                    pID.Value = "{34B2EF81-F4AC-11D1-A245-080009B6F22B}";
                    break;
                case eEsriLayerTypes.Esri_FDOGraphicsLayer:
                    pID.Value = "{5CEAE408-4C0A-437F-9DB3-054D83919850}";
                    break;
                case eEsriLayerTypes.Esri_CoverageAnnotationLayer:
                    pID.Value = "{0C22A4C7-DAFD-11D2-9F46-00C04F6BC78E}";
                    break;
                case eEsriLayerTypes.Esri_GroupLayer:
                    pID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";
                    break;
                default:
                    pID = null;
                    break;
            }

            IEnumLayer pEnumLayer = ArcMap.Document.FocusMap.Layers[pID, true];
            ILayer pLayer = pEnumLayer.Next();
            while (pLayer != null)
            {
                if (string.Compare(pLayer.Name, sLayerName, true) == 0)
                    return pLayer;

                pLayer = pEnumLayer.Next();
            }

            return null;
        }

        public static IGroupLayer GetGroupLayer(string sName, IGroupLayer pParentGroupLayer, ucProjectExplorer.NodeInsertModes topLevelMode, bool bCreateIfNeeded = true)
        {
            if (string.IsNullOrEmpty(sName))
            {
                // This route might be needed if the GCD calls this function without an open project.
                return null;
            }

            if (pParentGroupLayer == null)
            {
                UID pId = new UID();
                pId.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";

                IEnumLayer pEnum = ((IMapLayers)ArcMap.Document.FocusMap).Layers[pId, false];
                ILayer pLayer = pEnum.Next();
                while (pLayer != null)
                {
                    if (string.Compare(pLayer.Name, sName, true) == 0)
                        return (IGroupLayer)pLayer;

                    pLayer = pEnum.Next();
                }
            }
            else
            {
                // Try and find the group layer already in the hierarchy
                ICompositeLayer pCompositeLayer = (ICompositeLayer)pParentGroupLayer;
                for (int i = 0; i <= pCompositeLayer.Count - 1; i++)
                {
                    if (string.Compare(pCompositeLayer.Layer[i].Name, sName, true) == 0)
                    {
                        return (IGroupLayer)pCompositeLayer.Layer[i];
                    }
                }
            }

            IGroupLayer pResultLayer = new GroupLayer();
            pResultLayer.Name = sName;

            if (pParentGroupLayer != null)
            {
                ((IMapLayers)ArcMap.Document.FocusMap).InsertLayerInGroup(pParentGroupLayer, pResultLayer, true, 0);
            }
            else
            {
                if (topLevelMode == ucProjectExplorer.NodeInsertModes.Add)
                    ((IMapLayers)ArcMap.Document.FocusMap).InsertLayer(pResultLayer, false, ArcMap.Document.FocusMap.LayerCount + 1);

                else
                {
                    ((IMapLayers)ArcMap.Document.FocusMap).InsertLayer(pResultLayer, true, 0);
                }
            }

            return pResultLayer;
        }

        /// <summary>
        /// Create a singleton for a workspace factory
        /// </summary>
        /// <param name="eGISStorageType"></param>
        /// <returns>CALLER IS RESPONSIBLE FOR RELEASING RETURNED COM OBJECT</returns>
        /// <remarks>This is the only correct method for creating a workspace factory. Do not call "New" to create this singleton classes.
        /// http://forums.esri.com/Thread.asp?c=93&f=993&t=178686
        /// </remarks>
        private static IWorkspaceFactory GetWorkspaceFactory(FileSystemDataset.GISDataStorageTypes eGISStorageType)
        {
            Type aType = null;
            IWorkspaceFactory pWSFact = null;

            try
            {
                switch (eGISStorageType)
                {
                    case FileSystemDataset.GISDataStorageTypes.RasterFile:
                        aType = Type.GetTypeFromProgID("esriDataSourcesRaster.RasterWorkspaceFactory");
                        break;
                    case FileSystemDataset.GISDataStorageTypes.ShapeFile:
                        aType = Type.GetTypeFromProgID("esriDataSourcesFile.ShapefileWorkspaceFactory");
                        break;
                    case FileSystemDataset.GISDataStorageTypes.FileGeodatase:
                        aType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                        break;
                    case FileSystemDataset.GISDataStorageTypes.CAD:
                        aType = Type.GetTypeFromProgID("esriDataSourcesFile.CadWorkspaceFactory");
                        break;
                    case FileSystemDataset.GISDataStorageTypes.PersonalGeodatabase:
                        aType = Type.GetTypeFromProgID("esriDataSourcesGDB.AccessWorkspaceFactory");
                        break;
                    case FileSystemDataset.GISDataStorageTypes.TIN:
                        aType = Type.GetTypeFromProgID("esriDataSourcesFile.TinWorkspaceFactory");
                        break;
                    case FileSystemDataset.GISDataStorageTypes.GeoPackage:
                        aType = Type.GetTypeFromProgID("esriDataSourcesGDB.SqlWorkspaceFactory");
                        break;
                    default:
                        throw new Exception("Unhandled GIS storage type");
                }

                pWSFact = (IWorkspaceFactory)Activator.CreateInstance(aType);
            }
            catch (Exception ex)
            {
                Exception ex2 = new Exception("Error getting workspace factory", ex);
                ex2.Data["Workspace Type"] = eGISStorageType.ToString();
                throw ex2;
            }

            // CALLER IS RESPONSIBLE FOR RELEASING RETURNED COM OBJECT
            return pWSFact;
        }

        /// <summary>
        /// Open a file-based workspace (Raster or ShapeFile)
        /// </summary>
        /// <param name="fiFullPath">Workspace directory or file path</param>
        /// <returns>CALLER IS RESPONSIBLE FOR RELEASING RETURNED COM OBJECT</returns>
        public static IWorkspace GetWorkspace(FileSystemDataset data)
        {
            IWorkspaceFactory pWSFact = GetWorkspaceFactory(data.WorkspaceType);
            IWorkspace pWS = pWSFact.OpenFromFile(data.WorkspacePath.FullName, ArcMap.Application.hWnd);

            // Must release the workspace factory object
            int refsLeft = 0;
            do
            {
                refsLeft = System.Runtime.InteropServices.Marshal.ReleaseComObject(pWSFact);
            }
            while (refsLeft > 0);

            // CALLER IS RESPONSIBLE FOR RELEASING RETURNED COM OBJECT
            return pWS;
        }

        public static void RemoveLayer(FileSystemInfo layerPath)
        {
            ILayer pLayer = GetLayerBySource(layerPath);

            while (pLayer is ILayer)
            {
                IGroupLayer pParent = GetParentGroupLayer(pLayer);

                if (pLayer is IDataLayer2)
                {
                    ((IDataLayer2)pLayer).Disconnect();
                }

                ArcMap.Document.FocusMap.DeleteLayer(pLayer);
                ArcMap.Document.UpdateContents();

                // Remove empty group layers from ToC
                while (pParent is IGroupLayer)
                {
                    ILayer pNextParent = GetParentGroupLayer(pParent);
                    ICompositeLayer pComp = (ICompositeLayer)pParent;
                    if (pComp.Count < 1)
                    {
                        ArcMap.Document.FocusMap.DeleteLayer(pParent);
                        ArcMap.Document.UpdateContents();
                    }

                    if (pNextParent is IGroupLayer)
                        pParent = (IGroupLayer)pNextParent;
                    else
                        pParent = null;
                }

                ArcMap.Document.ActiveView.Refresh();

                // Release all references to the layer to prevent locks on the underlying data source
                // http://edndoc.esri.com/arcobjects/9.2/net/fe9f7423-2100-4c70-8bd6-f4f16d5ce8c0.htm
                int refsLeft = 0;
                do
                {
                    refsLeft = System.Runtime.InteropServices.Marshal.ReleaseComObject(pLayer);
                }
                while (refsLeft > 0);
                pLayer = null;
                GC.Collect();
                pLayer = GetLayerBySource(layerPath);
            }
        }

        private static IGroupLayer GetParentGroupLayer(ILayer pLayer)
        {
            //Loop over all group layers and see if the specified layer is inside
            IMap pMap = ArcMap.Document.FocusMap;
            UID pUID = new UID();
            pUID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";

            IEnumLayer pEnum = ArcMap.Document.FocusMap.Layers[pUID, true];
            ICompositeLayer pGroupLayer = (ICompositeLayer)pEnum.Next();
            while (pGroupLayer is ICompositeLayer)
            {
                for (int i = 0; i < pGroupLayer.Count; i++)
                {
                    if (pGroupLayer.Layer[i].Equals(pLayer))
                    {
                        return (IGroupLayer)pGroupLayer;
                    }
                }
                pGroupLayer = (ICompositeLayer)pEnum.Next();
            }

            return null;
        }

        //public void RemoveLayersfromTOC(string directory)
        //{
        //    //IMxDocument mxMap = (IMxDocument)application.Document;
        //    //IMap pMap = mxMap.FocusMap;
        //    //IMapLayers pMapLayers = pMap;

        //    for (int i = 0; i <= ArcMap.Document.FocusMap.LayerCount - 1; i++)
        //    {
        //        ILayer player = ArcMap.Document.FocusMap.Layer[i];
        //        if (player is IGroupLayer)
        //        {
        //            RemoveLayersfromGroupLayer((IGroupLayer)player, directory);
        //        }
        //        else
        //        {
        //            IDataset pDS = player;
        //            try
        //            {
        //                if (LCase(directory) == LCase(pDS.Workspace.PathName))
        //                {
        //                    pMap.DeleteLayer(player);
        //                }
        //            }
        //            }

        //        if (player != null)
        //        {
        //            System.Runtime.InteropServices.Marshal.ReleaseComObject(player);
        //            player = null;
        //        }
        //    }

        //    mxMap.UpdateContents();
        //    mxMap.ActiveView.Refresh();
        //    ESRI.ArcGIS.ArcMapUI.IContentsView pContentsView = mxMap.CurrentContentsView;
        //    pContentsView.Refresh(null);
        //    if (mxMap != null)
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(mxMap);
        //        mxMap = null;
        //    }

        //    if (pContentsView != null)
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(pContentsView);
        //        pContentsView = null;
        //    }
        //}

        //public void RemoveLayersfromGroupLayer(IGroupLayer pGroupLayer, string directory)
        //{
        //    ILayer pLayer;
        //    List<ILayer> LayersToDelete = new List<ILayer>();
        //    ICompositeLayer pCompositeLayer = (ICompositeLayer)pGroupLayer;
        //    for (int i = 1; i <= pCompositeLayer.Count; i++)
        //    {
        //        pLayer = pCompositeLayer.Layer[i - 1];
        //        if (pLayer is IGroupLayer)
        //        {
        //            RemoveLayersfromGroupLayer(pLayer, directory);
        //        }
        //        else
        //        {
        //            try
        //            {
        //                IDataset pDS = (IDataset)pLayer;
        //                string LayerDirectoryname = pDS.Workspace.PathName.ToLower();
        //                if (LayerDirectoryname.EndsWith(IO.Path.DirectorySeparatorChar))
        //                {
        //                    LayerDirectoryname = LayerDirectoryname.Substring(0, LayerDirectoryname.Length - 1);
        //                }

        //                if (LCase(directory) == LayerDirectoryname)
        //                {
        //                    LayersToDelete.Add(pLayer);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Debug.WriteLine(ex.Message);
        //            }
        //        }
        //    }

        //    foreach (ILayer pDeleteLayer in LayersToDelete)
        //    {
        //        pGroupLayer.Delete(pDeleteLayer);
        //        if (pDeleteLayer != null)
        //        {
        //            System.Runtime.InteropServices.Marshal.ReleaseComObject(pDeleteLayer);
        //            pDeleteLayer = null;
        //        }
        //    }

        //    if (pGroupLayer != null)
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(pGroupLayer);
        //        pGroupLayer = null;
        //    }
        //}

        //public void RemoveGroupLayer(string sGroupLayerName)
        //{
        //    IMap pMap = ArcMap.Document.FocusMap;
        //    UID pUID = new UID();
        //    pUID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";

        //    IEnumLayer pEnum = ArcMap.Document.FocusMap.Layers[pUID, true];
        //    ILayer pL = pEnum.Next();
        //    while (pL is ILayer)
        //    {
        //        if (string.Compare(sGroupLayerName, pL.Name, true) == 0)
        //        {
        //            pMap.DeleteLayer(pL);
        //        }

        //        pL = pEnum.Next();
        //    }
        //}

    }
}
