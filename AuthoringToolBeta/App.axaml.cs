using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AuthoringToolBeta.ViewModels;
using AuthoringToolBeta.Views;

namespace AuthoringToolBeta;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            
            // 1. MainWindowを先に生成し、インスタンスを取得
            var mainWindow = new MainWindow();
            // 2. Windowインスタンスを引数としてViewModelを生成（依存性の注入）
            var viewModel = new MainWindowViewModel(mainWindow);
            // 3. DataContextを設定
            mainWindow.DataContext = viewModel;
            // 4. デスクトップアプリケーションのメインウィンドウとして設定
            desktop.MainWindow = mainWindow;
            
            /*desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };*/
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }

    // Avaloniaのデータアノテーション検証プラグインを無効化
    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}