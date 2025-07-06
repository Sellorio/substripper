using Sellorio.Substripper.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sellorio.Substripper.Utils
{
    internal class DispositionJsonConverter : JsonConverter<Disposition>
    {
        public override Disposition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var model = JsonSerializer.Deserialize<JsonElement>(ref reader, options);

            var result = Disposition.None;

            foreach (var property in model.EnumerateObject())
            {
                if (property.Value.GetInt32() == 1)
                {
                    if (property.Name == "default")
                    {
                        result |= Disposition.Default;
                    }
                    else if (property.Name == "forced")
                    {
                        result |= Disposition.Forced;
                    }
                    else
                    {
                        result |= Disposition.Other;
                    }
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Disposition value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
