
namespace RaveAddIn.ProjectTree
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
