using Sellorio.Substripper.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Sellorio.Substripper.Models.Audio;
using Sellorio.Substripper.Utils;

namespace Sellorio.Substripper.Services
{
    internal class AudioInfoService : IAudioInfoService
    {
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public AudioInfoService()
        {
            _jsonOptions.Converters.Add(new LanguageJsonConverter());
            _jsonOptions.Converters.Add(new DispositionJsonConverter());
        }

        public async Task<IList<AudioInfo>> GetAudioInfoAsync(string filename)
        {
            var processStartInfo = new ProcessStartInfo("ffprobe", $"-select_streams a -show_entries stream -of json \"{filename}\"")
            {
                RedirectStandardOutput = true
            };

            var process = Process.Start(processStartInfo)!;
            await process.WaitForExitAsync();

            var outputJson = await process.StandardOutput.ReadToEndAsync();

            var data = JsonSerializer.Deserialize<StreamProbeWrapper<AudioInfo>>(outputJson, _jsonOptions);

            for (var i = 0; i < data.Streams.Count; i++)
            {
                data.Streams[i].AudioIndex = i;
            }

            return data!.Streams!;
        }
    }
}
