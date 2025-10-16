using Avalonia;
using ReactiveUI;
using AuthoringToolBeta.Model;
using AuthoringToolBeta.ViewModels.Base;

namespace AuthoringToolBeta.ViewModels
{
    public class ClipViewModel: ReactiveObject
    {
        private ClipModel _clipItem;
        public ClipModel ClipItem
        {
            get => ClipItem; 
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
        private double _clipItemPositionX;

        public double ClipItemPositionX
        {
            get =>  _clipItemPositionX; 
            set =>  this.RaiseAndSetIfChanged(ref _clipItemPositionX, value);
        }
        private Thickness _leftMarginThickness;

        public Thickness LeftMarginThickness
        {
            get => _leftMarginThickness;
            set =>  this.RaiseAndSetIfChanged(ref _leftMarginThickness, value);
        }
        
        public ClipViewModel(ClipModel model)
        {
            this.ClipItem = model;
            this.ClipItemName = model.AssetName;
            this.ClipItemPath = model.AssetPath;
            this.ClipItemType = model.AssetType;
            this.ClipItemPositionX = model.PositionX;
            this.LeftMarginThickness = new Thickness(model.PositionX, 0, 0, 0);
        }
    }
}