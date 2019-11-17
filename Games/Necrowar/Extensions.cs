using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Necrowar
{
    public static class Extensions
    {

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var s in source)
            {
                action(s);
            }
        }

        public static T MinByValue<T, K>(this IEnumerable<T> source, Func<T, K> selector)
        {
            var comparer = Comparer<K>.Default;

            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();

            var min = enumerator.Current;
            var minV = selector(min);

            while (enumerator.MoveNext())
            {
                var s = enumerator.Current;
                var v = selector(s);
                if (comparer.Compare(v, minV) < 0)
                {
                    min = s;
                    minV = v;
                }
            }
            return min;
        }

        public static T MaxByValue<T, K>(this IEnumerable<T> source, Func<T, K> selector)
        {
            var comparer = Comparer<K>.Default;

            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();

            var max = enumerator.Current;
            var maxV = selector(max);

            while (enumerator.MoveNext())
            {
                var s = enumerator.Current;
                var v = selector(s);
                if (comparer.Compare(v, maxV) > 0)
                {
                    max = s;
                    maxV = v;
                }
            }
            return max;
        }

        public static IEnumerable<T> While<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext() && predicate(enumerator.Current))
            {
                yield return enumerator.Current;
            }
        }

        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func, IDictionary<T, TResult> cache = null)
        {
            cache = cache ?? new Dictionary<T, TResult>();
            return t =>
            {
                TResult result;
                if (!cache.TryGetValue(t, out result))
                {
                    result = func(t);
                    cache[t] = result;
                }
                return result;
            };
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            return new[] { item };
        }

        public static IEnumerable<Tile> getTilesInRange(this Tower tower)
        {
            List<Tile> tilesInRange = new List<Tile>();

            /*
             * Shape of the tower range:
             *         _   x   _
             *           x x x
             *         x x T x x
             *           x x x
             *         _   x   _
            */

            //Grabbing the tiles in the upper half
            Tile Up = tower.Tile.TileNorth;
            Tile North = Up.TileNorth;
            Tile East = Up.TileEast;
            Tile West = Up.TileWest;
            tilesInRange.Add(Up);
            tilesInRange.Add(North);
            tilesInRange.Add(East);
            tilesInRange.Add(West);

            //Grabbing the tiles in the lower half
            Tile Down = tower.Tile.TileSouth;
            Tile South = Down.TileSouth;
            East = Down.TileEast;
            West = Down.TileWest;
            tilesInRange.Add(Up);
            tilesInRange.Add(South);
            tilesInRange.Add(East);
            tilesInRange.Add(West);

            //Grabbing the tiles to the left 
            Tile Left = tower.Tile.TileWest;
            West = Down.TileWest;
            tilesInRange.Add(Left);
            tilesInRange.Add(West);

            //Grabbing the tiles to the left 
            Tile Right = tower.Tile.TileEast;
            East = Down.TileEast;
            tilesInRange.Add(Right);
            tilesInRange.Add(East);

            return tilesInRange;
        }

        public static int NumUnits(this Tile tile, UnitJob job)
        {
            if (tile.Unit == null)
            {
                return 0;
            }
            if (tile.Unit.Job.PerTile == 1)
            {
                return 1;
            }
            switch (job.Title)
            {
                case "zombie":
                    return tile.NumZombies;
                case "ghoul":
                    return tile.NumGhouls;
                case "hound":
                    return tile.NumHounds;
            }
            return 1;
        }
    }
}