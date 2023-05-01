using System.Text.Json.Serialization;
using System.Text.Json;

namespace IntegrationTests;

public class CustomDecimalConverter : JsonConverter<Decimal>
{
    private readonly string Format;
    public CustomDecimalConverter(string format)
    {
        Format = format;
    }
    public override void Write(Utf8JsonWriter writer, Decimal decimalValue, JsonSerializerOptions options)
    {
        writer.WriteStringValue(decimalValue.ToString(Format));
    }
    public override Decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Decimal.Parse(reader.GetString(), System.Globalization.NumberStyles.AllowDecimalPoint, null);
    }
}