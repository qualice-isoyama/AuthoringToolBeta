using System;
using System.Reactive;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace AuthoringToolBeta.ViewModels;

public class TimeMarkerViewModel:ReactiveObject
{
    public TimeSpan Time { get; set; }
    private double _scaleInterval;

    public double ScaleInterval
    {
        get => _scaleInterval;
        set => this.RaiseAndSetIfChanged(ref _scaleInterval, value);
    }
    public int Scaleint => (int)ScaleInterval;
    public string FormattedTime => Time.ToString(@"hh\:mm\:ss");

    public TimeMarkerViewModel(TimeSpan time, double scaleInterval)
    {
        Time = time;
        ScaleInterval = scaleInterval;
    }
}