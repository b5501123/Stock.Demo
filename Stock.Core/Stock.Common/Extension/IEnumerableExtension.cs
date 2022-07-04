using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stock.Common.Extension
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<TSource> ExceptAll<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            // Do not call reuse the overload method because that is a slower imlementation
            if (first == null) { throw new ArgumentNullException("first"); }
            if (second == null) { throw new ArgumentNullException("second"); }

            var secondList = second.ToList();
            return first.Where(s => !secondList.Remove(s));
        }

        /// <summary>
        /// 檢查 self 是否有任一元素與 target 相同
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool HasSameItem<TSource>(this IEnumerable<TSource> self, IEnumerable<TSource> target) where TSource : IComparable<TSource>
        {
            return self.Any(i => target.Any(j => i.CompareTo(j) == 0));
        }

        /// <summary>
        /// Is list empty or null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmptyOrNull<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    return true;
                case ICollection collection:
                    return collection.Count < 1;
                default:
                    return !source.Any();
            }
        }

        /// <summary>
        /// Is list not empty and not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return false;

            switch (source)
            {
                case ICollection collection:
                    return collection.Count > 0;
                default:
                    return source.Any();
            }
        }

        /// <summary>
        /// To hashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        /// <summary>
        /// Distinct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> DistinctBy<T, TKey>(this IList<T> source,
            Func<T, TKey> func)
        {
            return source.IsEmptyOrNull() ? new List<T>() :
                source.GroupBy(func).Select(x => x.First()).ToList();
        }

        /// <summary>
        /// Get first in group
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="groupBy"></param>
        /// <param name="orderBy"></param>
        /// <param name="isOrderByDescending"></param>
        /// <returns></returns>
        public static List<T> GetFirstByGroup<T, TKey1, TKey2>(this IList<T> source,
            Func<T, TKey1> groupBy,
            Func<T, TKey2> orderBy,
            bool isOrderByDescending = false)
        {
            if (source.IsEmptyOrNull())
                return new List<T>();

            var group = source.GroupBy(groupBy);

            if (isOrderByDescending)
            {
                return group
                    .Select(a => a.OrderByDescending(orderBy)
                        .FirstOrDefault())
                    .ToList();
            }

            return group
                .Select(a => a.OrderBy(orderBy).FirstOrDefault())
                .ToList();
        }

        /// <summary>
        /// Full outer join
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="getKeySource"></param>
        /// <param name="getKeyTarget"></param>
        /// <returns></returns>
        public static List<TResult> FullOuterJoin<T1, T2, TKey, TResult>(this IEnumerable<T1> source,
            IEnumerable<T2> target,
            Func<T1, TKey> getKeySource,
            Func<T2, TKey> getKeyTarget,
            Func<TKey, T1, T2, TResult> getResult)
        {
            var gupS = source.GroupBy(getKeySource).ToList();
            var gupT = target.GroupBy(getKeyTarget).ToList();
            var allKsy = gupS.Select(i => i.Key).Union(gupT.Select(i => i.Key)).ToList();

            return
                (from k in allKsy
                 join s in gupS.SelectMany(i => i.Select(j => (Key: i.Key, Row: j))) on k equals s.Key into Ta
                 from s in Ta.DefaultIfEmpty((Key: default, Row: default))
                 join t in gupT.SelectMany(i => i.Select(j => (Key: i.Key, Row: j))) on k equals t.Key into Tb
                 from t in Tb.DefaultIfEmpty((Key: default, Row: default))
                 select getResult(k, s.Row, t.Row))
                .ToList();
        }

        /// <summary>
        /// 依筆數分群
        /// </summary>
        public static List<List<T>> GroupByCount<T>(this IEnumerable<T> source, int cnt)
        {
            return source
                .Select((i, idx) => (no: idx, row: i))
                .GroupBy(i => i.no / cnt)
                .Select(i => i.Select(j => j.row).ToList())
                .ToList();
        }

    }
}
