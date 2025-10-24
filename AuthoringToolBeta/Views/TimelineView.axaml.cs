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
    // �u�Ԃ̈ړ������̎Z�o�̊�_
    private Point _oldPosition;
    // �X�i�b�v���������� (�s�N�Z���P��)
    private const double SnapThresholdInPixels = 10.0;

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
                tvm.SelectedClips.Add(addedClip);
                addedClip.IsSelected = true;
            }
        }

        // �^�C�����C����̃N���b�v���ړ��������ꍇ
        if (e.Data.Get(DragDataFormat) is ClipViewModel currClip && track.DataContext is TrackViewModel currTrack)
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
        // �u�Ԃ̈ړ������̊
        _oldPosition = e.GetPosition(sender as ClipView);
        // �h���b�O�J�n�_
        if (sender is ClipView clip && e.GetCurrentPoint(clip).Properties.IsLeftButtonPressed && clip.DataContext is ClipViewModel cvm)
        {
            _isDraggingClipItem = true;
            // �e�N���b�v�̃h���b�O�J�n����StartTime
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
        // �S�̂ɓK�p����u�Ԃ̈ړ�����
        var deltaTime = (currentPosition.X - _oldPosition.X) / tvm.Scale;
        for (int selectClipIdx = 0; selectClipIdx < tvm.SelectedClips.Count; selectClipIdx++)
        {
            if (tvm.SelectedClips[selectClipIdx] != null)
            {
                // �ړ��������s���e�N���b�v
                var clip = tvm.SelectedClips[selectClipIdx];
                
                // �N���b�v�ړ�����
                // �����O�̊J�n����
                var oldStartTime = clip.StartTime;
                // ������̊J�n����
                var newStartTime = Math.Max(0, oldStartTime + deltaTime);
                
                // �������ʂ�K�p
                clip.StartTime = newStartTime;
                clip.EndTime = newStartTime + clip.Duration;
                clip.LeftMarginThickness = new Thickness(newStartTime * tvm.Scale, 0, 0, 0);
                
                // �X�i�b�v�����J�n
                // �����g���b�N��̑��̃N���b�v�̃��X�g���擾
                var onSameTrackClips = clip.ParentViewModel.Clips.Except(tvm.SelectedClips).ToList();
                // �X�i�b�v�������s�N�Z�����玞�Ԃɕϊ�
                var snapThresholdInSeconds = SnapThresholdInPixels / tvm.Scale;
                // �}�E�X�ٓ�����v�Z�����A�X�i�b�v�O�̖{���̊J�n����
                var potentialStartTime = oldStartTime + deltaTime;
                var potentialEndTime = potentialStartTime + clip.Duration;
                // �œK�ȃX�i�b�v�ʒu��T��
                double bestSnappOffset = double.MaxValue;
                foreach (var otherClip in onSameTrackClips)
                {
                    // 4�̃G�b�W�ɑ΂��ă`�F�b�N
                    CheckSnap(potentialStartTime, otherClip.StartTime, ref bestSnappOffset, snapThresholdInSeconds);
                    CheckSnap(potentialStartTime,otherClip.StartTime + otherClip.Duration, ref bestSnappOffset, snapThresholdInSeconds);
                    CheckSnap(potentialEndTime, otherClip.StartTime, ref bestSnappOffset, snapThresholdInSeconds);
                    CheckSnap(potentialEndTime,otherClip.StartTime +  otherClip.Duration, ref bestSnappOffset, snapThresholdInSeconds);
                }
                // �X�i�b�v�����������ꍇ�́AdaltaTime��␳
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
    // �X�i�b�v���`�F�b�N����w���p�[���\�b�h
    private void CheckSnap(double movingEdge, double staticEdge, ref double bestOffset, double threshold)
    {
        var diff = staticEdge - movingEdge;
        // ����臒l�ȓ��ŁA���A����܂Ō��������I�t�Z�b�g��菬�����ꍇ
        if (Math.Abs(diff) < threshold && Math.Abs(diff) < Math.Abs(bestOffset))
        {
            bestOffset = diff; // �œK�ȃI�t�Z�b�g���X�V
        }
    }
}