using AuthoringToolBeta.Commands;
using AuthoringToolBeta.Model;
using AuthoringToolBeta.Services;
using AuthoringToolBeta.Views;
using Avalonia.Controls;
using DynamicData.Tests;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AuthoringToolBeta.ViewModels
{
    public class MainWindowViewModel: ReactiveObject
    {
        // UIにリストの変更を自動通知できる特殊なコレクション
        public ObservableCollection<HierarchyModel> Assets { get; }
        private string _test;
        public string Test 
        {
            get => _test; 
            set => this.RaiseAndSetIfChanged(ref _test, value);
        }

        public TimelineViewModel Timeline { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenTestWindow { get; }
        private readonly Window _owner;
        private readonly ProjectService _projectService;

        public MainWindowViewModel()
        {
            Timeline = new TimelineViewModel();

            // 仮のアセットデータを作成
            Assets = new ObservableCollection<HierarchyModel>
            {
                new HierarchyFolderModel(
                    "Assets",
                    "Project/Assets",
                    "Folder",
                    new List<HierarchyModel>()),
                new HierarchyMediaFileModel(
                    "sampleScene1",
                    "Project/Assets/sampleScene1",
                    "movie",
                    "sampleScene1Content"),
                new HierarchyMediaFileModel(
                    "sampleScene2",
                    "Project/Assets/sampleScene2",
                    "movie",
                    "sampleScene2Content"),
                new HierarchyFolderModel(
                    "Sounds",
                    "Project/Sounds",
                    "Folder",
                    new List<HierarchyModel>()),
                new HierarchyMediaFileModel(
                    "sampleSound1",
                    "Project/Assets/sampleSound1",
                    "sound",
                    "sampleSound1Content"),
                new HierarchyMediaFileModel(
                    "sampleSound2",
                    "Project/Assets/sampleSound2",
                    "sound",
                    "sampleSound2Content"),
            };
        }

        public MainWindowViewModel(Avalonia.Controls.Window owner)
        {
            _owner = owner;
            _projectService = new ProjectService();
            Timeline = new TimelineViewModel();
            Assets = new ObservableCollection<HierarchyModel>
            {
                new HierarchyFolderModel(
                    "Assets",
                    "Project/Assets",
                    "Folder",
                    new List<HierarchyModel>()),
                new HierarchyMediaFileModel(
                    "sampleScene1",
                    "Project/Assets/sampleScene1",
                    "movie",
                    "sampleScene1Content"),
                new HierarchyMediaFileModel(
                    "sampleScene2",
                    "Project/Assets/sampleScene2",
                    "movie",
                    "sampleScene2Content"),
                new HierarchyFolderModel(
                    "Sounds",
                    "Project/Sounds",
                    "Folder",
                    new List<HierarchyModel>()),
                new HierarchyMediaFileModel(
                    "sampleSound1",
                    "Project/Assets/sampleSound1",
                    "sound",
                    "sampleSound1Content"),
                new HierarchyMediaFileModel(
                    "sampleSound2",
                    "Project/Assets/sampleSound2",
                    "sound",
                    "sampleSound2Content"),
            };
            // ...
            SaveCommand = new MyAsyncRelayCommand(_ => ExecuteSave());
            OpenCommand = new MyAsyncRelayCommand(_ => ExecuteOpen());
            //OpenTestWindow = ReactiveCommand.Create(() =>
            //{
            //    try
            //    {
            //        var testWindowVM = new TestWindowViewModel();
            //        var testWindow = new TestView {DataContext = testWindowVM};
            //        testWindow.Show();
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"ウィンドウ起動エラー: {ex.Message}");
            //    }
            //},outputScheduler: RxApp.MainThreadScheduler);

        }

        private async Task ExecuteSave()
        {
            await _projectService.SaveProjectAsync(this.Timeline, _owner.StorageProvider);
        }

        private async Task ExecuteOpen()
        {
            await _projectService.LoadProjectAsync(this.Timeline, _owner.StorageProvider);
        }

        public void exchangePosHierarchyItem(ObservableCollection<HierarchyModel> HierarchyInp,HierarchyModel exchangeFromHM, HierarchyModel exchangeToHM)
        {
            int exchangeFromIdx = HierarchyInp.IndexOf(exchangeFromHM);
            
            int exchangeToIdx = HierarchyInp.IndexOf(exchangeToHM);
            HierarchyInp[exchangeFromIdx] = exchangeToHM;
            HierarchyInp[exchangeToIdx] = exchangeFromHM;
        
        }
    }
}
