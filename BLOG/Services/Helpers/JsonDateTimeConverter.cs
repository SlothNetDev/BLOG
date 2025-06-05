using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BLOG.Services.Helpers
{
    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format;

        public JsonDateTimeConverter(string format = "yyyy-MM-ddTHH:mm:ssZ")
        {
            _format = format;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString()!, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format, CultureInfo.InvariantCulture));
        }
    }
}
