

namespace RaveAddIn.ProjectTree
{
    public class GroupLayer
    {
        public readonly string Label;
        public readonly bool Collapse;
        public readonly string Id;

        public GroupLayer(string label, bool collapse, string id)
        {
            Label = label;
            Collapse = collapse;
            Id = id;
        }
    }
}
