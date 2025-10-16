using Avalonia.Input;
using Avalonia.Controls;

namespace AuthoringToolBeta.Views;

public partial class ClipView : UserControl
{
    public ClipView()
    {
        InitializeComponent();
    }
    private async void Clip_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // DraggedObject.GetType().FullName を使用して、フォーマット文字列を一意にします。
        const string dragDataFormat = "AuthoringToolBeta.ViewModels.ClipViewModel";
        // アセットヒエラルキー（テキストブロック）からの操作の場合
        if (sender is TextBlock textBlock)
        {
            // TextBlockに表示されているアセット名（データ）を取得
            var currAsset = textBlock.DataContext;
            if (currAsset != null)
            {
                // ドラッグ＆ドロップ操作を開始
                var dragDataTest = new DataObject();
                dragDataTest.Set(dragDataFormat, currAsset);

                // DoDragDrop操作を開始
                await DragDrop.DoDragDrop(e, dragDataTest, DragDropEffects.Copy);
            }
        }
    }
}