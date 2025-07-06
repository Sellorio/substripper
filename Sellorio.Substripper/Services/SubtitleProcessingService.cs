using Sellorio.Substripper.Models;
using Sellorio.Substripper.Models.Subtitles;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    internal class SubtitleProcessingService(
        IAudioInfoService audioInfoService,
        ISubtitleInfoService subtitleInfoService,
        ISubtitleEditService subtitleEditService,
        IHistoryService historyService)
        : ISubtitleProcessingService
    {
        public async Task ProcessMediaFileAsync(string mediaFile)
        {
            if (!await WaitForFileToBeUnlockedAsync(mediaFile))
            {
                await historyService.AddFailureAsync(mediaFile, "Media file remained locked for too long.");
                return;
            }

            var audioTracks = await audioInfoService.GetAudioInfoAsync(mediaFile);

            if (audioTracks.All(x => x.Tags.Language == Language.English))
            {
                await historyService.AddSucceessAsync(mediaFile, [], [], "Skipping media file since no non-english audio is present.");
                return;
            }

            var subtitles = await subtitleInfoService.GetSubtitleInfoAsync(mediaFile);

            if (subtitles.All(x => Constants.Subtitles.LanguagesToKeep.Contains(x.Tags.Language) && !x.Disposition.HasFlag(Disposition.Forced)))
            {
                await historyService.AddSucceessAsync(mediaFile, [], [], "No languages to remove. No forced tracks to unforce.");
                return;
            }

            var subsToRemove = subtitles.Where(x => !Constants.Subtitles.LanguagesToKeep.Contains(x.Tags.Language)).ToList();
            var subsToKeep = subtitles.Except(subsToRemove).ToList();
            var subsToUnforce = subsToKeep.Where(x => x.Disposition.HasFlag(Disposition.Forced)).ToList();
            var dispositionEdits = subsToKeep.Where(x => x.Disposition.HasFlag(Disposition.Forced)).Select(x => new SubtitleDispositionEdit(x.Index, x.SubtitleIndex, x.Disposition & Disposition.Default)).ToList();

            try
            {
                await subtitleEditService.EditSubtitleTracksAsync(mediaFile, subsToKeep, dispositionEdits);
            }
            catch (Exception ex)
            {
                try
                {
                    await historyService.AddFailureAsync(mediaFile, ex.ToString());
                }
                catch
                {
                    // preserve the original exception
                }

                throw;
            }

            await historyService.AddSucceessAsync(
                mediaFile,
                subsToRemove.Select(x => x.Tags.Language).Distinct().ToList(),
                subsToUnforce.Select(x => subsToKeep.IndexOf(x)).ToList(),
                "Successfully processed.");
        }

        private static async Task<bool> WaitForFileToBeUnlockedAsync(string filename, int retryCount = 10, int retryDelayMs = 20000)
        {
            for (var i = 0; i < retryCount; i++)
            {
                if (IsFileLocked(filename))
                {
                    await Task.Delay(retryDelayMs);
                }
                else
                {
                    return true;
                }
            }

            if (IsFileLocked(filename))
            {
                return false;
            }

            return true;
        }

        private static bool IsFileLocked(string filePath)
        {
            try
            {
                using FileStream stream = new(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // The file is unavailable because it is:
                // still being written to, or being processed by another thread, or does not exist (optional to check).
                return true;
            }

            return false;
        }
    }
}
