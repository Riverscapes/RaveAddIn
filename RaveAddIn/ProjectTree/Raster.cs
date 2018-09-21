using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaveAddIn.ProjectTree
{
    public class Raster : GISItem
    {
        public Raster(FileInfo filepath, string name) : base(filepath, name)
        {

        }
    }
}
