using System.Diagnostics;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.ViewModels;

namespace AuthoringToolBeta.Views;

public partial class ClipView : UserControl
{
    private bool _isDragging = false;
    private Point _dragStartPoint;
    private double _originalPositionX;
    public ClipView()
    {
        InitializeComponent();
        // このコントロールでポインタイベントを購読する
        //this.PointerPressed += Clip_OnPointerPressed;
        //this.PointerMoved += Clip_OnPointerMoved;
        //this.PointerReleased += Clip_OnPointerReleased;
    }
    private async void Clip_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // DraggedObject.GetType().FullName を使用して、フォーマット文字列を一意にします。
        const string dragDataFormat = "AuthoringToolBeta.ViewModels.ClipViewModel";
        // アセットヒエラルキー（テキストブロック）からの操作の場合
        if (sender is TextBlock textBlock)
        {
            // 左クリックでのみドラッグ開始
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                _isDragging = true;
                _dragStartPoint = e.GetPosition(this.Parent as Visual); // 親(Canvas)からの相対位置
                
                if (this.DataContext is ClipViewModel vm)
                {
                    var test1 = this;
                    var test2 = DataContext;
                    var test3 = vm.ParentViewModel;
                    // まず「選択」コマンドを実行する
                    //vm.SelectCommand.Execute(DataContext);
                    vm.ParentViewModel.SelectedClip = vm;
                    _originalPositionX = vm.ClipItemPositionX; // ドラッグ開始時のX座標を記憶
                }
                e.Pointer.Capture(this); // このコントロールがマウスイベントを独占する
                e.Handled = true; // イベントを処理済みとする
            }
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
    private void Clip_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // 左クリックでのみドラッグ開始
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            _dragStartPoint = e.GetPosition(this.Parent as Visual); // 親(Canvas)からの相対位置
                
            if (this.DataContext is ClipViewModel vm)
            {
                // まず「選択」コマンドを実行する
                //vm.SelectCommand.Execute(this.DataContext);
                _originalPositionX = vm.ClipItemPositionX; // ドラッグ開始時のX座標を記憶
            }
            e.Pointer.Capture(this); // このコントロールがマウスイベントを独占する
            e.Handled = true; // イベントを処理済みとする
        }
    }

    private void Clip_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            Debug.WriteLine("Dragging clip");
            var test1 = this.DataContext is ClipViewModel;
            var tmp1 = this.DataContext as ClipViewModel;
            var test2 = tmp1.ParentViewModel is TimelineViewModel;
            var tmp2 = tmp1.ParentViewModel as TimelineViewModel;
            Point currentPoint = e.GetPosition(this.Parent as Visual);
            double deltaX = currentPoint.X - _dragStartPoint.X;
        
            // ピクセル単位の移動量を、Scaleで割って時間単位の移動量に変換
            // double deltaTime = deltaX / tmp2.Scale; 
        
            tmp1.ClipItemPositionX = _originalPositionX + deltaX; // StartTime を更新
        }
        if (_isDragging && this.DataContext is ClipViewModel vm && 
            vm.ParentViewModel is TimelineViewModel tvm) // 親のViewModelを取得 (※後述)
        {
            Point currentPoint = e.GetPosition(this.Parent as Visual);
            double deltaX = currentPoint.X - _dragStartPoint.X;
        
            // ピクセル単位の移動量を、Scaleで割って時間単位の移動量に変換
            double deltaTime = deltaX / tvm.Scale; 
        
            vm.ClipItemPositionX = _originalPositionX + deltaTime; // StartTime を更新
        }
    }

    private void Clip_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null); // マウスイベントの独占を解除
            e.Handled = true;
        }
    }
}