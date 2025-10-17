namespace AuthoringToolBeta.Model;

public class HierarchyMediaFileModel: HierarchyModel
{
    public string Content { get; set; }

    public HierarchyMediaFileModel(string name, string path, string type, string content)
    {
        this.Name = name;
        this.Path = path;
        this.Type = type;
        this.Content = content;
    }
}