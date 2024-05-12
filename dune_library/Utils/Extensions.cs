﻿using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public static class Extensions {
    private static Random rng = new Random();

    #region IEnumerable

    public static void Shuffle<T>(this IList<T> to_shuffle) {
      int n = to_shuffle.Count;
      while (n > 1) {
        n--;
        int k = rng.Next(n + 1);
        T value = to_shuffle[k];
        to_shuffle[k] = to_shuffle[n];
        to_shuffle[n] = value;
      }
    }

    public static IList<T> Shuffle<T>(this IReadOnlyList<T> source) {
      List<T> copy = new(source);
      (copy as IList<T>).Shuffle();
      return copy;
    }
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
      foreach (var item in source) {
        action(item);
      }
    }

    public static IEnumerable<T> Repeat<T>(this Func<T> producer, uint count) {
      return Range(0, count).Select(_ => producer.Invoke());
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