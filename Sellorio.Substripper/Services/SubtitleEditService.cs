using Sellorio.Substripper.Models;
using Sellorio.Substripper.Models.Subtitles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    internal class SubtitleEditService : ISubtitleEditService
    {
        public async Task EditSubtitleTracksAsync(string mediaFile, IList<SubtitleInfo> tracksToInclude, IList<SubtitleDispositionEdit> dispositionChanges = null)
        {
            var commandParameters = new StringBuilder(500);
            commandParameters.Append("-i ").Append('"').Append(mediaFile).Append('"').Append(" -map 0:v -map 0:a");

            foreach (var track in tracksToInclude)
            {
                commandParameters.Append(" -map 0:s:").Append(track.SubtitleIndex);
            }

            commandParameters.Append(" -c copy");

            foreach (var dispositionChange in dispositionChanges)
            {
                if (tracksToInclude.Any(x => x.SubtitleIndex == dispositionChange.SourceSubtitleIndex))
                {
                    var dispositionString = dispositionChange.NewDisposition switch
                    {
                        Disposition.None => "0",
                        Disposition.Default => "default",
                        Disposition.Forced => "forced",
                        _ => throw new NotSupportedException()
                    };

                    commandParameters.Append(" -disposition:s:").Append(dispositionChange.SourceSubtitleIndex).Append(' ').Append(dispositionString);
                }
            }

            var temporaryOutputFilename = GetTemporaryOutputFilename(mediaFile);
            var backupFilename = GetBackupFilename(mediaFile);

            commandParameters.Append(' ').Append('"').Append(temporaryOutputFilename).Append('"');

            var processStartInfo = new ProcessStartInfo("ffmpeg", commandParameters.ToString())
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            var process = Process.Start(processStartInfo)!;
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var errorText = await process.StandardError.ReadToEndAsync();

                Console.Error.WriteLine(errorText);

                try
                {
                    File.Delete(temporaryOutputFilename);
                }
                catch { }

                throw new InvalidOperationException("Failed to edit subtitles:\r\n" + errorText);
            }

            await PublishProcessedFileAsync(mediaFile, temporaryOutputFilename, backupFilename);
        }

        private static Task PublishProcessedFileAsync(string mediaFile, string temporaryOutputFilename, string backupFilename)
        {
            File.Move(mediaFile, backupFilename);

            try
            {
                File.Move(temporaryOutputFilename, mediaFile);
            }
            catch
            {
                File.Move(backupFilename, mediaFile);

                try
                {
                    File.Delete(temporaryOutputFilename);
                }
                catch
                {
                }

                throw;
            }

            File.Delete(backupFilename);

            return Task.CompletedTask;
        }

        private static string GetTemporaryOutputFilename(string mediaFile)
        {
            return Path.GetTempPath() + Guid.NewGuid().ToString() + Path.GetExtension(mediaFile);
        }

        private static string GetBackupFilename(string mediaFile)
        {
            const int MaxPathSegmentLength = 255;
            const string NamePrefix = "old-";

            var mediaFileName = Path.GetFileName(mediaFile);

            return
                Path.Combine(
                    Path.GetDirectoryName(mediaFile),
                    NamePrefix +
                    (mediaFileName.Length >= MaxPathSegmentLength - NamePrefix.Length
                        ? mediaFileName.Substring(0, MaxPathSegmentLength - NamePrefix.Length)
                        : mediaFileName));
        }
    }
}
