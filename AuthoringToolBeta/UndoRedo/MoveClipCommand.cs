using System.Collections.ObjectModel;
using System.Linq;
using AuthoringToolBeta.ViewModels;
using Avalonia;

namespace AuthoringToolBeta.UndoRedo
{
    public class MoveClipCommand : IUndoableCommand
    {
        private readonly ObservableCollection<ClipViewModel> _targetClips;
        private readonly ObservableCollection<double> _oldStartTimes;
        private readonly ObservableCollection<double> _newStartTimes;
        private readonly double _scale;

        public MoveClipCommand(ObservableCollection<ClipViewModel> targetClip)
        {
            if (targetClip.First() != null)
            {
                _scale = targetClip.First().ParentViewModel.ParentViewModel.Scale;
                _targetClips = targetClip;
                _oldStartTimes = new();
                _newStartTimes = new();
                for (int clipIdx = 0; clipIdx < targetClip.Count; clipIdx++)
                {
                    // ドラッグ開始時（コマンド実行前）の開始時間
                    _oldStartTimes.Add(_targetClips[clipIdx].DragStartTime);
                    // ドラッグ終了後（コマンド実行後）の開始時間
                    _newStartTimes.Add(_targetClips[clipIdx].StartTime);
                }
            }
        }

        public void Execute()
        {
            for (int clipIdx = 0; clipIdx < _targetClips.Count; clipIdx++)
            {
                _targetClips[clipIdx].StartTime = _newStartTimes[clipIdx];
                _targetClips[clipIdx].EndTime = _newStartTimes[clipIdx] + _targetClips[clipIdx].Duration;
                _targetClips[clipIdx].LeftMarginThickness = new Thickness(
                    _targetClips[clipIdx].StartTime * _scale, 0, 0, 0);
            }
        }

        public void Unexecute()
        {
            for (int clipIdx = 0; clipIdx < _targetClips.Count; clipIdx++)
            {
                _targetClips[clipIdx].StartTime = _oldStartTimes[clipIdx];
                _targetClips[clipIdx].EndTime = _oldStartTimes[clipIdx] + _targetClips[clipIdx].Duration;
                _targetClips[clipIdx].LeftMarginThickness = new Thickness(
                    _targetClips[clipIdx].StartTime * _scale, 0, 0, 0);
            }
        }
    }
}