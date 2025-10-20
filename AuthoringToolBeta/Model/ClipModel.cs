namespace AuthoringToolBeta.Model
{
    public class ClipModel
    {
        public string AssetName { get; set; }
        public string AssetPath { get; set; }
        public string AssetType { get; set; } // 動画　|| 音声 || 画像 || プロジェクトファイル
        public double PositionX { get; set; } // タイムライン上のX座標
        public double Width { get; set; }     // クリップの幅（再生時間）
        public double StartTime { get; set; }
        public double Duration { get; set; }
        

        public ClipModel(string assetName, string assetPath, string assetType, double positionX, double width, double startTime, double duration)
        {
            AssetName = assetName;
            AssetPath = assetPath;
            AssetType = assetType;
            PositionX = positionX;
            Width = width;
            StartTime = startTime;
            Duration = duration;
        }
    }
}