using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AuthoringToolBeta.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using AuthoringToolBeta.Model;

namespace AuthoringToolBeta.Views;

public partial class TimelineView : UserControl
{
    const string HierarchyViewModelFormat = "AuthoringToolBeta.ViewModels.HierarchyViewModel";
    public TimelineView()
    {
        //this.DataContext = new TimelineViewModel();
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
        // DraggedObject.GetType().FullName ���g�p���āA�t�H�[�}�b�g���������ӂɂ��܂��B
        const string dragDataFormat = "AuthoringToolBeta.ViewModels.ClipViewModel";
        // 1. ����View��DataContext�ł���ViewModel���擾
        var viewModel = this.DataContext as TimelineViewModel;
        if (viewModel == null) return;

        // 2. �h���b�v���ꂽ�A�Z�b�g���ƈʒu���擾
        if (e.Data.Get(HierarchyViewModelFormat) is HierarchyModel asset && sender is Control track)
        {
            var dropPosition = e.GetPosition(track);

            // 3. UI�𒼐ڑ��삹���AViewModel�ɃN���b�v�̒ǉ����u�˗��v����
            viewModel.AddClip(asset.Name, "","",dropPosition.X);
        }

        // �^�C�����C����̃N���b�v���ړ��������ꍇ
        if (e.Data.Get(dragDataFormat) is ClipViewModel currClip && sender is Control trackclip)
        {
            var dropPosition = e.GetPosition(trackclip);

            // 3. UI�𒼐ڑ��삹���AViewModel�ɃN���b�v�̒ǉ����u�˗��v����
            viewModel.SetClipPositionX(currClip, dropPosition.X);
        }
    }
}