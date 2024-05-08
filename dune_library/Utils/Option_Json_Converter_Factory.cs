using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public static class Helper {
    internal static bool Is_Generic_Type(this Type t, Type genericTypeDefinition) =>
      t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition;
  }

  /// <summary>
  /// <para>
  /// serializer + deserializer for Option&lt;THIS_TYPE&gt as arrays of either 0 (none) or 1 (some) elements;
  /// </para>
  /// </summary>
  public class Option_Json_Converter_Factory : JsonConverterFactory {
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
      var innerType = typeToConvert.GetGenericArguments().Match(
          () => throw new Exception("no generic type argument"),
          x => x,
          (x, xs) => throw new Exception("more than one generic type argument")
      );
      return Activator.CreateInstance(typeof(Option_Json_Converter<>).MakeGenericType(innerType), options) as JsonConverter;
    }

    public override bool CanConvert(Type typeToConvert) => typeToConvert.Is_Generic_Type(typeof(Option<>));

    private class Option_Json_Converter<T> : JsonConverter<Option<T>> {
      private readonly JsonConverter<T> _inner_converter;

      public Option_Json_Converter(JsonSerializerOptions options) =>
        _inner_converter = (JsonConverter<T>)options.GetConverter(typeof(T));

      public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Null) {
          return Option<T>.None;
        }
        if (reader.TokenType != JsonTokenType.StartArray) {
          throw new JsonException($"expected {JsonTokenType.StartArray} but found {reader.TokenType}");
        }
        if (!reader.Read()) {
          throw new JsonException($"could not read element in array or {JsonTokenType.EndArray}");
        }
        if (reader.TokenType == JsonTokenType.EndArray) {
          // empty array
          return Option<T>.None;
        }
        // non-empty-array
        var value = _inner_converter.Read(ref reader, typeof(T), options)!;
        if (!reader.Read()) {
          throw new JsonException("could not read next token");
        }
        if (reader.TokenType != JsonTokenType.EndArray) {
          throw new JsonException($"expected token {JsonTokenType.EndArray} but found {reader.TokenType}");
        }
        return Option<T>.Some(value);
      }

      public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options) {
        writer.WriteStartArray();
        value.IfSome(inner_value => _inner_converter.Write(writer, inner_value, options));
        writer.WriteEndArray();
      }
    }
  }
}
