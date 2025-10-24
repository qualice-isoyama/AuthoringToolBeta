using Avalonia;
using Avalonia.Threading;
using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using AuthoringToolBeta.Model;
using AuthoringToolBeta.UndoRedo;
using AuthoringToolBeta.Commands;
using AuthoringToolBeta.ViewModels.Base;



namespace AuthoringToolBeta.ViewModels
{
    public class TimelineViewModel: ViewModelBase
    {
        public UndoRedoManager UndoRedoManager { get; } = new();
        public ICommand MoveClipCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand GoToStartCommand { get; } 
        public ICommand GoToEndCommand { get; } 
        public ObservableCollection<TrackViewModel> Tracks { get; } = new();
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
        public ObservableCollection<ClipViewModel> SelectedClips { get; } = new();
        // タイムライン全体の総ピクセル幅 (仮に60秒固定とする)
        public double TotalWidth => 100.0 * Scale;
        private double _scale = 100.0; // デフォルト: 1秒 = 100ピクセル
        public double Scale
        {
            get => _scale;
            set
            {
                _scale = Math.Max(1.0, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalWidth));
                // スケールが変更されたら目盛りを再計算
                UpdateTimeMarkers();
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
            Tracks.Add(new TrackViewModel("映像トラック 1",this));
            Tracks.Add(new TrackViewModel("音声トラック 1",this));
            Tracks.Add(new TrackViewModel("テロップトラック 1",this));
            Tracks.Add(new TrackViewModel("演出トラック 1",this));
            Tracks.Add(new TrackViewModel("フィルタトラック 1",this));
            Tracks.First().Clips.Add(new ClipViewModel(new ClipModel
            (
                "DEFOeffect99.png",
                "source/DEFOeffect99.png",
                "image/png",
                2,
                2
            ),Tracks.First()));
            //GenerateMarkers(TimeSpan.FromMinutes(1));
            TimelineHierarchyVM = new TimelineHierarchyViewModel();
            // タイマーの初期化
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _timer.Tick += OnTimerTick;
            //MoveClipCommand = new RelayCommand(_ => MoveClip());
            PlayCommand = new RelayCommand(_ => Play());
            StopCommand = new RelayCommand(_ => Stop());
            GoToStartCommand = new RelayCommand(_ => GoToStart());
            GoToEndCommand = new RelayCommand(_ => GoToEnd());
            UpdateTimeMarkers();
        }

        // ドロップ操作に応じて新しいクリップを追加するメソッド
        public ClipViewModel AddClip(
            string assetName, 
            string assetPath, 
            string assetType, 
            double startTime, 
            double duration, 
            TrackViewModel  targetTrack)
        {
            var newClipViewModel = new ClipViewModel(new ClipModel
            (
                assetName,
                assetPath,
                assetType,
                startTime,
                duration
            ),targetTrack);
            
            var command = new AddClipCommand(targetTrack, newClipViewModel);
            UndoRedoManager.Do(command);
            //targetTrack.Clips.Add(newClipViewModel);
            return newClipViewModel;
        }

        public void SetClipPositionX(TrackViewModel trvm, ClipViewModel cvm, double positionX)
        {
            trvm.Clips[trvm.Clips.IndexOf(cvm)].StartTime = positionX / Scale;
            trvm.Clips[trvm.Clips.IndexOf(cvm)].LeftMarginThickness = new Thickness(positionX, 0, 0, 0);
        }
        // クリップを選択するためのメソッド
        public void SelectClip(ClipViewModel clipToSelect, bool isCtrlPressed)
        {
            SelectClipCommand command = new SelectClipCommand(SelectedClips,this,clipToSelect,isCtrlPressed);
            UndoRedoManager.Do(command);
        }
        private void ZoomIn()
        {
            Scale =  Scale * 1.25; // スケールを25%拡大
            for (int trackIdx = 0; trackIdx < Tracks.Count; trackIdx++)
            {
                for (int clipIdx = 0; clipIdx < Tracks[trackIdx].Clips.Count; clipIdx++)
                {
                    Tracks[trackIdx].Clips[clipIdx].UpdateClip();
                }
            }
            //UpdateMarkerWindths();
        }

        private void ZoomOut()
        {
            Scale /= 1.25; // スケールを25%縮小
            for (int trackIdx = 0; trackIdx < Tracks.Count; trackIdx++)
            {
                for (int clipIdx = 0; clipIdx < Tracks[trackIdx].Clips.Count; clipIdx++)
                {
                    Tracks[trackIdx].Clips[clipIdx].UpdateClip();
                }
            }
            //UpdateMarkerWindths();
        }

        private void GenerateMarkers(TimeSpan totalDuration)
        {
            TimeMarkers.Clear();
            for (TimeSpan time = TimeSpan.Zero; time <= totalDuration; time = time.Add(TimeSpan.FromSeconds(1)))
            {
                //TimeMarkers.Add(new TimeMarkerViewModel(time, Scale));
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
        // 目盛りを再計算するメソッド
        private void UpdateTimeMarkers()
        {
            TimeMarkers.Clear();

            // スケールに応じて、目盛りの間隔を決定する
            double majorIntervalSeconds;
            if (Scale > 150) majorIntervalSeconds = 1;      // すごくズームイン
            else if (Scale > 75) majorIntervalSeconds = 2;  // ズームイン
            else if (Scale > 30) majorIntervalSeconds = 5;  // 標準
            else if (Scale > 10) majorIntervalSeconds = 10; // ズームアウト
            else majorIntervalSeconds = 30;                 // すごくズームアウト

            double minorIntervalSeconds = majorIntervalSeconds / 5.0; // 細かい目盛り
            double totalDurationSeconds = 60.0; // 仮の総時間

            for (double time = 0; time <= totalDurationSeconds; time += minorIntervalSeconds)
            {
                // 丸め誤差を吸収
                time = Math.Round(time * 100) / 100.0;

                bool isMajor = time % majorIntervalSeconds < 0.001;
            
                //var label = isMajor ? TimeSpan.FromSeconds(time).ToString(@"mm\:ss") : "";
                var label = TimeSpan.FromSeconds(time).ToString(@"mm\:ss");
                var position = (time - 0.12 ) * Scale;
            
                TimeMarkers.Add(new TimeMarkerViewModel(time, label, position, isMajor));
            }

            var test0 = TimeMarkers;
        }
    }
}