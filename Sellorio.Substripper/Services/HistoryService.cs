using Sellorio.Substripper.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sellorio.Substripper.Services
{
    internal class HistoryService : IHistoryService
    {
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
        private List<MediaFileHistoryItem> _history;

        public HistoryService()
        {
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<bool> IsMediaFileAlreadyProcessed(string mediaFile)
        {
            await LoadHistoryAsync();
            return _history.Any(x => x.Filename == mediaFile);
        }

        public async Task AddFailureAsync(string mediaFile, string error)
        {
            _history.Add(new()
            {
                Filename = mediaFile,
                Message = error,
                Success = false
            });

            await SaveHistoryAsync();
        }

        public async Task AddSucceessAsync(string mediaFile, IList<Language> removedLanguages, IList<int> deforcedSubtitleIndexes, string message)
        {
            _history.Add(new()
            {
                Filename = mediaFile,
                RemovedLanguages = removedLanguages,
                DeforcedSubtitleIndexes = deforcedSubtitleIndexes,
                Message = message,
                Success = true
            });

            await SaveHistoryAsync();
        }

        private async Task LoadHistoryAsync()
        {
            if (_history != null)
            {
                return;
            }

            if (File.Exists(Constants.HistoryPath))
            {
                var json = await File.ReadAllTextAsync(Constants.HistoryPath);
                _history = JsonSerializer.Deserialize<List<MediaFileHistoryItem>>(json, _jsonOptions);
            }
            else
            {
                _history = [];
            }
        }

        private async Task SaveHistoryAsync()
        {
            await LoadHistoryAsync();
            await File.WriteAllTextAsync(Constants.HistoryPath, JsonSerializer.Serialize(_history, _jsonOptions));
        }
    }
}
