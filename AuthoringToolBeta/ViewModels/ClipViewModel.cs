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
        private TimelineViewModel _parentViewModel;
        public TimelineViewModel ParentViewModel 
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
        private double _clipItemPositionX;

        public double ClipItemPositionX
        {
            get =>  _clipItemPositionX; 
            set =>  this.RaiseAndSetIfChanged(ref _clipItemPositionX, value);
        }
        private double _clipItemWidth;

        public double ClipItemWidth
        {
            get => _clipItemWidth;
            set =>  this.RaiseAndSetIfChanged(ref _clipItemWidth, value);
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

        private double _duration;
        public double Duration // Width から変更
        {
            get => _duration;
            set => this.RaiseAndSetIfChanged(ref _duration, value);
        }
        
        public ClipViewModel(ClipModel model)
        {
            this.ClipItem = model;
            this.ClipItemName = model.AssetName;
            this.ClipItemPath = model.AssetPath;
            this.ClipItemType = model.AssetType;
            this.ClipItemPositionX = model.PositionX;
            this.ClipItemWidth  = model.Width;
            this.LeftMarginThickness = new Thickness(model.PositionX, 0, 0, 0);
        }

        public ClipViewModel(ClipModel model, TimelineViewModel parent)
        {
            this._parentViewModel = parent;
            this.ClipItem = model;
            this.ClipItemName = model.AssetName;
            this.ClipItemPath = model.AssetPath;
            this.ClipItemType = model.AssetType;
            this.ClipItemPositionX = model.PositionX;
            this.ClipItemWidth  = model.Width;
            this.LeftMarginThickness = new Thickness(model.PositionX, 0, 0, 0);
            this._startTime = model.StartTime;
            this._duration = model.Duration;
            this.SelectCommand = new RelayCommand(_ => _parentViewModel.SelectClip(this));
            this._parentViewModel = parent;
        }
        public ClipModel ToModel()
        {
            return ClipItem;
        }

        public void SetPriority(int priority)
        {
            Priority = priority;
        }

        public void UpdateClip()
        {
            ClipItemWidth = ParentViewModel.Scale * Duration;
            ClipItemPositionX = ParentViewModel.Scale * StartTime;
            LeftMarginThickness =  new  Thickness(ClipItemPositionX, 0, 0, 0);
        }
    }
}