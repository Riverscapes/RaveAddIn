
namespace RaveAddIn.ProjectTree
{
     abstract class BaseDataset
    {
        public string Name { get; private set; }
        public readonly int ImageIndex;

        public BaseDataset(string name, int index)
        {
            Name = name;
            ImageIndex = index;
        }
    }
}
