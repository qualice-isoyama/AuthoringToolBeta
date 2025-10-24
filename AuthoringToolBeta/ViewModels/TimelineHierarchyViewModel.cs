using System.Collections.ObjectModel;
using AuthoringToolBeta.Model;

namespace AuthoringToolBeta.ViewModels;

public class TimelineHierarchyViewModel
{
    public ObservableCollection<TimelineHierarchyItemViewModel> timelineHierarchy { get; set; }

    public TimelineHierarchyViewModel()
    {
        timelineHierarchy = new ObservableCollection<TimelineHierarchyItemViewModel>
        {
            new TimelineHierarchyItemViewModel() { Name = "test01" },
            new TimelineHierarchyItemViewModel()  { Name = "test02" },
            new TimelineHierarchyItemViewModel()  { Name = "test03" },
            new TimelineHierarchyItemViewModel()  { Name = "test04" },
            new TimelineHierarchyItemViewModel()  { Name = "test05" }
        };
    }
}