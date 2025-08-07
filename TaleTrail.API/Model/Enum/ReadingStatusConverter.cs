using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaleTrail.API.Model;

public class ReadingStatusConverter : JsonConverter<ReadingStatus>
{
    public override ReadingStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            // Handle integer input
            var intValue = reader.GetInt32();
            return (ReadingStatus)intValue;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            return stringValue?.ToLower() switch
            {
                "to_read" => ReadingStatus.ToRead,
                "in_progress" => ReadingStatus.InProgress,
                "completed" => ReadingStatus.Completed,
                "dropped" => ReadingStatus.Dropped,
                _ => throw new JsonException($"Unable to convert \"{stringValue}\" to ReadingStatus.")
            };
        }

        throw new JsonException("Expected number or string for ReadingStatus.");
    }

    public override void Write(Utf8JsonWriter writer, ReadingStatus value, JsonSerializerOptions options)
    {
        // Always write as string for database compatibility
        var stringValue = value switch
        {
            ReadingStatus.ToRead => "to_read",
            ReadingStatus.InProgress => "in_progress",
            ReadingStatus.Completed => "completed",
            ReadingStatus.Dropped => "dropped",
            _ => throw new JsonException($"Unable to convert ReadingStatus {value} to string.")
        };

        writer.WriteStringValue(stringValue);
    }
}