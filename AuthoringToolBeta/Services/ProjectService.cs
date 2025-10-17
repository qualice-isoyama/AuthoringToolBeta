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

        // 2. ViewModelからModelのリストに変換
        var clipModels = timelineViewModel.Clips.Select(vm => new ClipModel
        {
            AssetName = vm.ClipItemName,
            AssetPath = vm.ClipItemPath,
            AssetType = vm.ClipItemType,
            PositionX = vm.ClipItemPositionX,
            Width = vm.ClipItemWidth,
        }).ToList();
        
        // 3. JSONにシリアライズ
        var jsonString = JsonSerializer.Serialize(clipModels, new JsonSerializerOptions { WriteIndented = true });

        // 4. ファイルに書き込み
        await using var stream = await file.OpenWriteAsync();
        await using var streamWriter = new StreamWriter(stream);
        await streamWriter.WriteAsync(jsonString);
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

        // 4. ViewModelを更新
        timelineViewModel.Clips.Clear(); // 既存のクリップをクリア
        foreach (var model in clipModels)
        {
            timelineViewModel.Clips.Add(new ClipViewModel(model));
        }
    }
}