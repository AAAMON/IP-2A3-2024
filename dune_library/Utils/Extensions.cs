﻿using LanguageExt;
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
      Range(0, 10);
    }

    public static IEnumerable<uint> Range(uint start, uint count) => Enumerable.Range((int)start, (int)count).Select(i => (uint)i);

    #endregion

    #region Option

    public static T OrElseThrow<T>(this Option<T> option, Func<Exception> to_throw) {
      return option.Match(
          Some: value => value,
          None: () => throw to_throw.Invoke()
      );
    }

    public static T OrElseThrow<T>(this Option<T> option, Exception to_throw) {
      return option.Match(
          Some: value => value,
          None: () => throw to_throw
      );
    }

    #endregion    
    #region (u)int to sector

    public static uint To_Sector(this uint raw_sector) => raw_sector % Map_Resources.Map.NUMBER_OF_SECTORS;

    #endregion

    #region bool to (u)int
    public static int To_Int(this bool value) => value ? 1 : 0;

    public static uint To_Uint(this bool value) => (uint)(value ? 1 : 0);

    #endregion

  }
}
