﻿using System.IO;

namespace RaveAddIn.ProjectTree
{
    class TIN : GISDataset, IGISLayer
    {
        public TIN(RaveProject project, string name, string path, short transparency)
            : base(project, name, new DirectoryInfo(path), string.Empty, transparency, Directory.Exists(path) ? 6: 7)
        {

        }
    }
}
