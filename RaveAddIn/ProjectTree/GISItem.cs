using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaveAddIn.ProjectTree
{
    public class GISItem
    {
        public readonly string Name;
        public readonly FileInfo GISFileInfo;

        public GISItem(FileInfo filePath, string name)
        {
            GISFileInfo = filePath;
            Name = name;
        }
    }
}
