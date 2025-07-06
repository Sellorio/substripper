using Microsoft.Extensions.DependencyInjection;
using Sellorio.Substripper.Models;
using Sellorio.Substripper.Models.Subtitles;
using Sellorio.Substripper.Services;
using System;
using System.IO;
using System.Linq;

var services = new ServiceCollection();
services.AddSingleton<IAudioInfoService, AudioInfoService>();
services.AddSingleton<ISubtitleInfoService, SubtitleInfoService>();
services.AddSingleton<ISubtitleEditService, SubtitleEditService>();
var serviceProvider = services.BuildServiceProvider();

var mediaFiles =
    Directory.GetFiles("/src", "*.*", SearchOption.AllDirectories)
        .Where(x => x.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) || x.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
        .ToList();

Console.WriteLine();
Console.WriteLine();

var audioInfoService = serviceProvider.GetRequiredService<IAudioInfoService>();
var subtitleInfoService = serviceProvider.GetRequiredService<ISubtitleInfoService>();
var subtitleEditService = serviceProvider.GetRequiredService<ISubtitleEditService>();

foreach (var mediaFile in mediaFiles)
{
    Console.WriteLine();
    Console.WriteLine("Checking " + mediaFile + "...");

    var audioTracks = await audioInfoService.GetAudioInfoAsync(mediaFile);

    if (audioTracks.All(x => x.Tags.Language == Language.English))
    {
        Console.WriteLine("Skipping media file since no non-english audio is present.");
        continue;
    }

    Console.WriteLine("Processing " + mediaFile + "...");
    var subtitles = await subtitleInfoService.GetSubtitleInfoAsync(mediaFile);
    var englishOrJapaneseSubtitles = subtitles.Where(x => x.Tags.Language is Language.English or Language.Japanese).ToList();
    var dispositionEdits = englishOrJapaneseSubtitles.Where(x => x.Disposition.HasFlag(Disposition.Forced)).Select(x => new SubtitleDispositionEdit(x.Index, x.SubtitleIndex, x.Disposition & Disposition.Default)).ToList();

    await subtitleEditService.EditSubtitleTracksAsync(mediaFile, englishOrJapaneseSubtitles, dispositionEdits);
    Console.WriteLine("Successfully processed " + mediaFile + ".");
}

Console.WriteLine("Processing complete!");
