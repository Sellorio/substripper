using Sellorio.Substripper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    public interface IHistoryService
    {
        Task AddFailureAsync(string mediaFile, string error);
        Task AddSucceessAsync(string mediaFile, IList<Language> removedLanguages, IList<int> deforcedSubtitleIndexes, string message);
        Task<bool> IsMediaFileAlreadyProcessed(string mediaFile);
    }
}
