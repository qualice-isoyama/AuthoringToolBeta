using AuthoringToolBeta.Model;
using ReactiveUI;

namespace AuthoringToolBeta.ViewModels;

public class HierarchyItemViewModel: ReactiveObject
{
    private HierarchyModel _hModel;

    public HierarchyModel HModel
    {
        get => _hModel;
        set => this.RaiseAndSetIfChanged(ref _hModel, value);
    }

    public bool isDirectory()
    {
        return _hModel is HierarchyFolderModel? true: false;
    }
}