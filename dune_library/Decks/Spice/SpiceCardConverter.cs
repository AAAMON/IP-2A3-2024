using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using dune_library.Decks.Spice;

public class SpiceCardConverter : JsonConverter<Spice_Card>
{
    public override Spice_Card Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            if (doc.RootElement.TryGetProperty("type", out JsonElement typeElement))
            {
                string type = typeElement.GetString();
                switch (type)
                {
                    case "Territory_Card":
                        return doc.RootElement.Deserialize<Territory_Card>(options);
                    case "Shai_Hulud_Card":
                        return doc.RootElement.Deserialize<Shai_Hulud_Card>(options);
                }
            }
        }
        throw new JsonException("Unknown type");
    }

    public override void Write(Utf8JsonWriter writer, Spice_Card value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("type", value.GetType().Name);

        foreach (var property in value.GetType().GetProperties())
        {
            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, property.GetValue(value), options);
        }

        writer.WriteEndObject();
    }
}
