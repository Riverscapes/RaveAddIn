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
        public readonly FileInfo FilePath;

        public GISItem(FileInfo filePath)
        {
            FilePath = filePath;
        }
    }
}
