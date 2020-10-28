
namespace RaveAddIn.ProjectTree
{
    public abstract class BaseDataset
    {
        public string Name { get; private set; }
        
        private readonly int ImageIndex_Exists;
        private readonly int Image_index_Missing;

        public abstract bool Exists { get; }

        public int ImageIndex {  get { return Exists ? ImageIndex_Exists : Image_index_Missing; } }

        public BaseDataset(string name, int index_exists, int index_missing)
        {
            Name = name;
            ImageIndex_Exists = index_exists;
            Image_index_Missing = index_missing;
        }
    }
}
