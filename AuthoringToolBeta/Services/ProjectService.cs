using AuthoringToolBeta.Model;
using AuthoringToolBeta.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage; // ファイルダイアログ用

namespace AuthoringToolBeta.Services;

public class ProjectService
{
    public async Task SaveProjectAsync(TimelineViewModel timelineViewModel, IStorageProvider storageProvider)
    {
        // 1. 保存ファイルダイアログを表示
        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Project As...",
            FileTypeChoices = new[] { new FilePickerFileType("Project Files") { Patterns = new[] { "*.json" } } }
        });

        if (file is null) return; // キャンセルされた

        for (int trackIdx = 0; trackIdx < timelineViewModel.Tracks.Count; trackIdx++)
        {
            // 2. ViewModelからModelのリストに変換
            var clipModels = timelineViewModel.Tracks[trackIdx].Clips.Select(vm => vm.ToModel()).ToList();

            // 3. JSONにシリアライズ
            var jsonString = JsonSerializer.Serialize(clipModels, new JsonSerializerOptions { WriteIndented = true });

            // 4. ファイルに書き込み
            await using var stream = await file.OpenWriteAsync();
            await using var streamWriter = new StreamWriter(stream);
            await streamWriter.WriteAsync(jsonString);
        }
    }

    public async Task LoadProjectAsync(TimelineViewModel timelineViewModel, IStorageProvider storageProvider)
    {
        // 1. 開くファイルダイアログを表示
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Project File",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("Project Files") { Patterns = new[] { "*.json" } } }
        });
        
        if (files is not { Count: 1 }) return; // キャンセルされたか、複数選択された

        // 2. ファイルからJSONを読み込み
        await using var stream = await files[0].OpenReadAsync();
        using var streamReader = new StreamReader(stream);
        var jsonString = await streamReader.ReadToEndAsync();

        // 3. JSONからModelのリストにデシリアライズ
        var clipModels = JsonSerializer.Deserialize<List<ClipModel>>(jsonString);
        if (clipModels is null) return;

        foreach (var track in timelineViewModel.Tracks)
        {
            // 4. ViewModelを更新
            track.Clips.Clear(); // 既存のクリップをクリア
            foreach (var model in clipModels)
            {
                track.Clips.Add(new ClipViewModel(model));
            }
        }
    }
    public async Task ExportProjectAsync(TimelineViewModel timelineViewModel, IStorageProvider storageProvider)
    {
        // 1. 保存ファイルダイアログを表示
        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Project to JSON",
            SuggestedFileName = "scene_data.json", // デフォルトのファイル名
            FileTypeChoices = new[] { new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } } }
        });

        if (file is null) return; // キャンセルされた

        // 2. ViewModelからエクスポート用のModelにデータを変換
        var exportModel = new ProjectExportModel
        {
            TotalDuration = 60.0, // 仮の総時間
            Tracks = timelineViewModel.Tracks.Select(trackVm => new TrackExportModel
            {
                TrackName = trackVm.TrackName,
                Clips = trackVm.Clips.Select(clipVm => new ClipExportModel
                {
                    AssetName = clipVm.ClipItemName,
                    StartTime = clipVm.StartTime,
                    Duration = clipVm.Duration
                }).ToList()
            }).ToList()
        };

        // 3. JSONにシリアライズ (読みやすいようにインデントする)
        var jsonString = JsonSerializer.Serialize(exportModel, new JsonSerializerOptions { WriteIndented = true });

        // 4. ファイルに書き込み
        await using var stream = await file.OpenWriteAsync();
        await using var streamWriter = new StreamWriter(stream);
        await streamWriter.WriteAsync(jsonString);
    }
}