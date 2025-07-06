using Sellorio.Substripper.Models.Audio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    internal interface IAudioInfoService
    {
        Task<IList<AudioInfo>> GetAudioInfoAsync(string filename);
    }
}
