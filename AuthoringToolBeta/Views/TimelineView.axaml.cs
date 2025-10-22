using System;
using System.Reactive.PlatformServices;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using AuthoringToolBeta.Model;
using AuthoringToolBeta.ViewModels;


namespace AuthoringToolBeta.Views;

public partial class TimelineView : UserControl
{
    const string HierarchyViewModelFormat = "AuthoringToolBeta.ViewModels.HierarchyViewModel";
    const string dragDataFormat = "AuthoringToolBeta.ViewModels.ClipViewModel";
    private bool _isDraggingPlayhead = false;
    private bool _isDraggingClipItem = false;
    private Point _dragStartPoint;
    public TimelineView()
    {
        InitializeComponent();
    }
    private void Track_DragEnter(object? sender, DragEventArgs e)
    {
        // ドラッグされているデータがテキスト形式かを確認
        if (e.Data.Contains(HierarchyViewModelFormat))
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
        var viewModel = DataContext as TimelineViewModel;
        if (viewModel == null) return;

        // 2. ドロップされたアセット名と位置を取得
        if (e.Data.Get(HierarchyViewModelFormat) is HierarchyModel asset && sender is Control track && DataContext is TimelineViewModel tvm)
        {
            var dropPosition = e.GetPosition(track);

            if (sender is Border border && border.Name == "Track1")
            {
                // 3. UIを直接操作せず、ViewModelにクリップの追加を「依頼」する
                viewModel.AddClip(asset.Name, "", "", dropPosition.X,dropPosition.X / tvm.Scale, 2, 1);
            }
            else if (sender is Border border2 && border2.Name == "Track2")
            {
                viewModel.AddClip(asset.Name, "", "", dropPosition.X,dropPosition.X / tvm.Scale, 2, 2);
            }
            else if (sender is Border border3 && border3.Name == "Track3")
            {
                viewModel.AddClip(asset.Name, "", "", dropPosition.X,dropPosition.X / tvm.Scale, 2, 2);
            }
        }

        // タイムライン上のクリップを移動させた場合
        if (e.Data.Get(dragDataFormat) is ClipViewModel currClip && sender is Control trackclip)
        {
            var dropPosition = e.GetPosition(null);

            // 3. UIを直接操作せず、ViewModelにクリップの追加を「依頼」する
            viewModel.SetClipPositionX(currClip, dropPosition.X);
        }
    }
    
    private void PlayerHead_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // イベント発生源のCanvasとViewModelを取得
        if (sender is StackPanel canvas && this.DataContext is TimelineViewModel viewModel)
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
        if (_isDraggingPlayhead && sender is StackPanel canvas && this.DataContext is TimelineViewModel viewModel)
        {
            UpdatePlayheadPosition(e.GetPosition(canvas), viewModel);
        }
    }

    private void PlayerHead_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // ドラッグが終了したらフラグを倒し、イベント購読を解除
        if (_isDraggingPlayhead && sender is StackPanel canvas)
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
        if (sender is ClipView border && DataContext is TimelineViewModel vm)
        {
            if (e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
            {
                _isDraggingClipItem = true;
                _dragStartPoint = e.GetPosition(border);
                e.Pointer.Capture(border);
                border.PointerMoved += ClipPointerMoved;
                border.PointerReleased += ClipPointerReleased;
                e.Handled = true;

            }
        }
    }

    private void ClipPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDraggingClipItem && sender is ClipView border && DataContext is TimelineViewModel vm)
        {
            UpdateClipItemPosition(e.GetPosition(border), vm);
        }
    }

    private void ClipPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
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