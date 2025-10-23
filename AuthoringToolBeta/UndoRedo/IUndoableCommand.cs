namespace AuthoringToolBeta.UndoRedo;

public interface IUndoableCommand
{
    void Execute();
    void Unexecute();
}