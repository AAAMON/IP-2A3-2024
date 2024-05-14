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

    public Occurence_Dict(IReadOnlyDictionary<T, uint> dictionary) : this(dictionary as IEnumerable<KeyValuePair<T, uint>>) { }

    public Occurence_Dict(IEnumerable<KeyValuePair<T, uint>> collection) {
      Underlying_Dict = new(collection.GroupBy(kvp => kvp.Key)
                                      .Select(group => new KeyValuePair<T, uint>(group.Key, (uint)group.Sum(kvp => kvp.Value)))
                                      .Filter(kvp => kvp.Value != 0));
      out_of_constructor = true;
    }

    private Dictionary<T, uint> Underlying_Dict { get; }

    private bool out_of_constructor = false;

    #region Behaviour that did not change from the original implementation

    public IEnumerable<T> Keys => (Underlying_Dict as IReadOnlyDictionary<T, uint>).Keys;

    public IEnumerable<uint> Values => (Underlying_Dict as IReadOnlyDictionary<T, uint>).Values;

    ICollection<T> IDictionary<T, uint>.Keys => (Underlying_Dict as IDictionary<T, uint>).Keys;

    ICollection<uint> IDictionary<T, uint>.Values => (Underlying_Dict as IDictionary<T, uint>).Values;

    ICollection IDictionary.Keys => (Underlying_Dict as IDictionary).Keys;

    ICollection IDictionary.Values => (Underlying_Dict as IDictionary).Values;

    public int Count => (Underlying_Dict as IReadOnlyCollection<KeyValuePair<T, uint>>).Count;

    public bool IsReadOnly => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).IsReadOnly;

    public bool IsFixedSize => (Underlying_Dict as IDictionary).IsFixedSize;

    public bool IsSynchronized => (Underlying_Dict as ICollection).IsSynchronized;

    public object SyncRoot => (Underlying_Dict as ICollection).SyncRoot;

    public bool ContainsKey(T key) => (Underlying_Dict as IReadOnlyDictionary<T, uint>).ContainsKey(key);

    public bool Contains(KeyValuePair<T, uint> item) => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).Contains(item);

    public bool Contains(object key) => (Underlying_Dict as IDictionary).Contains(key);

    public bool TryGetValue(T key, [MaybeNullWhen(false)] out uint value) => (Underlying_Dict as IReadOnlyDictionary<T, uint>).TryGetValue(key, out value);

    public void Clear() => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).Clear();

    public void CopyTo(KeyValuePair<T, uint>[] array, int arrayIndex) => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).CopyTo(array, arrayIndex);

    public void CopyTo(Array array, int index) => (Underlying_Dict as ICollection).CopyTo(array, index);

    public IEnumerator<KeyValuePair<T, uint>> GetEnumerator() => (Underlying_Dict as IEnumerable<KeyValuePair<T, uint>>).GetEnumerator();

    IDictionaryEnumerator IDictionary.GetEnumerator() => (Underlying_Dict as IDictionary).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (Underlying_Dict as IEnumerable).GetEnumerator();

#pragma warning disable SYSLIB0050 // Type or member is obsolete (fuck do you want me to do?)
    public void GetObjectData(SerializationInfo info, StreamingContext context) => (Underlying_Dict as ISerializable).GetObjectData(info, context);
#pragma warning restore SYSLIB0050 // Type or member is obsolete

    public void OnDeserialization(object? sender) => (Underlying_Dict as IDeserializationCallback).OnDeserialization(sender);

    #endregion

    uint IDictionary<T, uint>.this[T key] {
      get => (Underlying_Dict as IDictionary<T, uint>)[key];
      set {
        if (out_of_constructor == false) {
          throw new NotSupportedException();
        }
        if (value != 0) {
          (Underlying_Dict as IDictionary<T, uint>)[key] = value;
        }
      }
    }

    public uint this[T key] {
      get => (Underlying_Dict as IReadOnlyDictionary<T, uint>)[key];
      set => (this as IDictionary<T, uint>)[key] = value;
    }

    public object? this[object key] {
      get => (Underlying_Dict as IDictionary)[key];
      set {
        if (out_of_constructor == false) {
          throw new NotSupportedException();
        }
        uint? v = (uint?)value;
        if (v.HasValue && v != 0) { (Underlying_Dict as IDictionary)[key] = v; }
      } // I'm not sure if this is right and also I'm not sure if this will ever be useful, so...
    }

    public void Add(T key, uint value = 1) {
      if (Underlying_Dict.ContainsKey(key)) {
        Underlying_Dict[key] += value;
      } else {
        Underlying_Dict[key] = value;
      }
    }

    public bool Remove(T key) => (Underlying_Dict as IDictionary<T, uint>).Remove(key);

    public void Add(KeyValuePair<T, uint> item) => Add(item.Key, item.Value);

    // this makes more sense to remove the kvp only if the exact same kvp can be found in the dictionary
    // as opposed to removing kvp.Value units from kvp.Key
    public bool Remove(KeyValuePair<T, uint> item) => (Underlying_Dict as ICollection<KeyValuePair<T, uint>>).Remove(item);

    public void Add(object key, object? value) => Add((T)key, (uint)value!);

    public void Remove(object key) => (Underlying_Dict as IDictionary).Remove(key);

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

    public I_Occurence_Dict<T> Clone() => new Occurence_Dict<T>(Underlying_Dict); // this does, in fact, clone (check Dictionary.cs, line 98 and Occurence_Dict.cs, line 28-30)
  }
}
