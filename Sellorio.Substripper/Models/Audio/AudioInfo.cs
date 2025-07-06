using System.Text.Json.Serialization;

namespace Sellorio.Substripper.Models.Audio
{
    public class AudioInfo
    {
        public int Index { get; set; }

        public int AudioIndex { get; set; }

        [JsonPropertyName("codec_name")]
        public string Codec { get; set; }

        public Disposition Disposition { get; set; }

        public AudioTagInfo Tags { get; set; }
    }
}
