using Avalonia.Controls;
using AuthoringToolBeta.Model;
using Avalonia.Input;
using AuthoringToolBeta.ViewModels;

namespace AuthoringToolBeta;

public partial class HierarchyView : UserControl
{
    const string HierarchyViewModelFormat = "AuthoringToolBeta.ViewModels.HierarchyViewModel";
    public HierarchyView()
    {
        InitializeComponent();
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
        // ドラッグされているデータがテキスト形式かを確認
        if (e.Data.Contains(DataFormats.Text))
        {
            // 受け入れ可能であることをカーソル形状で示す
            e.DragEffects = DragDropEffects.Copy;
        }
        else if (e.Data.Contains(HierarchyViewModelFormat))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            // 受け入れ不可であることを示す
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void Asset_Track_Drop(object? sender, DragEventArgs e)
    {
        // DraggedObject.GetType().FullName を使用して、フォーマット文字列を一意にします。
        
        // 1. このViewのDataContextであるViewModelを取得
        var viewModel = this.DataContext as MainWindowViewModel;
        if (viewModel == null) return;
        
        // このオブジェクトの上でドロップされたとき
        if (e.Data.Get(HierarchyViewModelFormat) is HierarchyItemViewModel movefrom && sender is TextBlock textBlock)
        {
            // 位置入れ替え処理
            var tmpSender = sender as TextBlock;
            var moveTo = tmpSender.DataContext as HierarchyItemViewModel;
            viewModel.Test =  movefrom.HModel.Name + "\n" + moveTo.HModel.Name;
            viewModel.exchangePosHierarchyItem(viewModel.Assets, movefrom, moveTo);
        }
    }
}