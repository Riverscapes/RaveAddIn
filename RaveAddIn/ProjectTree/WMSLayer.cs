
using System;

namespace RaveAddIn.ProjectTree
{
    class WMSLayer : BaseDataset, IGISLayer
    {
        public readonly string URL;

        public string GISPath { get { return URL; } }
        public string SymbologyKey { get { return string.Empty; } }

        public override bool Exists
        {
            get { return true; }
        }

        public WMSLayer(string name, string url, int imageIndex)
            : base(name, imageIndex, imageIndex)
        {
            URL = url;
        }
    }
}
