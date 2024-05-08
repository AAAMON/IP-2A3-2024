using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using dune_library.Utils;
using LanguageExt;

namespace library_tests.Utils {
  internal static class Helpers {
    internal static JsonSerializerOptions Deserialization_Options() {
      var options = new JsonSerializerOptions();
      options.Converters.Add(new Option_Json_Converter_Factory());
      return options;
    }
    internal static string Serialize<T>(this T obj) => JsonSerializer.Serialize(obj, Deserialization_Options());
    internal static T Deserialize<T>(this string json) => JsonSerializer.Deserialize<T>(json, Deserialization_Options());
    internal static T Serialize_Then_Deserialize<T>(this T obj) => obj.Serialize().Deserialize<T>();
    internal static string Print(this string json) {
      Console.WriteLine(json);
      return json;
    }
  }
  [TestClass]
  public class Optional_Deserialization_Tests {
    public class Type_With_Option {
      public Option<int> Default { get; set; }

      public Option<Option<int>> Nested_Once { get; set; }

      public Option<Option<Option<int>>> Nested_Twice { get; set; }

      public static Type_With_Option None => new Type_With_Option {
        Default = Option<int>.None,
        Nested_Once = Option<Option<int>>.None,
        Nested_Twice = Option<Option<Option<int>>>.None,
      };

      public static Type_With_Option Some(int a) => new Type_With_Option {
        Default = Option<int>.Some(a),
        Nested_Once = Option<Option<int>>.Some(Option<int>.None),
        Nested_Twice = Option<Option<Option<int>>>.Some(Option<Option<int>>.None),
      };

      public static Type_With_Option Some(int a, int b) => new Type_With_Option {
        Default = Option<int>.Some(a),
        Nested_Once = Option<Option<int>>.Some(Option<int>.Some(b)),
        Nested_Twice = Option<Option<Option<int>>>.Some(Option<Option<int>>.Some(Option<int>.None)),
      };

      public static Type_With_Option Some(int a, int b, int c) => new Type_With_Option {
        Default = Option<int>.Some(a),
        Nested_Once = Option<Option<int>>.Some(Option<int>.Some(b)),
        Nested_Twice = Option<Option<Option<int>>>.Some(Option<Option<int>>.Some(Option<int>.Some(c))),
      };
      public override bool Equals(object obj) {
        if(obj is Type_With_Option other) {
          return Default == other.Default && Nested_Once == other.Nested_Once && Nested_Twice == other.Nested_Twice;
        }
        return false;
      }
    }

    #region Identity

    [TestMethod]
    public void Identity_None() {
      var obj = Type_With_Option.None;
      Assert.AreEqual(obj, obj.Serialize()
                              .Print()
                              .Deserialize<Type_With_Option>());
    }

    [TestMethod]
    public void Identity_Some1() {
      var obj = Type_With_Option.Some(10);
      Assert.AreEqual(obj, obj.Serialize()
                              .Print()
                              .Deserialize<Type_With_Option>());
    }

    [TestMethod]
    public void Identity_Some2() {
      var obj = Type_With_Option.Some(10, 41);
      Assert.AreEqual(obj, obj.Serialize()
                              .Print()
                              .Deserialize<Type_With_Option>());
    }

    [TestMethod]
    public void Identity_Some3() {
      var obj = Type_With_Option.Some(10, 41, 32);
      Assert.AreEqual(obj, obj.Serialize()
                              .Print()
                              .Deserialize<Type_With_Option>());
    }

    #endregion

    #region Serialization

    [TestMethod]
    public void Serialization_None() {
      var obj = Type_With_Option.None;
      var expected_string = "{\"Default\":[],\"Nested_Once\":[],\"Nested_Twice\":[]}";
      Assert.AreEqual(obj.Serialize(), expected_string);
    }

    [TestMethod]
    public void Serialization_Some1() {
      var obj = Type_With_Option.Some(10);
      var expected_string = "{\"Default\":[10],\"Nested_Once\":[[]],\"Nested_Twice\":[[]]}";
      Assert.AreEqual(obj.Serialize(), expected_string);
    }

    [TestMethod]
    public void Serialization_Some2() {
      var obj = Type_With_Option.Some(10, 41);
      var expected_string = "{\"Default\":[10],\"Nested_Once\":[[41]],\"Nested_Twice\":[[[]]]}";
      Assert.AreEqual(obj.Serialize(), expected_string);
    }
    
    [TestMethod]
    public void Serialization_Some3() {
      var obj = Type_With_Option.Some(10, 41, 32);
      var expected_string = "{\"Default\":[10],\"Nested_Once\":[[41]],\"Nested_Twice\":[[[32]]]}";
      Assert.AreEqual(obj.Serialize(), expected_string);
    }

    #endregion

    #region Deserialization

    [TestMethod]
    public void Deserialization_None() {
      var obj = Type_With_Option.None;
      var json = "{}";
      var deserialized = json.Deserialize<Type_With_Option>();
      Assert.AreEqual(obj, deserialized);
    }

    [TestMethod]
    public void Deserialization_Some1() {
      var obj = Type_With_Option.Some(10);
      var json = "{\"Default\":[10],\"Nested_Once\":[[]],\"Nested_Twice\":[[]]}";
      var deserialized = json.Deserialize<Type_With_Option>();
      Assert.AreEqual(obj, deserialized);
    }
    
    [TestMethod]
    public void Deserialization_Some2() {
      var obj = Type_With_Option.Some(10, 41);
      var json = "{\"Default\":[10],\"Nested_Once\":[[41]],\"Nested_Twice\":[[[]]]}";
      var deserialized = json.Deserialize<Type_With_Option>();
      Assert.AreEqual(obj, deserialized);
    }
    
    [TestMethod]
    public void Deserialization_Some3() {
      var obj = Type_With_Option.Some(10, 41, 32);
      var json = "{\"Default\":[10],\"Nested_Once\":[[41]],\"Nested_Twice\":[[[32]]]}";
      var deserialized = json.Deserialize<Type_With_Option>();
      Assert.AreEqual(obj, deserialized);
    }

    #endregion
  }
}
