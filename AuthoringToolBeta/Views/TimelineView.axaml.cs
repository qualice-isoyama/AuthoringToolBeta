using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.UndoRedo;
using AuthoringToolBeta.ViewModels;


namespace AuthoringToolBeta.Views;

public partial class TimelineView : UserControl
{
    const string HierarchyItemViewModelFormat = "AuthoringToolBeta.ViewModels.HierarchyItemViewModel";
    const string dragDataFormat = "AuthoringToolBeta.ViewModels.ClipViewModel";
    private bool _isDraggingPlayhead = false;
    private bool _isDraggingClipItem = false;
    private Point _dragStartPoint;
    private double  _dragStartTime;
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
                addedClip.SelectCommand.Execute(addedClip);
            }
        }

        // タイムライン上のクリップを移動させた場合
        if (e.Data.Get(dragDataFormat) is ClipViewModel currClip && track.DataContext is TrackViewModel currTrack)
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

        if (sender is ClipView clip && e.GetCurrentPoint(clip).Properties.IsLeftButtonPressed && clip.DataContext is ClipViewModel cvm)
        {
            _isDraggingClipItem = true;
            _dragStartPoint = e.GetPosition(clip);
            _dragStartTime = cvm.StartTime;
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
        MoveClipCommand command = new MoveClipCommand(tvm.SelectedClip,_dragStartTime,tvm.SelectedClip.StartTime);
        tvm.UndoRedoManager.Do(command);
        _isDraggingClipItem = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private void UpdateClipItemPosition(Point pointerPosition, TimelineViewModel tvm)
    {
        if (tvm.SelectedClip != null)
        {
            var originalStartTime = tvm.SelectedClip.StartTime;
            var deltaTime = (pointerPosition.X - _dragStartPoint.X) / tvm.Scale;
            var startTime = Math.Max(0, originalStartTime + deltaTime);
            tvm.SelectedClip.StartTime = startTime;
            tvm.SelectedClip.EndTime = startTime + tvm.SelectedClip.Duration;
            tvm.SelectedClip.LeftMarginThickness = new Thickness(startTime * tvm.Scale, 0, 0, 0);
            _dragStartPoint = pointerPosition;
        }
    }
}