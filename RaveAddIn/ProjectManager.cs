using System;
using System.Collections.Generic;
using System.IO;

namespace RaveAddIn
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Base class for the GCD project manager. This should only contain
    /// members needed by both console and UI applications. The GCD still uses the inherited
    /// GCDProjectManager class that has more members that are specific to
    /// the desktop software.</remarks>
    public class ProjectManager
    {
        public const string RasterExtension = "tif";

        public static RaveProject Project { get; internal set; }

        // Simplifies the path combinations above
        public static DirectoryInfo CombinePaths(DirectoryInfo parentDir, string subDir) { return new DirectoryInfo(Path.Combine(parentDir.FullName, subDir)); }


        public static void CreateProject(RaveProject project)
        {
            project.Save();
            Project = project;
        }

        public static void OpenProject(FileInfo projectFile)
        {
            Project = RaveProject.Load(projectFile);
        }

        public static void RefreshProject()
        {
            Project = RaveProject.Load(Project.ProjectFile);
        }

        public static void OpenProject(RaveProject project)
        {
            Project = project;
        }

        public static void CloseCurrentProject()
        {
            Project = null;
            GC.Collect();
        } 
    }
}