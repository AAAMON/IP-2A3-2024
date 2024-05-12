using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public interface I_Occurence_Dict<T> : IDictionary<T, uint>, IDictionary, I_Occurence_Dict_Read_Only<T> where T : notnull {
    public bool Remove(T key, uint value);
  }
}
