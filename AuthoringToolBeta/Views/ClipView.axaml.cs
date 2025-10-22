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
                    vm.SelectCommand.Execute(vm);
                }
            }
        }
    }

    private void LeftHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
            DataContext is ClipViewModel cvm)
        {
            _dragStartPoint = e.GetPosition(null);
            _beforePoint = e.GetPosition(null);
            _originalStartTime = cvm.StartTime;
            _originalDuration = cvm.Duration;
            _isDragging = true;
            _currentDragMode = DragMode.ResizeLeft;
            e.Pointer.Capture(border);
            border.PointerMoved += Handle_PointerMoved;
            border.PointerReleased += Handle_PointerReleased;
            e.Handled = true;
        }
    }
    

    private void RightHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
            DataContext is ClipViewModel cvm)
        {
            _dragStartPoint = e.GetPosition(null);
            _beforePoint = e.GetPosition(null);
            _originalStartTime = cvm.StartTime;
            _originalDuration = cvm.Duration;
            _isDragging = true;
            _currentDragMode = DragMode.ResizeRight;
            e.Pointer.Capture(border);
            border.PointerMoved += Handle_PointerMoved;
            border.PointerReleased += Handle_PointerReleased;
            e.Handled = true;
        }
    }
    private void Handle_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (DataContext is ClipViewModel cvm && _isDragging)
        {
            UpdateClip(e.GetPosition(null), cvm);
        }
    }

    private void Handle_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Border border )
        {
            _isDragging = false;
            _currentDragMode = DragMode.None;
            e.Pointer.Capture(null);
            border.PointerMoved -= Handle_PointerMoved;
            border.PointerReleased -= Handle_PointerReleased;
            e.Handled = true;
        }
    }

    public void UpdateClip(Point currentPoint, ClipViewModel cvm)
    {
        var deltaX =  currentPoint.X - _beforePoint.X;
        var deltaTime = deltaX / cvm.ParentViewModel.ParentViewModel.Scale;
        if (_currentDragMode == DragMode.ResizeLeft)
        {
            var originalStartTime = cvm.StartTime;
            var newStartTime = originalStartTime + deltaTime;
            var originalEndTime = _originalStartTime + _originalDuration;
            newStartTime= Math.Max(0, newStartTime);
            newStartTime = Math.Min(newStartTime, originalEndTime - 1);
            if (newStartTime < originalEndTime - 1 && (cvm.EndTime - cvm.StartTime) > 1)
            {
                cvm.StartTime = newStartTime;
                cvm.LeftMarginThickness = new Thickness(newStartTime * cvm.ParentViewModel.ParentViewModel.Scale, 0, 0, 0);
                if (cvm.StartTime > 0)
                {
                    cvm.Duration = cvm.EndTime - cvm.StartTime;
                    cvm.Duration = Math.Max(1, cvm.Duration);
                }
            }
        }
        else if (_currentDragMode == DragMode.ResizeRight)
        {
            cvm.Duration += deltaTime;
            cvm.Duration = Math.Max(1, cvm.Duration);
            cvm.EndTime = cvm.StartTime + cvm.Duration;
        }
        _beforePoint = currentPoint;
    }
    
}