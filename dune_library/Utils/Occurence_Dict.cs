using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  internal class Occurence_Dict<T> : Dictionary<T, uint> where T : notnull {
    public new void Add(T key, uint value) {
      if (ContainsKey(key)) {
        base[key] += value;
      } else {
        base[key] = value;
      }
    }

    public bool Remove(T key, uint value) {
      if (ContainsKey(key)) {
        if (base[key] < value) {
          throw new ArgumentException("Tried to remove " + value + " elements, while the map has " + base[key] + " elements present", nameof(value));
        }
        base[key] -= value;
        if (base[key] == 0) {
          Remove(key); // always true
        }
        return true;
      }
      return false;
    }
  }
}
