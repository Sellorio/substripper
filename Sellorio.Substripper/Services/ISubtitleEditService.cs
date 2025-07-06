using Sellorio.Substripper.Models.Subtitles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    public interface ISubtitleEditService
    {
        Task EditSubtitleTracksAsync(string mediaFile, IList<SubtitleInfo> tracksToInclude, IList<SubtitleDispositionEdit> dispositionChanges = null);
    }
}
