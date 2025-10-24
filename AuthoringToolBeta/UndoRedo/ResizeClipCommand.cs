using System.Collections.ObjectModel;
using System.Linq;
using AuthoringToolBeta.ViewModels;
using Avalonia;

namespace AuthoringToolBeta.UndoRedo
{
    public class ResizeClipCommand : IUndoableCommand
    {
        private readonly ObservableCollection<ClipViewModel> _targetClips;
        private readonly ObservableCollection<double> _oldStartTimes;
        private readonly ObservableCollection<double> _newStartTimes;
        private readonly ObservableCollection<double> _oldDurations;
        private readonly ObservableCollection<double> _newDurations;
        private readonly double _scale;

        public ResizeClipCommand(
            ObservableCollection<ClipViewModel> targetClip)
        {
            if (targetClip.First() != null)
            {
                _scale = targetClip.First().ParentViewModel.ParentViewModel.Scale;
                _targetClips = targetClip;
                _oldStartTimes = new();
                _newStartTimes = new();
                _oldDurations = new();
                _newDurations = new();
                for (int clipIdx = 0; clipIdx < targetClip.Count; clipIdx++)
                {
                    _oldStartTimes.Add(_targetClips[clipIdx].DragStartTime);
                    _newStartTimes.Add(_targetClips[clipIdx].StartTime);
                    _oldDurations.Add(_targetClips[clipIdx].DragStartDuration);
                    _newDurations.Add(_targetClips[clipIdx].Duration);
                }
            }
        }

        public void Execute()
        {
            for (int clipIdx = 0; clipIdx < _targetClips.Count; clipIdx++)
            {
                _targetClips[clipIdx].StartTime = _newStartTimes[clipIdx];
                _targetClips[clipIdx].EndTime = _newStartTimes[clipIdx] + _newDurations[clipIdx];
                _targetClips[clipIdx].LeftMarginThickness = new Thickness(
                    _targetClips[clipIdx].StartTime * _scale, 0, 0, 0);
                if (_newStartTimes[clipIdx] > 0)
                {
                    _targetClips[clipIdx].Duration = _newDurations[clipIdx];
                }
            }
        }

        public void Unexecute()
        {
            for (int clipIdx = 0; clipIdx < _targetClips.Count; clipIdx++)
            {
                _targetClips[clipIdx].StartTime = _oldStartTimes[clipIdx];
                _targetClips[clipIdx].EndTime = _oldStartTimes[clipIdx] + _oldDurations[clipIdx];
                _targetClips[clipIdx].LeftMarginThickness = new Thickness(
                    _targetClips[clipIdx].StartTime * _scale, 0, 0, 0);
                _targetClips[clipIdx].Duration = _oldDurations[clipIdx];
            }
        }
    }
}