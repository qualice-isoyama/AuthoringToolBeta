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
        // 1. このViewのDataContextであるViewModelを取得
        var viewModel = DataContext as TimelineViewModel;
        if (viewModel == null) return;

        // 2. ドロップされたアセット名と位置を取得
        if (e.Data.Get(HierarchyViewModelFormat) is HierarchyModel asset && sender is Control track)
        {
            var dropPosition = e.GetPosition(track);

            if (sender is Border border && border.Name == "Track1")
            {
                // 3. UIを直接操作せず、ViewModelにクリップの追加を「依頼」する
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

        // タイムライン上のクリップを移動させた場合
        if (e.Data.Get(dragDataFormat) is ClipViewModel currClip && sender is Control trackclip)
        {
            var dropPosition = e.GetPosition(trackclip);

            // 3. UIを直接操作せず、ViewModelにクリップの追加を「依頼」する
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