using Sellorio.Substripper.Models;
using Sellorio.Substripper.Models.Subtitles;
using Sellorio.Substripper.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    internal class SubtitleInfoService : ISubtitleInfoService
    {
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public SubtitleInfoService()
        {
            _jsonOptions.Converters.Add(new LanguageJsonConverter());
            _jsonOptions.Converters.Add(new DispositionJsonConverter());
        }

        public async Task<IList<SubtitleInfo>> GetSubtitleInfoAsync(string filename)
        {
            var processStartInfo = new ProcessStartInfo("ffprobe", $"-select_streams s -show_entries stream -of json \"{filename}\"")
            {
                RedirectStandardOutput = true
            };

            var process = Process.Start(processStartInfo)!;
            await process.WaitForExitAsync();

            var outputJson = await process.StandardOutput.ReadToEndAsync();

            var data = JsonSerializer.Deserialize<StreamProbeWrapper<SubtitleInfo>>(outputJson, _jsonOptions);

            for (var i = 0; i < data.Streams.Count; i++)
            {
                data.Streams[i].SubtitleIndex = i;
            }

            return data!.Streams!;
        }
    }
}
