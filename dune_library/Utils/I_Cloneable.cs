using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public interface I_Cloneable<T> {
    public T Clone();
  }
}
