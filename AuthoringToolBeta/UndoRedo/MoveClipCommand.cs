using AuthoringToolBeta.ViewModels;
using Avalonia;

namespace AuthoringToolBeta.UndoRedo
{
    public class MoveClipCommand : IUndoableCommand
    {
        private readonly ClipViewModel _targetClip;
        private readonly double _oldStartTime;
        private readonly double _newStartTime;

        public MoveClipCommand(ClipViewModel targetClip, double oldStartTime, double newStartTime)
        {
            _targetClip = targetClip;
            _oldStartTime = oldStartTime;
            _newStartTime = newStartTime;
        }

        public void Execute()
        {
            _targetClip.StartTime = _newStartTime;
            _targetClip.EndTime = _newStartTime + _targetClip.Duration;
            _targetClip.LeftMarginThickness =
                new Thickness(_targetClip.StartTime * _targetClip.ParentViewModel.ParentViewModel.Scale, 0, 0, 0);
        }

        public void Unexecute()
        {
            _targetClip.StartTime = _oldStartTime;
            _targetClip.EndTime = _oldStartTime + _targetClip.Duration;
            _targetClip.LeftMarginThickness =
                new Thickness(_targetClip.StartTime * _targetClip.ParentViewModel.ParentViewModel.Scale, 0, 0, 0);
        }
    }
}