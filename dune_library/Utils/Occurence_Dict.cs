using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public class Occurence_Dict<T> : IDictionary<T, uint>, IEnumerable<T> where T : notnull {
    public Occurence_Dict() {
      underlying_dict = [];
    }

    [JsonConstructor]
    public Occurence_Dict(Dictionary<T, uint> underlying_dict) {
      this.underlying_dict = underlying_dict;
    }

    private Dictionary<T, uint> underlying_dict { get; }

    public uint this[T key] { get => underlying_dict[key]; set => throw new NotSupportedException(); }

    [JsonIgnore]
    public ICollection<T> Keys => underlying_dict.Keys;

    [JsonIgnore]
    public ICollection<uint> Values => underlying_dict.Values;

    [JsonIgnore]
    public int Count => underlying_dict.Count;

    [JsonIgnore]
    public bool IsReadOnly => false;

    public void Add(KeyValuePair<T, uint> item) {
      underlying_dict.Add(item.Key, item.Value);
    }

    public void Add(T key, uint value = 1) {
      if (underlying_dict.ContainsKey(key)) {
        underlying_dict[key] += value;
      } else {
        underlying_dict[key] = value;
      }
    }

    public void Clear() => underlying_dict.Clear();

    public bool Contains(KeyValuePair<T, uint> item) => underlying_dict.Contains(item);

    public bool ContainsKey(T key) => underlying_dict.ContainsKey(key);

    public void CopyTo(KeyValuePair<T, uint>[] array, int arrayIndex) {
      throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<T, uint>> GetEnumerator() => underlying_dict.GetEnumerator();

    public bool Remove(KeyValuePair<T, uint> item) => Remove(item.Key, item.Value);

    public bool Remove(T key, uint value) {
      if (underlying_dict.ContainsKey(key)) {
        if (underlying_dict[key] < value) {
          throw new ArgumentException("Tried to remove " + value + " elements, while the map has " + underlying_dict[key] + " elements present", nameof(value));
        }
        underlying_dict[key] -= value;
        if (underlying_dict[key] == 0) {
          underlying_dict.Remove(key);
        }
        return true;
      }
      return false;
    }

    public bool Remove(T key) => Remove(key, 1);

    public bool TryGetValue(T key, [MaybeNullWhen(false)] out uint value) => underlying_dict.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => underlying_dict.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() {
      return (IEnumerator<T>)underlying_dict.SelectMany(kvp => Enumerable.Range(0, (int)kvp.Value).Select(_ => kvp.Key)).AsEnumerable();
    }
  }
}
