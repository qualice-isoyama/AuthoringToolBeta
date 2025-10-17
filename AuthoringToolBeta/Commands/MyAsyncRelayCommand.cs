using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AuthoringToolBeta.Commands;

public class MyAsyncRelayCommand : ICommand
{
    private readonly Func<object?, Task> _execute;
    private readonly Predicate<object?>? _canExecute;
    private bool _isExecuting;

    public MyAsyncRelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        // コマンドが実行中でない、かつ、指定された実行可否条件を満たす場合のみ実行可能
        return !_isExecuting && (_canExecute == null || _canExecute(parameter));
    }

    public async void Execute(object? parameter)
    {
        // async void は通常避けるべきですが、イベントハンドラやコマンドのExecuteメソッドは例外的な許可された使用箇所です。
        
        _isExecuting = true;
        RaiseCanExecuteChanged(); // 実行状態が変化したことをUIに通知

        try
        {
            await _execute(parameter);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged(); // 実行状態が変化したことをUIに通知
        }
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        // UIスレッドで実行されることを保証する
        Avalonia.Threading.Dispatcher.UIThread.Post(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
    }
}