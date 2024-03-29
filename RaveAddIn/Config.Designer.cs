//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RaveAddIn {
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Editor;
    using ESRI.ArcGIS.esriSystem;
    using System;
    using System.Collections.Generic;
    using ESRI.ArcGIS.Desktop.AddIns;
    
    
    /// <summary>
    /// A class for looking up declarative information in the associated configuration xml file (.esriaddinx).
    /// </summary>
    internal static class ThisAddIn {
        
        internal static string Name {
            get {
                return "RAVE";
            }
        }
        
        internal static string AddInID {
            get {
                return "{67c9bba1-7371-430f-b4ca-6b2545de71df}";
            }
        }
        
        internal static string Company {
            get {
                return "North Arrow Research Ltd.";
            }
        }
        
        internal static string Version {
            get {
                return "3.2.0";
            }
        }
        
        internal static string Description {
            get {
                return "Riverscapes River Analyst And Explorer AddIn";
            }
        }
        
        internal static string Author {
            get {
                return "Philip Bailey";
            }
        }
        
        internal static string Date {
            get {
                return "10/06/2022";
            }
        }
        
        internal static ESRI.ArcGIS.esriSystem.UID ToUID(this System.String id) {
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = id;
            return uid;
        }
        
        /// <summary>
        /// A class for looking up Add-in id strings declared in the associated configuration xml file (.esriaddinx).
        /// </summary>
        internal class IDs {
            
            /// <summary>
            /// Returns 'RAVE_AddIn_ToolbarExplorerButton', the id declared for Add-in Button class 'RaveAddIn.AddInCommands.btnProjectExplorer'
            /// </summary>
            internal static string RaveAddIn_AddInCommands_btnProjectExplorer {
                get {
                    return "RAVE_AddIn_ToolbarExplorerButton";
                }
            }
            
            /// <summary>
            /// Returns 'RAVE_AddIn_OpenProjectButton', the id declared for Add-in Button class 'RaveAddIn.AddInCommands.btnOpenProject'
            /// </summary>
            internal static string RaveAddIn_AddInCommands_btnOpenProject {
                get {
                    return "RAVE_AddIn_OpenProjectButton";
                }
            }
            
            /// <summary>
            /// Returns 'RAVE_AddIn_CloseAllProjectsButton', the id declared for Add-in Button class 'RaveAddIn.AddInCommands.btnCloseAllProjects'
            /// </summary>
            internal static string RaveAddIn_AddInCommands_btnCloseAllProjects {
                get {
                    return "RAVE_AddIn_CloseAllProjectsButton";
                }
            }
            
            /// <summary>
            /// Returns 'RAVE_AddIn_OptionsButton', the id declared for Add-in Button class 'RaveAddIn.AddInCommands.btnOptions'
            /// </summary>
            internal static string RaveAddIn_AddInCommands_btnOptions {
                get {
                    return "RAVE_AddIn_OptionsButton";
                }
            }
            
            /// <summary>
            /// Returns 'RAVE_AddIn_UpdateResourcesButton', the id declared for Add-in Button class 'RaveAddIn.AddInCommands.btnUpdateResources'
            /// </summary>
            internal static string RaveAddIn_AddInCommands_btnUpdateResources {
                get {
                    return "RAVE_AddIn_UpdateResourcesButton";
                }
            }
            
            /// <summary>
            /// Returns 'RAVE_AddIn_FindResourcesFolderButton', the id declared for Add-in Button class 'RaveAddIn.AddInCommands.btnFindResourcesFolder'
            /// </summary>
            internal static string RaveAddIn_AddInCommands_btnFindResourcesFolder {
                get {
                    return "RAVE_AddIn_FindResourcesFolderButton";
                }
            }
            
            /// <summary>
            /// Returns 'RAVE_AddIn_AboutButton', the id declared for Add-in Button class 'RaveAddIn.AddInCommands.btnAbout'
            /// </summary>
            internal static string RaveAddIn_AddInCommands_btnAbout {
                get {
                    return "RAVE_AddIn_AboutButton";
                }
            }
            
            /// <summary>
            /// Returns 'RAVE_AddIn_WebSite', the id declared for Add-in Button class 'RaveAddIn.AddInCommands.btnWebSite'
            /// </summary>
            internal static string RaveAddIn_AddInCommands_btnWebSite {
                get {
                    return "RAVE_AddIn_WebSite";
                }
            }
            
            /// <summary>
            /// Returns 'North_Arrow_Research_RaveAddIn_RaveExtension', the id declared for Add-in Extension class 'RaveExtension'
            /// </summary>
            internal static string RaveExtension {
                get {
                    return "North_Arrow_Research_RaveAddIn_RaveExtension";
                }
            }
            
            /// <summary>
            /// Returns 'RAVE_AddIn_ucProjectExplorer', the id declared for Add-in DockableWindow class 'ucProjectExplorer+AddinImpl'
            /// </summary>
            internal static string ucProjectExplorer {
                get {
                    return "RAVE_AddIn_ucProjectExplorer";
                }
            }
        }
    }
    
internal static class ArcMap
{
  private static IApplication s_app = null;
  private static IDocumentEvents_Event s_docEvent;

  public static IApplication Application
  {
    get
    {
      if (s_app == null)
      {
        s_app = Internal.AddInStartupObject.GetHook<IMxApplication>() as IApplication;
        if (s_app == null)
        {
          IEditor editorHost = Internal.AddInStartupObject.GetHook<IEditor>();
          if (editorHost != null)
            s_app = editorHost.Parent;
        }
      }
      return s_app;
    }
  }

  public static IMxDocument Document
  {
    get
    {
      if (Application != null)
        return Application.Document as IMxDocument;

      return null;
    }
  }
  public static IMxApplication ThisApplication
  {
    get { return Application as IMxApplication; }
  }
  public static IDockableWindowManager DockableWindowManager
  {
    get { return Application as IDockableWindowManager; }
  }
  public static IDocumentEvents_Event Events
  {
    get
    {
      s_docEvent = Document as IDocumentEvents_Event;
      return s_docEvent;
    }
  }
  public static IEditor Editor
  {
    get
    {
      UID editorUID = new UID();
      editorUID.Value = "esriEditor.Editor";
      return Application.FindExtensionByCLSID(editorUID) as IEditor;
    }
  }
}

namespace Internal
{
  [StartupObjectAttribute()]
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  public sealed partial class AddInStartupObject : AddInEntryPoint
  {
    private static AddInStartupObject _sAddInHostManager;
    private List<object> m_addinHooks = null;

    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    public AddInStartupObject()
    {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool Initialize(object hook)
    {
      bool createSingleton = _sAddInHostManager == null;
      if (createSingleton)
      {
        _sAddInHostManager = this;
        m_addinHooks = new List<object>();
        m_addinHooks.Add(hook);
      }
      else if (!_sAddInHostManager.m_addinHooks.Contains(hook))
        _sAddInHostManager.m_addinHooks.Add(hook);

      return createSingleton;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void Shutdown()
    {
      _sAddInHostManager = null;
      m_addinHooks = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal static T GetHook<T>() where T : class
    {
      if (_sAddInHostManager != null)
      {
        foreach (object o in _sAddInHostManager.m_addinHooks)
        {
          if (o is T)
            return o as T;
        }
      }

      return null;
    }

    // Expose this instance of Add-in class externally
    public static AddInStartupObject GetThis()
    {
      return _sAddInHostManager;
    }
  }
}
}
