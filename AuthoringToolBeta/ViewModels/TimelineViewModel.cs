using System.Collections.Frozen;
using AuthoringToolBeta.Model;
using AuthoringToolBeta.ViewModels.Base;
using System.Collections.ObjectModel;
using Avalonia;

namespace AuthoringToolBeta.ViewModels
{
    public class TimelineViewModel: ViewModelBase
    {
        // クリップのViewModelを保持するコレクション
        public ObservableCollection<ClipViewModel> Clips { get; } = new();
        
        public TimelineViewModel()
        {
            Clips.Add(new ClipViewModel(new ClipModel
            (
                "DEFOeffect99.png",
                "source/DEFOeffect99.png",
                "image/png",
                300,
                150
            )));
        }

        // ドロップ操作に応じて新しいクリップを追加するメソッド
        public void AddClip(string assetName, string assetPath, string assetType, double positionX)
        {
            var newClipViewModel = new ClipViewModel(new ClipModel
            (
                assetName,
                assetPath,
                assetType,
                positionX,
                150 // 仮の幅
            ));
            Clips.Add(newClipViewModel);
        }

        public void SetClipPositionX(ClipViewModel clipViewModel, double positionX)
        {
            var test1 = Clips[Clips.IndexOf(clipViewModel)].ClipItemPositionX;
            Clips[Clips.IndexOf(clipViewModel)].ClipItemPositionX = positionX;
            Clips[Clips.IndexOf(clipViewModel)].LeftMarginThickness = new Thickness(positionX, 0, 0, 0);
            var test2 = Clips[Clips.IndexOf(clipViewModel)].ClipItemPositionX;
        }
    }
}