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

            commandParameters.Append(' ').Append('"').Append(GetTemporaryOutputFilename(mediaFile)).Append('"');

            var processStartInfo = new ProcessStartInfo("ffmpeg", commandParameters.ToString())
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            var process = Process.Start(processStartInfo)!;
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine(await process.StandardError.ReadToEndAsync());

                try
                {
                    File.Delete(GetTemporaryOutputFilename(mediaFile));
                }
                catch { }

                throw new InvalidOperationException("Failed to edit subtitles.");
            }

            File.Move(mediaFile, GetBackupFilename(mediaFile));

            try
            {
                File.Move(GetTemporaryOutputFilename(mediaFile), mediaFile);
            }
            catch
            {
                File.Move(GetBackupFilename(mediaFile), mediaFile);
                throw;
            }

            File.Delete(GetBackupFilename(mediaFile));
        }

        private static string GetTemporaryOutputFilename(string mediaFile)
        {
            return Path.Combine(Path.GetDirectoryName(mediaFile), "edit-" + Path.GetFileName(mediaFile));
        }

        private static string GetBackupFilename(string mediaFile)
        {
            return Path.Combine(Path.GetDirectoryName(mediaFile), "old-" + Path.GetFileName(mediaFile));
        }
    }
}
