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
        // �h���b�O����Ă���f�[�^���e�L�X�g�`�������m�F
        if (e.Data.Contains(HierarchyItemViewModelFormat))
        {
            // �󂯓���\�ł��邱�Ƃ��J�[�\���`��Ŏ���
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            // �󂯓���s�ł��邱�Ƃ�����
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void Track_Drop(object? sender, DragEventArgs e)
    {
        // 1. ����View��DataContext�ł���ViewModel���擾
        var tvm = DataContext as TimelineViewModel;
        if (tvm == null) return;
        var track = sender as Border;

        // 2. �h���b�v���ꂽ�A�Z�b�g���ƈʒu���擾
        if (e.Data.Get(HierarchyItemViewModelFormat) is HierarchyItemViewModel asset)
        {
            if (track.DataContext is TrackViewModel targetTrackVM)
            {
                var dropPosition = e.GetPosition(track);
                var addedClip = tvm.AddClip(asset.HModel.Name, "", "", dropPosition.X / tvm.Scale, asset.HModel.Duration,targetTrackVM);
                addedClip.SelectCommand.Execute(addedClip);
            }
        }

        // �^�C�����C����̃N���b�v���ړ��������ꍇ
        if (e.Data.Get(dragDataFormat) is ClipViewModel currClip && track.DataContext is TrackViewModel currTrack)
        {
            var dropPosition = e.GetPosition(null);
            tvm.SetClipPositionX(currTrack,currClip, dropPosition.X);
        }
    }
    
    private void PlayerHead_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // �C�x���g��������Canvas��ViewModel���擾
        if (sender is Border canvas && this.DataContext is TimelineViewModel viewModel)
        {
            // �}�E�X�̍��{�^���������ꂽ��h���b�O�J�n
            if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed)
            {
                _isDraggingPlayhead = true;
                
                // �|�C���^�[�C�x���g������Canvas�ŃL���v�`������
                e.Pointer.Capture(canvas);

                // PointerMoved�C�x���g���w�ǊJ�n
                canvas.PointerMoved += PlayerHead_PointerMoved;
                // PointerReleased�C�x���g���w�ǊJ�n
                canvas.PointerReleased += PlayerHead_PointerReleased;
                
                // �܂��N���b�N�����ʒu�ɍĐ��w�b�h���ړ�������
                UpdatePlayheadPosition(e.GetPosition(canvas), viewModel);

                e.Handled = true; // �C�x���g���������ꂽ���Ƃ�����
            }
        }
    }
    private void PlayerHead_PointerMoved(object? sender, PointerEventArgs e)
    {
        // �h���b�O���ł���΍Đ��w�b�h�̈ʒu���X�V
        if (_isDraggingPlayhead && sender is Border canvas && this.DataContext is TimelineViewModel viewModel)
        {
            UpdatePlayheadPosition(e.GetPosition(canvas), viewModel);
        }
    }

    private void PlayerHead_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // �h���b�O���I��������t���O��|���A�C�x���g�w�ǂ�����
        if (_isDraggingPlayhead && sender is Border canvas)
        {
            _isDraggingPlayhead = false;
            
            // �|�C���^�[�̃L���v�`�������
            e.Pointer.Capture(null);
            
            // �n���h�����������āA�s�v�ȃC�x���g������h��
            canvas.PointerMoved -= PlayerHead_PointerMoved;
            canvas.PointerReleased -= PlayerHead_PointerReleased;
            
            e.Handled = true;
        }
    }
    
    // �Đ��w�b�h�̈ʒu���X�V����w���p�[���\�b�h
    private void UpdatePlayheadPosition(Point pointerPosition, TimelineViewModel viewModel)
    {
        // X���W���}�C�i�X�ɂȂ�Ȃ��悤��Clamp����
        var positionX = Math.Max(0, pointerPosition.X);
        
        // �s�N�Z�����W������(�b)�ɕϊ�����ViewModel���X�V
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