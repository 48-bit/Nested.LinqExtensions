using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nested.LinqExtensions
{
    public static class LinqExtensions
    {
        
        public static IQueryable<TSource> AncestorsOf<TSource, TFilter>(
            this IQueryable<TSource> collection, 
            Expression<Func<TSource, IHasTreeEntry>> navigateExpr, 
            TFilter item, 
            int depth=-1, 
            bool includeSelf = false) 
            where TFilter: IHasTreeEntry
        {
            return collection.Where(navigateExpr.Navigate(i => i.TreeEntry), NestedIntervalsSpec.AncestorsOf(item, depth, includeSelf));
        }

        public static IQueryable<TResult> AncestorsOf<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item, int depth=-1, bool includeSelf = false) 
            where TResult : IHasTreeEntry
            where TFilter: IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.AncestorsOf(item, depth, includeSelf));
        }

        public static IQueryable<TResult> DescendantsOf<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item, int depth = -1, bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter: IHasTreeEntry

        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.DescendantsOf(item, includeSelf, depth));
        }

        public static IQueryable<TResult> DescendantsOfAny<TResult, TFilter>(this IQueryable<TResult> collection, IEnumerable<TFilter> items,
            bool includeSelf = false, int depth = -1)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.DescendantsOfAny(items, includeSelf, depth));
        }
        public static IQueryable<TSource> DescendantsOfAny<TSource, TFilter>(
            this IQueryable<TSource> collection,
            Expression<Func<TSource, IHasTreeEntry>> navigateExpr,
            IEnumerable<TFilter> items,
            bool includeSelf = false, int depth = -1)
            where TFilter : IHasTreeEntry
        {
            return collection.Where(navigateExpr.Navigate(i => i.TreeEntry), NestedIntervalsSpec.DescendantsOfAny(items, includeSelf, depth));
        }

        public static TResult ParentOf<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter: IHasTreeEntry
        {
            return AncestorsOf<TResult, TFilter>(collection, item, 1).FirstOrDefault();
        }

        public static IQueryable<TResult> ChildrenOf<TResult, TFilter>(this IQueryable<TResult> collection,
            TFilter item)
            where TResult: IHasTreeEntry
            where TFilter: IHasTreeEntry
        {
            return collection.DescendantsOf(item, depth: 1, includeSelf: false);
        }

        public static IQueryable<TResult> SiblingsOf<TResult, TFilter>(this IQueryable<TResult> collection,
            TFilter item, bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsOf(item.TreeEntry, includeSelf));
        }

        public static IQueryable<TResult> SiblingsBefore<TResult, TFilter>(this IQueryable<TResult> collection,
            TFilter item, bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsBefore(item.TreeEntry, includeSelf));
        }

        public static IQueryable<TResult> SiblingsAfter<TResult, TFilter>(this IQueryable<TResult> collection,
            TFilter item, bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsAfter(item.TreeEntry, includeSelf));
        }

        public static TResult FirstSiblingOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsBefore(item.TreeEntry, true))
                .OrderBy(i => i.TreeEntry.Nv).FirstOrDefault();
        }

        public static TResult LastSiblingOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsAfter(item.TreeEntry, true))
                .OrderBy(i => i.TreeEntry.Nv).LastOrDefault();
        }

        public static TResult NextSiblingOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.NextSiblingOf(item.TreeEntry)).FirstOrDefault();
        }

        public static TResult FirstChildOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.ChildrenOf(item).OrderBy(i => i.TreeEntry.Nv).FirstOrDefault();
        }

        public static TResult LastChildOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.ChildrenOf(item).OrderBy(i => i.TreeEntry.Nv).LastOrDefault();
        }

        public static TResult FirstRootOrDefault<TResult>(this IQueryable<TResult> collection)
            where TResult : IHasTreeEntry
        {
            return collection.Where(c => c.TreeEntry, NestedIntervalsSpec.FirstRootEntry()).OrderBy(i => i.TreeEntry.Nv).FirstOrDefault();
        }

        public static TResult LastRootOrDefault<TResult>(this IQueryable<TResult> collection)
            where TResult: IHasTreeEntry
        {
            return collection
                .Where(c => c.TreeEntry, NestedIntervalsSpec.RootEntries())
                .OrderByDescending(i => i.TreeEntry.Nv)
                .FirstOrDefault();
        }

        public static IQueryable<TResult> ByPositionsPath<TResult>(this IQueryable<TResult> collection,
            IEnumerable<long> positionsPath)
        where TResult: IHasTreeEntry
        {
            return collection
                .Where((i) => i.TreeEntry, NestedIntervalsSpec.ElementsByPositionsPath(positionsPath))
                .OrderBy(i => i.TreeEntry.Nv);
        }

        public static IQueryable<T1> Where<T1, T2>(this IQueryable<T1> collection, Expression<Func<T1, T2>> propertySelector,
            Expression<Func<T2, bool>> propertyPredicate)
        {
            return collection.Where(propertySelector.Compose(propertyPredicate));
        }
    }
}