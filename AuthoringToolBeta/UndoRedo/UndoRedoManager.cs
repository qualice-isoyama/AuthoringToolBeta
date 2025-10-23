using System.Windows.Input; // ICommandのため
using AuthoringToolBeta.Commands; // RelayCommandのため
using AuthoringToolBeta.Common; // ViewModelBaseのため
using System.Collections.Generic;

namespace AuthoringToolBeta.UndoRedo
{
    public class UndoRedoManager : ViewModelBase
    {
        private readonly Stack<IUndoableCommand> _undoStack = new();
        private readonly Stack<IUndoableCommand> _redoStack = new();

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public UndoRedoManager()
        {
            UndoCommand = new RelayCommand(_ => Undo(), _ => CanUndo);
            RedoCommand = new RelayCommand(_ => Redo(), _ => CanRedo);
        }

        private void UpdateCanExecute()
        {
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            // RelayCommandに状態変化を通知する
            (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RedoCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
        public void Do(IUndoableCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear(); // 新しい操作をしたらRedoスタックはクリア
            UpdateCanExecute();
        }

        public void Undo()
        {
            if (CanUndo)
            {
                var command = _undoStack.Pop();
                command.Unexecute();
                _redoStack.Push(command);
                UpdateCanExecute();
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
                UpdateCanExecute();
            }
        }
    }
}