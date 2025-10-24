using System.Collections.ObjectModel;
using AuthoringToolBeta.ViewModels;
using Avalonia.Controls.Primitives;

namespace AuthoringToolBeta.UndoRedo;

public class SelectClipCommand : IUndoableCommand
{
    private readonly ObservableCollection<ClipViewModel> _selectClips= new();
    private readonly TimelineViewModel _parentVM;
    private readonly ClipViewModel _targetClip;
    private readonly bool _isPressCtrl;
    private bool _isAddClip;
    
    public SelectClipCommand(ObservableCollection<ClipViewModel> clips, TimelineViewModel parentVM, ClipViewModel targetClip, bool isPressCtrl)
    {
        ObservableCollection<ClipViewModel> _selectClips = new();
        for (int clipIdx = 0; clipIdx < clips.Count; clipIdx++)
        {
            _selectClips.Add(clips[clipIdx]);
        }
        _parentVM = parentVM;
        _targetClip = targetClip;
        _isPressCtrl = isPressCtrl;
    }

    public void Execute()
    {
        // Ctrlキーが押されていない場合（通常の単一クリック）
        // 選択済みリストをクリア
        if (!_isPressCtrl)
        {
            for (int trackIdx = 0; trackIdx < _parentVM.Tracks.Count; trackIdx++)
            {
                for (int clipIdx = 0; clipIdx < _parentVM.Tracks[trackIdx].Clips.Count; clipIdx++)
                {
                    _parentVM.Tracks[trackIdx].Clips[clipIdx].IsSelected = false;
                }
            }
            _parentVM.SelectedClips.Clear();
        }
        // クリックされたクリップの選択状態を反転
        _targetClip.IsSelected = !_targetClip.IsSelected;
        // 選択状態に応じて選択済みリストに追加/削除
        if (_targetClip.IsSelected)
        {
            if (!_selectClips.Contains(_targetClip))
            {
                _isAddClip = true;
                _parentVM.SelectedClips.Add(_targetClip);
            }
        }
        else // 選択状態でなければ選択済みリストから削除
        {
            _isAddClip = false;
            _parentVM.SelectedClips.Remove(_targetClip);
        }
    }
    public void Unexecute()
    {
        // クリックされたクリップの選択状態を反転
        _targetClip.IsSelected = !_targetClip.IsSelected;
        // 選択済みリストを復元
        _parentVM.SelectedClips.Clear();
        for (int selectClipIdx = 0; selectClipIdx < _selectClips.Count; selectClipIdx++)
        {
            _parentVM.SelectedClips.Add(_selectClips[selectClipIdx]);
        }
    }
    
}