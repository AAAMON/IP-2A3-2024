using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public static class Extensions {
    private static readonly Random rng = new();

    #region IList and IReadOnlyList

    public static void Shuffle<T>(this IList<T> to_shuffle) {
      int n = to_shuffle.Count;
      while (n > 1) {
        n--;
        int k = rng.Next(n + 1);
        (to_shuffle[n], to_shuffle[k]) = (to_shuffle[k], to_shuffle[n]);
      }
    }

    public static IList<T> Clone_Shuffled<T>(this IReadOnlyList<T> source) {
      List<T> copy = new(source);
      copy.Shuffle();
      return copy;
    }

    #endregion

    #region IEnumerable

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

    public static T Or<T>(this Option<T> option, T fallback) {
      return option.Match(
        Some: value => value,
        None: () => fallback
      );
    }

    #endregion

    #region Either

    public static L LeftOrThrow<L, R>(this Either<L, R> either, Func<Exception> to_throw) {
      return either.Match(
        Left: value => value,
        Right: _ => throw to_throw.Invoke()
      );
    }

    public static R RightOrThrow<L, R>(this Either<L, R> either, Func<Exception> to_throw) => either.Swap().LeftOrThrow(to_throw);

    public static L LeftOrThrow<L, R>(this Either<L, R> either, Exception to_throw) {
      return either.Match(
        Left: value => value,
        Right: _ => throw to_throw
      );
    }

    public static R RightOrThrow<L, R>(this Either<L, R> either, Exception to_throw) => either.Swap().LeftOrThrow(to_throw);

    public static L LeftOr<L, R>(this Either<L, R> either, L fallback) {
      return either.Match(
        Left: value => value,
        Right: _ => fallback
      );
    }

    public static R RightOr<L, R>(this Either<L, R> either, R fallback) => either.Swap().LeftOr(fallback);

    public static L Left<L, R>(this Either<L, R> either) => (L)either;

    public static R Right<L, R>(this Either<L, R> either) => either.Swap().Left();

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
