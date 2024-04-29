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
  /// serialize Option&lt;T&gt; like T? 
  /// </summary>
  /// <remarks>Add [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] to the property to disable writing 'null' for None into json</remarks>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class Option_As_Nullable_Json_Converter_Attribute : JsonConverterAttribute {
    public override JsonConverter? CreateConverter(Type typeToConvert) =>
        Activator.CreateInstance(typeof(Option_Json_Converter_Factory.Option_As_Nullable_Json_Converter<>)
                 .MakeGenericType(typeToConvert.Is_Generic_Type(typeof(Option<>)) ?
                    typeToConvert.GetGenericArguments().Head() :
                    typeToConvert)) as JsonConverter;
  }

  internal class Option_Json_Converter_Factory : JsonConverterFactory {
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
      // var innerType = typeToConvert.GetSingleGenericTypeArgument();
      var innerType = typeToConvert.GetGenericArguments().Match(
          () => throw new Exception("no generic type argument"),
          x => x,
          (x, xs) => throw new Exception("more than one generic type argument")
      );
      return Activator.CreateInstance(typeof(Option_Json_Converter<>).MakeGenericType(innerType), options) as JsonConverter;
    }

    public override bool CanConvert(Type typeToConvert) => typeToConvert.Is_Generic_Type(typeof(Option<>));

    private class Option_Json_Converter<T> : JsonConverter<Option<T>> {
      private readonly JsonConverter<T> _innerConverter;

      public Option_Json_Converter(JsonSerializerOptions options) =>
        _innerConverter = (JsonConverter<T>)options.GetConverter(typeof(T));

      public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Null) {
          return None;
        } else {
          if (reader.TokenType != JsonTokenType.StartArray) {
            throw new JsonException($"expected {JsonTokenType.StartArray} but found {reader.TokenType}");
          }
          if (!reader.Read()) {
            throw new JsonException($"could not read element in array or {JsonTokenType.EndArray}");
          }
          if (reader.TokenType != JsonTokenType.EndArray) {
            // non-empty-array
            var value = _innerConverter.Read(ref reader, typeof(T), options);
            if (!reader.Read()) {
              throw new JsonException("could not read next token");
            }
            if (reader.TokenType != JsonTokenType.EndArray) {
              throw new JsonException($"expected token {JsonTokenType.EndArray} but found {reader.TokenType}");
            }
            return Option<T>.Some(value!);
          } else {
            return Option<T>.None;
          }
        }
      }

      public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options) {
        writer.WriteStartArray();
        value.IfSome(some => _innerConverter.Write(writer, some, options));
        writer.WriteEndArray();
      }
    }

    internal class Option_As_Nullable_Json_Converter<T> : JsonConverter<Option<T>> {
      public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
              reader.TokenType == JsonTokenType.Null ? None : Some((options.GetConverter(typeof(T?)) as JsonConverter<T> ??
                throw new JsonException($"Could not get converter for {typeof(T)}")).Read(ref reader, typeof(T), options)!);

      public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options) {
        if (value.Case is T some) {
          ((JsonConverter<T>)options.GetConverter(typeof(T))).Write(writer, some, options);
        } else {
          writer.WriteNullValue();
        }
      }
    }
  }
}
