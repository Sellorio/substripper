using System.Collections.Generic;

namespace Sellorio.Substripper.Models
{
    public class MediaFileHistoryItem
    {
        public string Filename { get; set; }
        public IList<Language> RemovedLanguages { get; set; }
        public IList<int> DeforcedSubtitleIndexes { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
