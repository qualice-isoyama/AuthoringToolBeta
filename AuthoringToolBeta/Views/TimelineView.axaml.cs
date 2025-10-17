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
        // ドラッグされているデータがテキスト形式かを確認
        if (e.Data.Contains(HierarchyViewModelFormat))
        {
            // 受け入れ可能であることをカーソル形状で示す
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            // 受け入れ不可であることを示す
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void Track_Drop(object? sender, DragEventArgs e)
    {
        // DraggedObject.GetType().FullName を使用して、フォーマット文字列を一意にします。
        const string dragDataFormat = "AuthoringToolBeta.ViewModels.ClipViewModel";
        // 1. このViewのDataContextであるViewModelを取得
        var viewModel = this.DataContext as TimelineViewModel;
        if (viewModel == null) return;

        // 2. ドロップされたアセット名と位置を取得
        if (e.Data.Get(HierarchyViewModelFormat) is HierarchyModel asset && sender is Control track)
        {
            var dropPosition = e.GetPosition(track);

            // 3. UIを直接操作せず、ViewModelにクリップの追加を「依頼」する
            viewModel.AddClip(asset.Name, "","",dropPosition.X);
        }

        // タイムライン上のクリップを移動させた場合
        if (e.Data.Get(dragDataFormat) is ClipViewModel currClip && sender is Control trackclip)
        {
            var dropPosition = e.GetPosition(trackclip);

            // 3. UIを直接操作せず、ViewModelにクリップの追加を「依頼」する
            viewModel.SetClipPositionX(currClip, dropPosition.X);
        }
    }
}