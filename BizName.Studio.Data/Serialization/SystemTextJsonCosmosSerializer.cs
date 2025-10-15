using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Azure.Cosmos;

namespace BizName.Studio.Data.Serialization;

public class SystemTextJsonCosmosSerializer : CosmosSerializer
{
    private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);
    private readonly JsonSerializerOptions _options;

    public SystemTextJsonCosmosSerializer()
    {
        _options = BizNameCosmosSerializationOptions.Default;
    }

    public SystemTextJsonCosmosSerializer(JsonSerializerOptions options)
    {
        _options = options;
    }

    public override T FromStream<T>(Stream stream)
    {
        if (stream.CanSeek && stream.Length == 0)
        {
            return default(T)!;
        }

        if (typeof(Stream).IsAssignableFrom(typeof(T)))
        {
            return (T)(object)stream;
        }

        using var streamReader = new StreamReader(stream);
        var json = streamReader.ReadToEnd();
        return JsonSerializer.Deserialize<T>(json, _options)!;
    }

    public override Stream ToStream<T>(T input)
    {
        var streamPayload = new MemoryStream();

        using (var writer = new Utf8JsonWriter(streamPayload, new JsonWriterOptions
        {
            Indented = false
        }))
        {
            JsonSerializer.Serialize(writer, input, _options);
        }

        streamPayload.Position = 0;
        return streamPayload;
    }
}
