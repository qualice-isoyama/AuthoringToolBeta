using AuthoringToolBeta.Commands;
using AuthoringToolBeta.Common;
using Avalonia;
using ReactiveUI;
using AuthoringToolBeta.Model;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuthoringToolBeta.ViewModels
{
    public class ClipViewModel: ReactiveObject
    {
        private TrackViewModel _parentViewModel;
        public TrackViewModel ParentViewModel 
        {
            get => _parentViewModel; 
            set => this.RaiseAndSetIfChanged(ref _parentViewModel, value);
        }
        public ICommand SelectCommand { get; }
        private ClipModel _clipItem;
        public ClipModel ClipItem
        {
            get => _clipItem; 
            set => this.RaiseAndSetIfChanged(ref _clipItem, value);
        }

        private string _clipItemName;

        public string ClipItemName
        {
            get => _clipItemName; 
            set =>  this.RaiseAndSetIfChanged(ref _clipItemName, value);
        }

        private string _clipItemPath;

        public string ClipItemPath
        {
            get => _clipItemPath; 
            set =>  this.RaiseAndSetIfChanged(ref _clipItemPath, value);
        }

        private string _clipItemType;

        public string ClipItemType
        {
            get => _clipItemType; 
            set =>   this.RaiseAndSetIfChanged(ref _clipItemType, value);
        }

        private int _priority;

        public int Priority
        {
            get => _priority;
            set => this.RaiseAndSetIfChanged(ref _priority, value);
        }
        private Thickness _leftMarginThickness;

        public Thickness LeftMarginThickness
        {
            get => _leftMarginThickness;
            set =>  this.RaiseAndSetIfChanged(ref _leftMarginThickness, value);
        }
        private double _startTime;
        public double StartTime // PositionX から変更
        {
            get => _startTime;
            set => this.RaiseAndSetIfChanged(ref _startTime, value);
        }

        private double _endTime;

        public double EndTime
        {
            get => _endTime;
            set => this.RaiseAndSetIfChanged(ref _endTime, value);
        }

        private double _duration;
        public double Duration // Width から変更
        {
            get => _duration;
            set => this.RaiseAndSetIfChanged(ref _duration, value);
        }
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
        
        public ClipViewModel(ClipModel model)
        {
            ClipItem = model;
            ClipItemName = model.AssetName;
            ClipItemPath = model.AssetPath;
            ClipItemType = model.AssetType;
            StartTime = model.StartTime;
            EndTime = model.StartTime + model.Duration;
            Duration = model.Duration;
            LeftMarginThickness = new Thickness(model.StartTime * ParentViewModel.ParentViewModel.Scale, 0, 0, 0);
        }

        public ClipViewModel(ClipModel model, TrackViewModel parent)
        {
            _parentViewModel = parent;
            ClipItem = model;
            ClipItemName = model.AssetName;
            ClipItemPath = model.AssetPath;
            ClipItemType = model.AssetType;
            StartTime = model.StartTime;
            EndTime = model.StartTime + model.Duration;
            Duration = model.Duration;
            LeftMarginThickness = new Thickness(model.StartTime * ParentViewModel.ParentViewModel.Scale, 0, 0, 0);
            IsSelected = false;
            SelectCommand = new RelayCommand(_ => _parentViewModel.ParentViewModel.SelectClip(this));
        }
        public ClipModel ToModel()
        {
            return ClipItem;
        }

        public void SetPriority(int priority)
        {
            Priority = priority;
        }

        // スケール変更の際のクリップ位置調整
        public void UpdateClip()
        {
            LeftMarginThickness =  new  Thickness(StartTime * _parentViewModel.ParentViewModel.Scale, 0, 0, 0);
        }
    }
}