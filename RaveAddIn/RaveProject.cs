using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace RaveAddIn
{
    class RaveProject
    {
        public readonly FileInfo ProjectFile;

    }

    public static RaveProject Load(FileInfo projectFile)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(projectFile.FullName);
    }
}