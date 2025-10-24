using System;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.UndoRedo;
using AuthoringToolBeta.ViewModels;


namespace AuthoringToolBeta.Views;

public partial class TimelineView : UserControl
{
    private const string HierarchyItemViewModelFormat = "AuthoringToolBeta.ViewModels.HierarchyItemViewModel";
    private const string DragDataFormat = "AuthoringToolBeta.ViewModels.ClipViewModel";
    private bool _isDraggingPlayhead = false;
    private bool _isDraggingClipItem = false;
    // 瞬間の移動距離の算出の基準点
    private Point _oldPosition;
    // スナップが効く距離 (ピクセル単位)
    private const double SnapThresholdInPixels = 10.0;

    public TimelineView()
    {
        InitializeComponent();
    }
    private void Track_DragEnter(object? sender, DragEventArgs e)
    {
        // ドラッグされているデータがテキスト形式かを確認
        if (e.Data.Contains(HierarchyItemViewModelFormat))
        {
            // 受け入れ可能であることをカーソル形状で示す
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            // 受け入れ不可であることを示す
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void Track_Drop(object? sender, DragEventArgs e)
    {
        // 1. このViewのDataContextであるViewModelを取得
        var tvm = DataContext as TimelineViewModel;
        if (tvm == null) return;
        var track = sender as Border;

        // 2. ドロップされたアセット名と位置を取得
        if (e.Data.Get(HierarchyItemViewModelFormat) is HierarchyItemViewModel asset)
        {
            if (track.DataContext is TrackViewModel targetTrackVM)
            {
                var dropPosition = e.GetPosition(track);
                var addedClip = tvm.AddClip(asset.HModel.Name, "", "", dropPosition.X / tvm.Scale, asset.HModel.Duration,targetTrackVM);
                tvm.SelectedClips.Add(addedClip);
                addedClip.IsSelected = true;
            }
        }

        // タイムライン上のクリップを移動させた場合
        if (e.Data.Get(DragDataFormat) is ClipViewModel currClip && track.DataContext is TrackViewModel currTrack)
        {
            var dropPosition = e.GetPosition(null);
            tvm.SetClipPositionX(currTrack,currClip, dropPosition.X);
        }
    }
    
    private void PlayerHead_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // イベント発生源のCanvasとViewModelを取得
        if (sender is Border canvas && this.DataContext is TimelineViewModel viewModel)
        {
            // マウスの左ボタンが押されたらドラッグ開始
            if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed)
            {
                _isDraggingPlayhead = true;
                
                // ポインターイベントをこのCanvasでキャプチャする
                e.Pointer.Capture(canvas);

                // PointerMovedイベントを購読開始
                canvas.PointerMoved += PlayerHead_PointerMoved;
                // PointerReleasedイベントを購読開始
                canvas.PointerReleased += PlayerHead_PointerReleased;
                
                // まずクリックした位置に再生ヘッドを移動させる
                UpdatePlayheadPosition(e.GetPosition(canvas), viewModel);

                e.Handled = true; // イベントが処理されたことを示す
            }
        }
    }
    private void PlayerHead_PointerMoved(object? sender, PointerEventArgs e)
    {
        // ドラッグ中であれば再生ヘッドの位置を更新
        if (_isDraggingPlayhead && sender is Border canvas && this.DataContext is TimelineViewModel viewModel)
        {
            UpdatePlayheadPosition(e.GetPosition(canvas), viewModel);
        }
    }

