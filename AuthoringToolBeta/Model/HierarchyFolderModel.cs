using System.Collections.Generic;

namespace AuthoringToolBeta.Model;

public class HierarchyFolderModel: HierarchyModel
{
    public List<HierarchyModel> Children { get; set; }

    public HierarchyFolderModel(string name, string path, string type, List<HierarchyModel> children)
    {
        Name = name;
        Path = path;
        Type = type;
        Children = children;
    }
}