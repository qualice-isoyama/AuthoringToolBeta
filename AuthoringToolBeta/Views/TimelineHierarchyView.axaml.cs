using System;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using AuthoringToolBeta.UndoRedo;
using AuthoringToolBeta.ViewModels;

namespace AuthoringToolBeta.Views;

public partial class TimelineHierarchyView : UserControl
{
    public TimelineHierarchyView()
    {
        InitializeComponent();
    }
}