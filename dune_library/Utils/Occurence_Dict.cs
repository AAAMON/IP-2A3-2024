using dune_library.Player_Resources;
using LanguageExt;
using static LanguageExt.Prelude;
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
  public class Occurence_Dict<T> : I_Occurence_Dict<T> where T : notnull {
    public Occurence_Dict() {
      Underlying_Dict = [];
      out_of_constructor = true;
    }

    public Occurence_Dict(IDictionary<T, uint> dictionary) : this(dictionary as IEnumerable<KeyValuePair<T, uint>>) { }

    public Occurence_Dict(IEnumerable<KeyValuePair<T, uint>> collection) {
      Underlying_Dict = new(collection.Filter(kvp => kvp.Value != 0));
      out_of_constructor = true;
    }

    private Dictionary<T, uint> Underlying_Dict { get; }

    private bool out_of_constructor = false;

    #region Behaviour that did not change from the original implementation

    public ICollection<T> Keys => (Underlying_Dict as IDictionary<T, uint>).Keys;

    public ICollection<uint> Values => (Underlying_Dict as IDictionary<T, uint>).Values;

    public int Count => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).Count;

    public bool IsReadOnly => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).IsReadOnly;

    public bool IsFixedSize => (Underlying_Dict as IDictionary).IsFixedSize;

    ICollection IDictionary.Keys => (Underlying_Dict as IDictionary).Keys;

    ICollection IDictionary.Values => (Underlying_Dict as IDictionary).Values;

    public bool IsSynchronized => (Underlying_Dict as ICollection).IsSynchronized;

    public object SyncRoot => (Underlying_Dict as ICollection).SyncRoot;

    IEnumerable<T> IReadOnlyDictionary<T, uint>.Keys => (Underlying_Dict as IReadOnlyDictionary<T, uint>).Keys;

    IEnumerable<uint> IReadOnlyDictionary<T, uint>.Values => (Underlying_Dict as IReadOnlyDictionary<T, uint>).Values;

    public void Clear() => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).Clear();

    public bool Contains(KeyValuePair<T, uint> item) => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).Contains(item);

    public bool ContainsKey(T key) => (Underlying_Dict as IDictionary<T, uint>).ContainsKey(key);

    public void CopyTo(KeyValuePair<T, uint>[] array, int arrayIndex) => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<T, uint>> GetEnumerator() => (Underlying_Dict as IEnumerable<KeyValuePair<T, uint>>).GetEnumerator();

    public bool TryGetValue(T key, [MaybeNullWhen(false)] out uint value) => (Underlying_Dict as IDictionary<T, uint>).TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => (Underlying_Dict as IEnumerable).GetEnumerator();

    public bool Contains(object key) => (Underlying_Dict as IDictionary).Contains(key);

    IDictionaryEnumerator IDictionary.GetEnumerator() => (Underlying_Dict as IDictionary).GetEnumerator();

    public void CopyTo(Array array, int index) => (Underlying_Dict as ICollection).CopyTo(array, index);

#pragma warning disable SYSLIB0050 // Type or member is obsolete (fuck do you want me to do?)
    public void GetObjectData(SerializationInfo info, StreamingContext context) => (Underlying_Dict as ISerializable).GetObjectData(info, context);
#pragma warning restore SYSLIB0050 // Type or member is obsolete

    public void OnDeserialization(object? sender) => (Underlying_Dict as IDeserializationCallback).OnDeserialization(sender);

    public bool Remove(KeyValuePair<T, uint> item) => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).Remove(item);

    public bool Remove(T key) => (Underlying_Dict as IDictionary<T, uint>).Remove(key);

    public void Remove(object key) => (Underlying_Dict as IDictionary).Remove(key);

    #endregion

    public uint this[T key] { get => Underlying_Dict[key]; set {
        if (out_of_constructor == false) {
          throw new NotSupportedException();
        }
        if (value != 0) {
          Underlying_Dict[key] = value;
        }
      }
    }

    public object? this[object key] { get => (Underlying_Dict as IDictionary)[key]; set {
        if (out_of_constructor == false) {
          throw new NotSupportedException();
        }
        uint? v = (uint?)value;
        if (v.HasValue && v != 0 ) { (Underlying_Dict as IDictionary)[key] = v; }
      }
    }

    public void Add(T key, uint value = 1) {
      if (Underlying_Dict.ContainsKey(key)) {
        Underlying_Dict[key] += value;
      } else {
        Underlying_Dict[key] = value;
      }
    }

    public void Add(KeyValuePair<T, uint> item) => Add(item.Key, item.Value);

    public void Add(object key, object? value) => Add((T)key, (uint)value!);

    public bool Remove(T key, uint value) {
      if (Underlying_Dict.ContainsKey(key) == false || Underlying_Dict[key] < value) {
        return false;
      }
      Underlying_Dict[key] -= value;
      if (Underlying_Dict[key] == 0) {
        Underlying_Dict.Remove(key);
      }
      return true;
    }

    public bool Remove(T key, out uint value) => Underlying_Dict.Remove(key, out value);

    public void Remove(T key, out Option<uint> value) {
      if (Remove(key, out uint v) == false) {
        value = None;
      }
      value = Some(v);
    }
  }
}
