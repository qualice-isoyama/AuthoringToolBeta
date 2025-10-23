using AuthoringToolBeta.ViewModels;
using AuthoringToolBeta.Views;
using Avalonia;

namespace AuthoringToolBeta.UndoRedo
{
    public class ResizeClipCommand : IUndoableCommand
    {
        private readonly ClipViewModel _targetClip;
        private readonly ClipView.DragMode _dragMode;
        private readonly double _oldStartTime;
        private readonly double _newStartTime;
        private readonly double _oldDuration;
        private readonly double _newDuration;

        public ResizeClipCommand(
            ClipViewModel targetClip, 
            double oldStartTime, 
            double newStartTime,
            double oldDuration, 
            double newDuration)
        {
            _targetClip = targetClip;
            _oldStartTime = oldStartTime;
            _newStartTime = newStartTime;
            _oldDuration = oldDuration;
            _newDuration = newDuration;
        }

        public void Execute()
        {
            _targetClip.StartTime = _newStartTime;
            _targetClip.EndTime = _newStartTime + _newDuration;
            _targetClip.LeftMarginThickness =
                new Thickness(_targetClip.StartTime * _targetClip.ParentViewModel.ParentViewModel.Scale, 0, 0, 0);
            if (_newStartTime > 0)
            {
                _targetClip.Duration = _newDuration;
            }
        }

        public void Unexecute()
        {
            _targetClip.StartTime = _oldStartTime;
            _targetClip.EndTime = _oldStartTime + _oldDuration;
            _targetClip.LeftMarginThickness =
                new Thickness(_oldStartTime * _targetClip.ParentViewModel.ParentViewModel.Scale, 0, 0, 0);
            _targetClip.Duration = _oldDuration;
        }
    }
}