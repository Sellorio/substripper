using Sellorio.Substripper.Models.Subtitles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    internal interface ISubtitleInfoService
    {
        Task<IList<SubtitleInfo>> GetSubtitleInfoAsync(string filename);
    }
}
