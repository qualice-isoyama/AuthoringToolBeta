using AuthoringToolBeta.Model;
using ReactiveUI;

namespace AuthoringToolBeta.ViewModels;

public class HierarchyViewModel: ReactiveObject
{
    private HierarchyModel _hmodel;

    public HierarchyModel Hmodel
    {
        get => _hmodel; 
        set => this.RaiseAndSetIfChanged(ref _hmodel, value);
    }
}