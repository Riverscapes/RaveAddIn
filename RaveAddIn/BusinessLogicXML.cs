using System;
using System.IO;

namespace RaveAddIn
{
    public class BusinessLogicXML
    {
        public readonly string DisplayName;
        public readonly string ProjectType;
        public readonly FileInfo XMLPath;

        public BusinessLogicXML(string name, string projectType, string filePath)
        {
            DisplayName = name;
            ProjectType = projectType;
            XMLPath = new FileInfo(filePath);
        }
    }
}
