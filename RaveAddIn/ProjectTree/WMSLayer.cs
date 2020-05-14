
namespace RaveAddIn.ProjectTree
{
    class WMSLayer : BaseDataset, IGISLayer
    {
        public readonly string URL;

        public string GISPath {  get { return URL; } }
        public string SymbologyKey { get { return string.Empty; } }

        public WMSLayer(string name, string url, int imageIndex)
            : base(name, imageIndex)
        {
            URL = url;
        }
    }
}
