using AuthoringToolBeta.Model;
using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.ViewModels;

namespace AuthoringToolBeta.Views;

public partial class MainWindow : Window
{
    const string HierarchyViewModelFormat = "AuthoringToolBeta.ViewModels.HierarchyViewModel";
    public MainWindow()
    {
        InitializeComponent();
        // ViewModel�̃C���X�^���X���쐬���ADataContext�ɐݒ�
        this.DataContext = new MainWindowViewModel(this);
    }
    private async void Asset_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock textBlock)
        {
            var hierarchyItem = textBlock.DataContext as HierarchyModel;
            if (hierarchyItem != null)
            {
                var dragData = new DataObject();
                dragData.Set(HierarchyViewModelFormat, hierarchyItem);
                await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Copy);
            }
        }
    }
    private void Asset_Track_DragEnter(object? sender, DragEventArgs e)
    {
        // �h���b�O����Ă���f�[�^���e�L�X�g�`�������m�F
        if (e.Data.Contains(DataFormats.Text))
        {
            // �󂯓���\�ł��邱�Ƃ��J�[�\���`��Ŏ���
            e.DragEffects = DragDropEffects.Copy;
        }
        else if (e.Data.Contains(HierarchyViewModelFormat))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            // �󂯓���s�ł��邱�Ƃ�����
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void Asset_Track_Drop(object? sender, DragEventArgs e)
    {
        // 1. ����View��DataContext�ł���ViewModel���擾
        var viewModel = this.DataContext as MainWindowViewModel;
        if (viewModel == null) return;
        
        // ���̃I�u�W�F�N�g�̏�Ńh���b�v���ꂽ�Ƃ�
        if (e.Data.Get(HierarchyViewModelFormat) is HierarchyModel movefrom && sender is TextBlock textBlock)
        {
            // �ʒu����ւ�����
            var tmpSender = sender as TextBlock;
            var moveTo = tmpSender.DataContext as HierarchyModel;
            viewModel.Test =  movefrom.Name + "\n" + moveTo.Name;
            viewModel.exchangePosHierarchyItem(viewModel.Assets, movefrom, moveTo);
        }
    }
}