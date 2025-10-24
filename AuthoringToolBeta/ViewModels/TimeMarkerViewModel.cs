using System;
using System.Reactive;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace AuthoringToolBeta.ViewModels;

public class TimeMarkerViewModel:ReactiveObject
{
    // 修正前
    public TimeSpan Time { get; set; }
    private double _scaleInterval;
    public double ScaleInterval
    {
        get => _scaleInterval;
        set => this.RaiseAndSetIfChanged(ref _scaleInterval, value);
    }
    public int Scaleint => (int)ScaleInterval;
    public string FormattedTime => Time.ToString(@"hh\:mm\:ss");
    
    // 修正後
    // 目盛りの時間（秒）
    public double TimeInSeconds;
    // 表示するラベル ("00:05", "00:10" など)
    public string Label { get; set; }
    // UI上のX座標（ピクセル）
    public double PositionX  { get; set; }
    // 目盛りの種類(True: ラベル付きの太い線, False: 線のみ)
    public bool IsMajorTick { get; set; }

    

    /*
    public TimeMarkerViewModel(TimeSpan time, double scaleInterval)
    {
        // 修正前
        Time = time;
        ScaleInterval = scaleInterval;
        
    }
    */
    
    public TimeMarkerViewModel(double time, string label, double position, bool isMajor)
    {
        TimeInSeconds = time;
        Label = label;
        PositionX = position;
        IsMajorTick = isMajor;
    }
}