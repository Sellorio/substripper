using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    internal class MonitorService(IHistoryService historyService, ISubtitleProcessingService processingService) : IMonitorService
    {
        public async Task MonitorAsync()
        {
            while (true)
            {
                var allMediaFiles =
                    Directory.GetFiles(Constants.MediaFilePath, "*.*", SearchOption.AllDirectories)
                        .Where(x => x.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) || x.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                foreach (var mediaFile in allMediaFiles)
                {
                    if (await historyService.IsMediaFileAlreadyProcessed(mediaFile))
                    {
                        continue;
                    }

                    Console.WriteLine("Processing " + mediaFile + "...");

                    try
                    {
                        await processingService.ProcessMediaFileAsync(mediaFile);
                        Console.WriteLine("Processing completed.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Processing failed:\r\n" + ex.ToString());
                    }
                }

                Console.WriteLine("Waiting for new media...");
                await Task.Delay(Constants.PollingRate);
            }
        }
    }
}
