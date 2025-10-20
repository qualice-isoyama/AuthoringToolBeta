using Avalonia;
using System;
using System.Windows.Input;
using AuthoringToolBeta.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using AuthoringToolBeta.Model;
using AuthoringToolBeta.ViewModels;
using AuthoringToolBeta.ViewModels.Base;



namespace AuthoringToolBeta.ViewModels
{
    public class TimelineViewModel: ViewModelBase
    {
        // クリップのViewModelを保持するコレクション
        
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ObservableCollection<ClipViewModel> Clips { get; } = new();
        public ObservableCollection<ClipViewModel> Clips2 { get; } = new();
        public ObservableCollection<ClipViewModel> Clips3 { get; } = new();
        public ObservableCollection<ObservableCollection<ClipViewModel>> Tracks { get; } = new();
        public ObservableCollection<TimeMarkerViewModel> TimeMarkers { get; } = new ();
        private ClipViewModel? _selectedClip;
        public ClipViewModel? SelectedClip
        {
            get => _selectedClip;
            set
            {
                if (_selectedClip != value)
                {
                    _selectedClip = value;
                    //OnPropertyChanged(); // "SelectedClip" プロパティが変更されたことをUIに通知
                }
            }
        }
        // タイムライン全体の総ピクセル幅 (仮に60秒固定とする)
        public double TotalWidth => 100.0 * Scale;
        private double _scale = 100.0; // デフォルト: 1秒 = 100ピクセル
        public double Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                OnPropertyChanged();
            
                // Scaleが変わったら、それに依存する計算結果も変わったことを通知する
                // (これは次のステップで使います)
                OnPropertyChanged(nameof(TotalWidth)); 
            }
        }

        
        
        public TimelineViewModel()
        {
            ZoomInCommand = new RelayCommand(_ => ZoomIn());
            ZoomOutCommand = new RelayCommand(_ => ZoomOut());
            Clips.Add(new ClipViewModel(new ClipModel
            (
                "DEFOeffect99.png",
                "source/DEFOeffect99.png",
                "image/png",
                300,
                100,
                3,
                2
            ),this));
            Tracks.Add(Clips);
            GenerateMarkers(TimeSpan.FromMinutes(1));
        }

        // ドロップ操作に応じて新しいクリップを追加するメソッド
        public void AddClip(string assetName, string assetPath, string assetType, double positionX, double startTime, double duration, int trackNum)
        {
            var newClipViewModel = new ClipViewModel(new ClipModel
            (
                assetName,
                assetPath,
                assetType,
                positionX,
                duration * Scale, // 仮の幅
                startTime,
                duration
            ),this);
            if (trackNum == 1)
            {
                Clips.Add(newClipViewModel);
            }
            else if (trackNum == 2)
            {
                Clips2.Add(newClipViewModel);
            }
            
        }

        public void SetClipPositionX(ClipViewModel clipViewModel, double positionX)
        {
            Clips[Clips.IndexOf(clipViewModel)].StartTime = positionX / 100;
            Clips[Clips.IndexOf(clipViewModel)].ClipItemPositionX = positionX;
            Clips[Clips.IndexOf(clipViewModel)].LeftMarginThickness = new Thickness(positionX, 0, 0, 0);
        }
        // クリップを選択するためのメソッド
        public void SelectClip(ClipViewModel clip)
        {
            SelectedClip = clip;
        }
        private void ZoomIn()
        {
            Scale *= 1.25; // スケールを25%拡大
            SelectedClip.UpdateClip();
            UpdateMarkerWindths();
        }

        private void ZoomOut()
        {
            Scale /= 1.25; // スケールを25%縮小
            SelectedClip.UpdateClip();
            UpdateMarkerWindths();
        }

        private void GenerateMarkers(TimeSpan totalDuration)
        {
            TimeMarkers.Clear();
            for (TimeSpan time = TimeSpan.Zero; time <= totalDuration; time = time.Add(TimeSpan.FromSeconds(1)))
            {
                TimeMarkers.Add(new TimeMarkerViewModel(time, Scale));
            }
        }

        private void UpdateMarkerWindths()
        {
            foreach (var marker in TimeMarkers)
            {
                marker.ScaleInterval = Scale;
            }
        }
    }
}