using System.Collections.ObjectModel;
using AuthoringToolBeta.Model;

namespace AuthoringToolBeta.ViewModels;

public class TimelineHierarchyViewModel
{
    public ObservableCollection<TimelineHierarchyModel> timelineHierarchy { get; set; }

    public TimelineHierarchyViewModel()
    {
        timelineHierarchy = new ObservableCollection<TimelineHierarchyModel>
        {
            new TimelineHierarchyModel() { Name = "test01" },
            new TimelineHierarchyModel() { Name = "test02" },
            new TimelineHierarchyModel() { Name = "test03" },
            new TimelineHierarchyModel() { Name = "test04" },
            new TimelineHierarchyModel() { Name = "test05" }
        };
    }
}