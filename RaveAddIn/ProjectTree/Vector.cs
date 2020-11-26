﻿using System.IO;

namespace RaveAddIn.ProjectTree
{
    class Vector : GISDataset, IGISLayer
    {
        public Vector(RaveProject project, string name, string path, string symbology, short transparency, string id)
            : base(project, name, new FileInfo(path), symbology, transparency, 3, 5, id)
        {

        }
    }
}
