namespace AuthoringToolBeta.Model;

public class HierarchyMediaFileModel: HierarchyModel
{
    public string Content { get; set; }

    public HierarchyMediaFileModel(string name, string path, string type, string content , double duration)
    {
        Name = name;
        Path = path;
        Type = type;
        Content = content;
        Duration = duration;
    }
}