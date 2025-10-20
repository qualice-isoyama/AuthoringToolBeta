using System.Collections.ObjectModel;
using AuthoringToolBeta.Model;


namespace AuthoringToolBeta.ViewModels;

public class HierarchyViewModel
{
    public ObservableCollection<HierarchyItemViewModel> Hierarchy { get; }

    public void exchangePosHierarchyItem(HierarchyItemViewModel exchangeFromHM, HierarchyItemViewModel exchangeToHM)
    {
        int exchangeFromIdx = Hierarchy.IndexOf(exchangeFromHM);
        int exchangeToIdx = Hierarchy.IndexOf(exchangeToHM);
        Hierarchy[exchangeFromIdx] = exchangeToHM;
        Hierarchy[exchangeToIdx] = exchangeFromHM;
        
    }
}