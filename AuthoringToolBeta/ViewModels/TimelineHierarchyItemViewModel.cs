using ReactiveUI;

namespace AuthoringToolBeta.ViewModels;

public class TimelineHierarchyItemViewModel: ReactiveObject
{
    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}