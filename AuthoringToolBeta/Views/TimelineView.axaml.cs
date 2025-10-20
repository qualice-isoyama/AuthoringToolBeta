using Avalonia.Controls;
using Avalonia.Input;
using AuthoringToolBeta.Model;
using AuthoringToolBeta.ViewModels;
using Avalonia.Remote.Protocol.Input;
using Avalonia.Threading;

namespace AuthoringToolBeta.Views;

public partial class TimelineView : UserControl
{
    const string HierarchyViewModelFormat = "AuthoringToolBeta.ViewModels.HierarchyViewModel";
    const string dragDataFormat = "AuthoringToolBeta.ViewModels.ClipViewModel";
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
        if (e.Data.Get(HierarchyViewModelFormat) is HierarchyModel asset && sender is Control track)
        {
            var dropPosition = e.GetPosition(track);

            if (sender is Border border && border.Name == "Track1")
            {
                // 3. UI�𒼐ڑ��삹���AViewModel�ɃN���b�v�̒ǉ����u�˗��v����
                viewModel.AddClip(asset.Name, "", "", dropPosition.X,dropPosition.X / 100, 2, 1);
            }
            else if (sender is Border border2 && border2.Name == "Track2")
            {
                viewModel.AddClip(asset.Name, "", "", dropPosition.X,dropPosition.X / 100, 2, 2);
            }
            else if (sender is Border border3 && border3.Name == "Track3")
            {
                viewModel.AddClip(asset.Name, "", "", dropPosition.X,dropPosition.X / 100, 2, 2);
            }
        }

        // �^�C�����C����̃N���b�v���ړ��������ꍇ
        if (e.Data.Get(dragDataFormat) is ClipViewModel currClip && sender is Control trackclip)
        {
            var dropPosition = e.GetPosition(trackclip);

            // 3. UI�𒼐ڑ��삹���AViewModel�ɃN���b�v�̒ǉ����u�˗��v����
            viewModel.SetClipPositionX(currClip, dropPosition.X);
        }
    }

    private void Track_DragOver(object? sender, DragEventArgs e)
    {
        if (DataContext is TimelineViewModel vm && vm.SelectedClip is ClipViewModel cvm && sender is Border track )
        {
            Dispatcher.UIThread.RunJobs(DispatcherPriority.Input);
            cvm.ClipItemPositionX = e.GetPosition(track).X;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                cvm.ClipItemPositionX = e.GetPosition(track).X;
            },DispatcherPriority.Input);
        }
    }
    private void Track_DragOverByPointer(object? sender, PointerEventArgs e)
    {
        if (DataContext is TimelineViewModel vm && vm.SelectedClip is ClipViewModel cvm && sender is Border track )
        {
            Dispatcher.UIThread.RunJobs(DispatcherPriority.Input);
            cvm.ClipItemPositionX = e.GetPosition(track).X;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                cvm.ClipItemPositionX = e.GetPosition(track).X;
            },DispatcherPriority.MaxValue);
        }
    }
}