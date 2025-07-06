using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    public interface ISubtitleProcessingService
    {
        Task ProcessMediaFileAsync(string mediaFile);
    }
}
