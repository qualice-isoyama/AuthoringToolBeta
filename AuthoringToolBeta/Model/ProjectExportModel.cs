using System.Collections.Generic;

namespace AuthoringToolBeta.Model
{
    // 1つのクリップのデータを表すクラス (既存のClipModelを再利用しても良い)
    public class ClipExportModel
    {
        public string AssetName { get; set; }
        public double StartTime { get; set; }
        public double Duration { get; set; }
    }

    // 1つのトラックのデータを表すクラス
    public class TrackExportModel
    {
        public string TrackName { get; set; }
        public List<ClipExportModel> Clips { get; set; } = new();
    }

    // プロジェクト全体のデータを表すクラス
    public class ProjectExportModel
    {
        public double TotalDuration { get; set; }
        public List<TrackExportModel> Tracks { get; set; } = new();
    }
}