namespace AuthoringToolBeta.Model
{
    public class ClipModel
    {
        public string AssetName { get; set; }
        public string AssetPath { get; set; }
        public string AssetType { get; set; } 
        public double StartTime { get; set; }
        public double Duration { get; set; }
        

        public ClipModel(string assetName, string assetPath, string assetType, double startTime, double duration)
        {
            AssetName = assetName;
            AssetPath = assetPath;
            AssetType = assetType;
            StartTime = startTime;
            Duration = duration;
        }
    }
}