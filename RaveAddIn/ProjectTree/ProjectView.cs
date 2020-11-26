using System;
using System.Collections.Generic;

namespace RaveAddIn.ProjectTree
{
public    class ProjectView
    {
        public readonly string Name;
        public readonly bool IsDefaultView;
        public readonly List<ProjectViewLayer> Layers;
            
        public ProjectView(string name, bool is_default)
        {
            Name = name;
            IsDefaultView = is_default;
            Layers = new List<ProjectViewLayer>();
        }
    }
}
