using AuthoringToolBeta.ViewModels;

namespace AuthoringToolBeta.UndoRedo
{
    public class AddClipCommand : IUndoableCommand
    {
        private readonly TrackViewModel _targetTrack;
        private readonly ClipViewModel _clipToAdd;

        public AddClipCommand(TrackViewModel targetTrack, ClipViewModel clipToAdd)
        {
            _targetTrack = targetTrack;
            _clipToAdd = clipToAdd;
        }

        public void Execute() => _targetTrack.Clips.Add(_clipToAdd);
        public void Unexecute() => _targetTrack.Clips.Remove(_clipToAdd);
    }
}