
namespace RaveAddIn.MetaData
{
    public class MetaDataItem
    {
        public string Key { get; internal set; }
        public string Value { get; internal set; }

        public MetaDataItem()
        {
        }

        public MetaDataItem(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
