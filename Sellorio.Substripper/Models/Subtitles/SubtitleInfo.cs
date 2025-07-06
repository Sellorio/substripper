using System.Text.Json.Serialization;

namespace Sellorio.Substripper.Models.Subtitles
{
    public class SubtitleInfo
    {
        public int Index { get; set; }

        public int SubtitleIndex { get; set; }

        [JsonPropertyName("codec_name")]
        public string Format { get; set; }

        public Disposition Disposition { get; set; }

        public SubtitleTagInfo Tags { get; set; }
    }
}
