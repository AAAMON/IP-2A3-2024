using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public interface I_Occurence_Dict<T> : I_Occurence_Dict_Read_Only<T>, IDictionary<T, uint>, IDictionary where T : notnull {

    public new uint this[T key] { get => (this as I_Occurence_Dict_Read_Only<T>)[key]; set => (this as IDictionary<T, uint>)[key] = value; }

    public new ICollection<T> Keys => (this as IDictionary<T, uint>).Keys;

    public new ICollection<uint> Values => (this as IDictionary<T, uint>).Values;


    public new void Add(T key, uint value = 1) => (this as IDictionary<T, uint>).Add(key, value);
    
    public new bool ContainsKey(T key) => (this as I_Occurence_Dict_Read_Only<T>).ContainsKey(key);

    public new bool Remove(T key) => (this as IDictionary<T, uint>).Remove(key);

    public new bool TryGetValue(T key, out uint value) => (this as I_Occurence_Dict_Read_Only<T>).TryGetValue(key, out value);


    public new int Count { get => (this as I_Occurence_Dict_Read_Only<T>).Count; }

    public new IEnumerator<KeyValuePair<T, uint>> GetEnumerator() => (this as I_Occurence_Dict_Read_Only<T>).GetEnumerator();


    public bool Remove(T key, uint value);
  }
}
