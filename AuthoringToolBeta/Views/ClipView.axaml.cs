using System;
using System.Drawing;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.ViewModels;
using Point = Avalonia.Point;

namespace AuthoringToolBeta.Views;

public partial class ClipView : UserControl
{
    private bool _isDragging = false;
    private Point _dragStartPoint;
    private Point _beforePoint;
    private double _originalStartTime;
    private double _originalDuration;
    private enum DragMode { None, Move, ResizeLeft, ResizeRight }
    private DragMode _currentDragMode = DragMode.None;
    public ClipView()
    {
        InitializeComponent();
    }
    private async void Clip_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock textBlock)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                if (DataContext is ClipViewModel vm)
                {
                    // このクリップを選択する
                    //vm.SelectCommand.Execute(DataContext);
                    vm.ParentViewModel.SelectedClip = vm;
                }
            }
            // TextBlockに表示されているアセット名（データ）を取得
            /*var currAsset = textBlock.DataContext;
            if (currAsset != null)
            {
                // ドラッグ＆ドロップ操作を開始
                var dragDataTest = new DataObject();
                dragDataTest.Set(dragDataFormat, currAsset);

                // DoDragDrop操作を開始
                await DragDrop.DoDragDrop(e, dragDataTest, DragDropEffects.Copy);
            }*/
        }
    }

    private void LeftHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Avalonia.Controls.Shapes.Rectangle rect && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
            DataContext is ClipViewModel cvm)
        {
            _dragStartPoint = e.GetPosition(null);
            _beforePoint = e.GetPosition(null);
            _originalStartTime = cvm.StartTime;
            _originalDuration = cvm.Duration;
            _isDragging = true;
            _currentDragMode = DragMode.ResizeLeft;
            e.Pointer.Capture(rect);
            rect.PointerMoved += Handle_PointerMoved;
            rect.PointerReleased += Handle_PointerReleased;
            e.Handled = true;
        }
    }
    

    private void RightHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Avalonia.Controls.Shapes.Rectangle rect && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
            DataContext is ClipViewModel cvm)
        {
            _dragStartPoint = e.GetPosition(null);
            _beforePoint = e.GetPosition(null);
            _originalStartTime = cvm.StartTime;
            _originalDuration = cvm.Duration;
            _isDragging = true;
            _currentDragMode = DragMode.ResizeRight;
            e.Pointer.Capture(rect);
            rect.PointerMoved += Handle_PointerMoved;
            rect.PointerReleased += Handle_PointerReleased;
            e.Handled = true;
        }
    }
    private void Handle_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is Avalonia.Controls.Shapes.Rectangle rect && DataContext is ClipViewModel cvm)
        {
            UpdateClip(e.GetPosition(null), cvm);
        }
    }

    private void Handle_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Avalonia.Controls.Shapes.Rectangle rect )
        {
            _isDragging = false;
            _currentDragMode = DragMode.None;
            e.Pointer.Capture(null);
            rect.PointerMoved -= Handle_PointerMoved;
            rect.PointerReleased -= Handle_PointerReleased;
            e.Handled = true;
        }
    }

    public void UpdateClip(Point currentPoint, ClipViewModel cvm)
    {
        var deltaX =  currentPoint.X - _beforePoint.X;
        var deltaTime = deltaX / cvm.ParentViewModel.Scale;
        if (_currentDragMode == DragMode.ResizeLeft)
        {
            cvm.Duration -= deltaTime;
            cvm.Duration = Math.Max(0.3, cvm.Duration);
            if (cvm.Duration > 0.3)
            {
                var newStartTime = _originalStartTime + deltaTime;
                var originalEndTime = _originalStartTime + _originalDuration;
                newStartTime = Math.Min(newStartTime, originalEndTime - 0.1);
                var positionX = Math.Min((cvm.ClipItemPositionX + deltaX), (originalEndTime - 0.1) * cvm.ParentViewModel.Scale);
                cvm.ClipItemPositionX = positionX; 
                cvm.StartTime = newStartTime;
                cvm.LeftMarginThickness = new Thickness(cvm.ClipItemPositionX, 0, 0, 0);
            }
        }
        else if (_currentDragMode == DragMode.ResizeRight)
        {
            cvm.Duration += deltaTime;
            cvm.Duration = Math.Max(0.3, cvm.Duration);
        }
        cvm.ClipItemWidth = cvm.Duration * cvm.ParentViewModel.Scale;
        _beforePoint = currentPoint;
    }
    
}