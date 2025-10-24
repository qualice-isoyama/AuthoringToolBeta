using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.UndoRedo;
using AuthoringToolBeta.ViewModels;
using Point = Avalonia.Point;

namespace AuthoringToolBeta.Views;

public partial class ClipView : UserControl
{
    private bool _isDragging = false;
    private bool _isClipping = false;
    private bool _isCtrlPressed = false;
    private Point _oldPosition;
    public enum DragMode { None, Move, ResizeLeft, ResizeRight }
    private DragMode _currentDragMode = DragMode.None;
    public ClipView()
    {
        InitializeComponent();
    }
    private async void Clip_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        
        if (DataContext is ClipViewModel cvm && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && sender is Border clipBody)
        {
            // Ctrl???????? (Mac?Command??????)
            bool isCtrlPressed = (e.KeyModifiers & KeyModifiers.Control) != 0;
            _isCtrlPressed = isCtrlPressed;
            // Ctrl?????????????????????????????????????????
            if (isCtrlPressed)
            {
                _isClipping = true;
                clipBody.PointerReleased += Clip_PointerReleased;
                cvm.ParentViewModel.ParentViewModel.SelectClip(cvm, isCtrlPressed);
            }
            else
            {
                // ViewModel????????????
                cvm.ParentViewModel.ParentViewModel.SelectClip(cvm, isCtrlPressed);
            }
        }
    }

    private void Clip_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ClipViewModel cvm =  DataContext as ClipViewModel;
        _isClipping = false;
        if (sender is Border clipBody)
        {
            if (!_isClipping)
            {
                cvm.ParentViewModel.ParentViewModel.SelectClip(cvm, _isCtrlPressed);
            }
            clipBody.PointerReleased -= Clip_PointerReleased;
        }
    }

    private void LeftHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
            DataContext is ClipViewModel cvm)
        {
            _oldPosition = e.GetPosition(null);
            // ??????????????StartTime?Duration
            cvm.DragStartTime = cvm.StartTime;
            cvm.DragStartDuration = cvm.Duration;
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
            _oldPosition = e.GetPosition(null);
            // ??????????????StartTime?Duration
            cvm.DragStartTime = cvm.StartTime;
            cvm.DragStartDuration = cvm.Duration;
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
            UpdateClip(e.GetPosition(null), cvm.ParentViewModel.ParentViewModel);
        }
    }

    private void Handle_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Border border && border.DataContext is ClipViewModel cvm)
        {
            ResizeClipCommand command = new ResizeClipCommand(cvm.ParentViewModel.ParentViewModel.SelectedClips);
            cvm.ParentViewModel.ParentViewModel.UndoRedoManager.Do(command);
            _isDragging = false;
            _currentDragMode = DragMode.None;
            e.Pointer.Capture(null);
            border.PointerMoved -= Handle_PointerMoved;
            border.PointerReleased -= Handle_PointerReleased;
            e.Handled = true;
        }
    }

    public void UpdateClip(Point currentPosition, TimelineViewModel tvm)
    {
        // ??????????????
        double deltaTime =  (currentPosition.X - _oldPosition.X) / tvm.Scale;
        
        for (int selectClipIdx = 0; selectClipIdx < tvm.SelectedClips.Count; selectClipIdx++)
        {
            // ????????????
            var clip =  tvm.SelectedClips[selectClipIdx];
            
            if (_currentDragMode == DragMode.ResizeLeft)
            {
                //????????????????????
                
                // ????????
                // ????????
                double oldStartTime = clip.StartTime;
                // ???????? 
                double newStartTime = oldStartTime + deltaTime;
                // ???????
                double originalEndTime = clip.DragStartTime + clip.DragStartDuration;
                newStartTime= Math.Max(0, newStartTime);
                newStartTime = Math.Min(newStartTime, clip.EndTime - 1);
                
                if (newStartTime < originalEndTime - 1 && (clip.EndTime - clip.StartTime) > 1)
                {
                    clip.StartTime = newStartTime;
                    clip.LeftMarginThickness = new Thickness(newStartTime * clip.ParentViewModel.ParentViewModel.Scale, 0, 0, 0);
                    if (clip.StartTime > 0)
                    {
                        clip.Duration = clip.EndTime - clip.StartTime;
                        clip.Duration = Math.Max(1, clip.Duration);
                    }
                }
                else
                {
                    if (newStartTime >= originalEndTime - 1 )
                    {
                        //throw new InvalidOperationException("left Handle Validation Error");
                    }
                }
            }
            else if (_currentDragMode == DragMode.ResizeRight)
            {
                // ????????????????
                clip.Duration += deltaTime;
                clip.Duration = Math.Max(1, clip.Duration);
                clip.EndTime = clip.StartTime + clip.Duration;
            }
        }
        _oldPosition = currentPosition;
    }
}