using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaveAddIn
{
   public class WMSLayer
    {
        public readonly string Name;
        public readonly string URL;

        public WMSLayer(string name, string url)
        {
            Name = name;
            URL = url;
        }
    }
}
