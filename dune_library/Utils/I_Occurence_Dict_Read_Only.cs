using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public interface I_Occurence_Dict_Read_Only<T> : IReadOnlyDictionary<T, uint>, ISerializable, IDeserializationCallback where T : notnull {
  }
}