    private void PlayerHead_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // ドラッグが終了したらフラグを倒し、イベント購読を解除
        if (_isDraggingPlayhead && sender is Border canvas)
        {
            _isDraggingPlayhead = false;
            
            // ポインターのキャプチャを解放
            e.Pointer.Capture(null);
            
            // ハンドラを解除して、不要なイベント処理を防ぐ
            canvas.PointerMoved -= PlayerHead_PointerMoved;
            canvas.PointerReleased -= PlayerHead_PointerReleased;
            
            e.Handled = true;
        }
    }
    
    // 再生ヘッドの位置を更新するヘルパーメソッド
    private void UpdatePlayheadPosition(Point pointerPosition, TimelineViewModel viewModel)
    {
        // X座標がマイナスにならないようにClampする
        var positionX = Math.Max(0, pointerPosition.X);
        
        // ピクセル座標を時間(秒)に変換してViewModelを更新
        viewModel.CurrentTime = positionX / viewModel.Scale;
    }

    private void ClipPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // 瞬間の移動距離の基準
        _oldPosition = e.GetPosition(sender as ClipView);
        // ドラッグ開始点
        if (sender is ClipView clip && e.GetCurrentPoint(clip).Properties.IsLeftButtonPressed && clip.DataContext is ClipViewModel cvm)
        {
            _isDraggingClipItem = true;
            // 各クリップのドラッグ開始時のStartTime
            cvm.DragStartTime = cvm.StartTime;
            e.Pointer.Capture(clip);
            clip.PointerMoved += ClipPointerMoved;
            clip.PointerReleased += ClipPointerReleased;
            e.Handled = true;
        }
    }

    private void ClipPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDraggingClipItem && sender is ClipView clip && DataContext is TimelineViewModel tvm)
        {
            UpdateClipItemPosition(e.GetPosition(clip), tvm);
        }
    }

    private void ClipPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        TimelineViewModel tvm = DataContext as TimelineViewModel;
        //_dragDelta = (e.GetPosition(sender as ClipView).X - _dragStartPointTVM.X) / tvm.Scale;
        MoveClipCommand command = new MoveClipCommand(tvm.SelectedClips);
        tvm.UndoRedoManager.Do(command);
        _isDraggingClipItem = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private void UpdateClipItemPosition(Point currentPosition, TimelineViewModel tvm)
    {
        // 全体に適用する瞬間の移動距離
        var deltaTime = (currentPosition.X - _oldPosition.X) / tvm.Scale;
        for (int selectClipIdx = 0; selectClipIdx < tvm.SelectedClips.Count; selectClipIdx++)
        {
            if (tvm.SelectedClips[selectClipIdx] != null)
            {
                // 移動処理を行う各クリップ
                var clip = tvm.SelectedClips[selectClipIdx];
                
                // クリップ移動処理
                // 処理前の開始時間
                var oldStartTime = clip.StartTime;
                // 処理後の開始時間
                var newStartTime = Math.Max(0, oldStartTime + deltaTime);
                
                // 処理結果を適用
                clip.StartTime = newStartTime;
                clip.EndTime = newStartTime + clip.Duration;
                clip.LeftMarginThickness = new Thickness(newStartTime * tvm.Scale, 0, 0, 0);
                
                // スナップ処理開始
                // 同じトラック上の他のクリップのリストを取得
                var onSameTrackClips = clip.ParentViewModel.Clips.Except(tvm.SelectedClips).ToList();
                // スナップ距離をピクセルから時間に変換
                var snapThresholdInSeconds = SnapThresholdInPixels / tvm.Scale;
                // マウス異動から計算される、スナップ前の本来の開始時間
                var potentialStartTime = oldStartTime + deltaTime;
                var potentialEndTime = potentialStartTime + clip.Duration;
                // 最適なスナップ位置を探す
                double bestSnappOffset = double.MaxValue;
                foreach (var otherClip in onSameTrackClips)
                {
                    // 4つのエッジに対してチェック
                    CheckSnap(potentialStartTime, otherClip.StartTime, ref bestSnappOffset, snapThresholdInSeconds);
                    CheckSnap(potentialStartTime,otherClip.StartTime + otherClip.Duration, ref bestSnappOffset, snapThresholdInSeconds);
                    CheckSnap(potentialEndTime, otherClip.StartTime, ref bestSnappOffset, snapThresholdInSeconds);
                    CheckSnap(potentialEndTime,otherClip.StartTime +  otherClip.Duration, ref bestSnappOffset, snapThresholdInSeconds);
                }
                // スナップが見つかった場合は、daltaTimeを補正
                if (bestSnappOffset != double.MaxValue)
                {
                    deltaTime += bestSnappOffset;
                    clip.StartTime += deltaTime;
                    clip.EndTime = clip.StartTime + clip.Duration;
                    clip.LeftMarginThickness = new Thickness(clip.StartTime * tvm.Scale, 0, 0, 0);
                    clip.BeforePos = currentPosition.X;
                }
            }
        }
        _oldPosition = currentPosition;
    }
    // スナップをチェックするヘルパーメソッド
    private void CheckSnap(double movingEdge, double staticEdge, ref double bestOffset, double threshold)
    {
        var diff = staticEdge - movingEdge;
        // 差が閾値以内で、かつ、これまで見つかったオフセットより小さい場合
        if (Math.Abs(diff) < threshold && Math.Abs(diff) < Math.Abs(bestOffset))
        {
            bestOffset = diff; // 最適なオフセットを更新
        }
    }
}