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
        // �h���b�O����Ă���f�[�^���e�L�X�g�`�������m�F
        if (e.Data.Contains(HierarchyViewModelFormat))
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
        var viewModel = DataContext as TimelineViewModel;
        if (viewModel == null) return;

        // 2. �h���b�v���ꂽ�A�Z�b�g���ƈʒu���擾
        if (e.Data.Get(HierarchyViewModelFormat) is HierarchyModel asset && sender is Control track && DataContext is TimelineViewModel tvm)
        {
            var dropPosition = e.GetPosition(track);

            if (sender is Border border && border.Name == "Track1")
            {
                // 3. UI�𒼐ڑ��삹���AViewModel�ɃN���b�v�̒ǉ����u�˗��v����
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

        // �^�C�����C����̃N���b�v���ړ��������ꍇ
        if (e.Data.Get(dragDataFormat) is ClipViewModel currClip && sender is Control trackclip)
        {
            var dropPosition = e.GetPosition(null);

            // 3. UI�𒼐ڑ��삹���AViewModel�ɃN���b�v�̒ǉ����u�˗��v����
            viewModel.SetClipPositionX(currClip, dropPosition.X);
        }
    }
    
    private void PlayerHead_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // �C�x���g��������Canvas��ViewModel���擾
        if (sender is StackPanel canvas && this.DataContext is TimelineViewModel viewModel)
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
        if (_isDraggingPlayhead && sender is StackPanel canvas && this.DataContext is TimelineViewModel viewModel)
        {
            UpdatePlayheadPosition(e.GetPosition(canvas), viewModel);
        }
    }

    private void PlayerHead_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // �h���b�O���I��������t���O��|���A�C�x���g�w�ǂ�����
        if (_isDraggingPlayhead && sender is StackPanel canvas)
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