using Sellorio.Substripper.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sellorio.Substripper.Utils
{
    public class LanguageJsonConverter : JsonConverter<Language>
    {
        public override Language Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            return value switch
            {
                "en" or "eng" => Language.English,
                "ja" or "jpn" => Language.Japanese,
                "fr" or "fra" or "fre" => Language.French,
                "ita" => Language.Italian,
                "ger" => Language.German,
                "rus" => Language.Russian,
                "spa" => Language.Spanish,
                "por" => Language.Portuguese,
                "may" => Language.Malaysian,
                "id" or "ind" => Language.Indonesian,
                "vie" => Language.Vietnamese,
                "tha" => Language.Thai,
                "zh" or "chi" or "zho" => Language.Chinese,
                "ara" => Language.Arabic,
                "cze" => Language.Czech,
                "dan" => Language.Danish,
                "gre" => Language.Greek,
                "fin" => Language.Finnish,
                "hun" => Language.Hungarian,
                "kor" => Language.Korean,
                "dut" => Language.Dutch,
                "nor" => Language.Norwegian,
                "pol" => Language.Polish,
                "rum" => Language.Romanian,
                "slo" => Language.Slovenian,
                "swe" => Language.Swedish,
                _ => Language.Other,
            };
        }

        public override void Write(Utf8JsonWriter writer, Language value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
