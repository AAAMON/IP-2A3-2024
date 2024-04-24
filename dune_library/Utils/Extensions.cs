using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public static class Extensions {

    #region IEnumerable

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
      foreach (var item in source) {
        action(item);
      }
    }

    #endregion

  }
}
