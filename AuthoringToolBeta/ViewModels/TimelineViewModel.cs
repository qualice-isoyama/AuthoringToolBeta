using Avalonia;
using System;
using System.Windows.Input;
using AuthoringToolBeta.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using AuthoringToolBeta.Model;
using AuthoringToolBeta.ViewModels;
using AuthoringToolBeta.ViewModels.Base;
using Avalonia.Threading;


namespace AuthoringToolBeta.ViewModels
{
    public class TimelineViewModel: ViewModelBase
    {
        // クリップのViewModelを保持するコレクション
        
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand GoToStartCommand { get; } 
        public ICommand GoToEndCommand { get; } 
        public ObservableCollection<ClipViewModel> Clips { get; } = new();
        public ObservableCollection<ClipViewModel> Clips2 { get; } = new();
        public ObservableCollection<ClipViewModel> Clips3 { get; } = new();
        public ObservableCollection<ObservableCollection<ClipViewModel>> Tracks { get; } = new();
        public ObservableCollection<TimeMarkerViewModel> TimeMarkers { get; } = new ();
        private TimelineHierarchyViewModel _timelineHierarchyVM;

        public TimelineHierarchyViewModel TimelineHierarchyVM
        {
            get => _timelineHierarchyVM;
            set
            {
                if (_timelineHierarchyVM != value)
                {
                    _timelineHierarchyVM = value;
                    OnPropertyChanged(); 
                }
            }
        }
        private ClipViewModel? _selectedClip;
        public ClipViewModel? SelectedClip
        {
            get => _selectedClip;
            set
            {
                if (_selectedClip != value)
                {
                    _selectedClip = value;
                    OnPropertyChanged(); 
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
                OnPropertyChanged(nameof(TotalWidth)); 
            }
        }

        private double currentTime;

        public double CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                OnPropertyChanged();
            }
        }
        private readonly DispatcherTimer _timer;
        private bool _isPlaying;

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged();
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
                3 * Scale,
                2 * Scale,
                3,
                2
            ),this));
            Tracks.Add(Clips);
            GenerateMarkers(TimeSpan.FromMinutes(1));
            TimelineHierarchyVM = new TimelineHierarchyViewModel();
            // タイマーの初期化
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _timer.Tick += OnTimerTick;
            PlayCommand = new RelayCommand(_ => Play());
            StopCommand = new RelayCommand(_ => Stop());
            GoToStartCommand = new RelayCommand(_ => GoToStart());
            GoToEndCommand = new RelayCommand(_ => GoToEnd());
        }

        // ドロップ操作に応じて新しいクリップを追加するメソッド
        public void AddClip(string assetName, string assetPath, string assetType, double positionX, double startTime, double duration, int trackNum)
        {
            var newClipViewModel = new ClipViewModel(new ClipModel
            (
                assetName,
                assetPath,
                assetType,
                startTime * Scale,
                duration * Scale,
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
            Clips[Clips.IndexOf(clipViewModel)].StartTime = positionX / Scale;
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
        private void Play()
        {
            // 既に再生中でなければ、再生を開始する
            if (!IsPlaying)
            {
                IsPlaying = true;
                _timer.Start();
            }
        }

        private void Stop()
        {
            // 再生中であれば、再生を停止する
            if (IsPlaying)
            {
                IsPlaying = false;
                _timer.Stop();
            }
        }
    
        // タイマーがTickするたびに呼ばれるメソッド
        private void OnTimerTick(object? sender, EventArgs e)
        {
            // 現在時間に、経過した時間（秒）を加算する
            CurrentTime += _timer.Interval.TotalSeconds;

            // タイムラインの終端に達したら停止する (仮に60秒を終端とする)
            if (CurrentTime >= 60.0) 
            {
                Stop();
                CurrentTime = 60.0; // 終端にピッタリ合わせる
            }
        }
        private void GoToStart()
        {
            // 再生中であれば停止してから移動
            if (IsPlaying) Stop();
            CurrentTime = 0.0;
        }

        private void GoToEnd()
        {
            // 再生中であれば停止してから移動
            if (IsPlaying) Stop();
            CurrentTime = 60.0; // 仮の総時間
        }
    }
}