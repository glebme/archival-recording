using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.JsonConverters;

public class DescriptionEnumConverter<T> : JsonConverter<T> where T : Enum
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        foreach (var field in typeToConvert.GetFields())
        {
            if (field.GetCustomAttribute<DescriptionAttribute>()?.Description == value)
            {
                return (T?)field.GetValue(null);
            }
        }
        
        throw new JsonException($"Unable to convert \"{value}\" to Enum \"{typeToConvert}\". No matching Description attribute found.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var field = typeof(T).GetField(value.ToString());
        var description =  field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
        writer.WriteStringValue(description ?? value.ToString());
    }
}
