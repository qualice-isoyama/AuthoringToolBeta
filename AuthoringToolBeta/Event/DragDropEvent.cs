using AuthoringToolBeta.Model;
using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.ViewModels;

namespace AuthoringToolBeta.Event;

public class DragDropEvent
{
    public async void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // DraggedObject.GetType().FullName を使用して、フォーマット文字列を一意にします。
        const string dragDataFormat = "AuthoringToolBeta.ViewModels.ClipViewModel";
        // アセットヒエラルキー（テキストブロック）からの操作の場合
        if (sender is TextBlock textBlock)
        {
            // TextBlockに表示されているアセット名（データ）を取得
            var assetName = textBlock.DataContext as string;
            if (assetName != null)
            {
                // ドラッグ＆ドロップ操作を開始
                var dragData = new DataObject();
                dragData.Set(DataFormats.Text, assetName); // ドラッグするデータを設定
                var dragDataTest = new DataObject();
                dragDataTest.Set(dragDataFormat, assetName);

                // DoDragDrop操作を開始
                await DragDrop.DoDragDrop(e, dragDataTest, DragDropEffects.Copy);
            }
        }
    }
}