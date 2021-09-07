using System;
using System.Collections.Generic;

namespace RaveAddIn.ProjectTree
{
public    class ProjectView
    {
        public readonly string Id;
        public readonly string Name;
        public readonly bool IsDefaultView;
        public readonly List<ProjectViewLayer> Layers;
            
        public ProjectView(string id, string name, bool is_default)
        {
            Id = id;
            Name = name;
            IsDefaultView = is_default;
            Layers = new List<ProjectViewLayer>();
        }
    }
}
