using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.ViewModels;

namespace AuthoringToolBeta.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // ViewModel�̃C���X�^���X���쐬���ADataContext�ɐݒ�
        this.DataContext = new MainWindowViewModel();
    }
    private async void Asset_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // �C�x���g�̔������iTextBlock�j���擾
        if (sender is TextBlock textBlock)
        {
            // TextBlock�ɕ\������Ă���A�Z�b�g���i�f�[�^�j���擾
            var assetName = textBlock.DataContext as string;
            if (assetName != null)
            {
                // �h���b�O���h���b�v������J�n
                var dragData = new DataObject();
                dragData.Set(DataFormats.Text, assetName); // �h���b�O����f�[�^��ݒ�

                // DoDragDrop������J�n
                await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Copy);
            }
        }
    }
}