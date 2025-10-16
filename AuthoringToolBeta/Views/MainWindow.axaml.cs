using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.ViewModels;

namespace AuthoringToolBeta.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // ViewModelのインスタンスを作成し、DataContextに設定
        this.DataContext = new MainWindowViewModel();
    }
    private async void Asset_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // イベントの発生源（TextBlock）を取得
        if (sender is TextBlock textBlock)
        {
            // TextBlockに表示されているアセット名（データ）を取得
            var assetName = textBlock.DataContext as string;
            if (assetName != null)
            {
                // ドラッグ＆ドロップ操作を開始
                var dragData = new DataObject();
                dragData.Set(DataFormats.Text, assetName); // ドラッグするデータを設定

                // DoDragDrop操作を開始
                await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Copy);
            }
        }
    }
}