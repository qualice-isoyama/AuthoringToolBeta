using AuthoringToolBeta.Common;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace AuthoringToolBeta.ViewModels
{
    public class TrackViewModel : ReactiveObject
    {
        // このトラックが持つクリップのコレクション
        public ObservableCollection<ClipViewModel> Clips { get; } = new();
        private TimelineViewModel _parentViewModel;
        public TimelineViewModel ParentViewModel 
        {
            get => _parentViewModel; 
            set => this.RaiseAndSetIfChanged(ref _parentViewModel, value);
        }

        // 将来的にトラック名などを追加することも可能
        public string TrackName { get; set; }

        public TrackViewModel(string name, TimelineViewModel parentVM)
        {
            TrackName = name;
            ParentViewModel = parentVM;
        }
    }
}