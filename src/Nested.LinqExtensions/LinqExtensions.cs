using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nested.LinqExtensions
{
    /// <summary>
    /// Set of extensions to query hierarchical data specified with nested interval encoding.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Filters a sequence of values to include only ancestors (parents and all parents of parents) of given item.
        /// </summary>
        /// <param name="collection">sequence to filter. </param>
        /// <param name="navigateExpr">Property expression returning path to IHasTreeEntry item. </param>
        /// <param name="item">child item to find it's ancestors. </param>
        /// <param name="depth">Level of inheritance to filter ancestors.
        /// 1 returns only direct parents, 2 - parents of parents etc.
        /// -1 to return all ancestors up to root items.
        /// -1 by default. </param>
        /// <param name="includeSelf">Whether or not to include given child item itself in list. false by default. </param>
        /// <typeparam name="TResult">Type of sequence containing ancestors elements. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in filter. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of ancestors of given item. </returns>
        public static IQueryable<TResult> AncestorsOf<TResult, TFilter>(
            this IQueryable<TResult> collection,
            Expression<Func<TResult, IHasTreeEntry>> navigateExpr,
            TFilter item,
            int depth = -1,
            bool includeSelf = false)
            where TFilter : IHasTreeEntry
        {
            return collection.Where(navigateExpr.Navigate(i => i.TreeEntry), NestedIntervalsSpec.AncestorsOf(item, depth, includeSelf));
        }

        /// <summary>
        /// Filters a sequence of values to include only ancestors (parents and all parents of parents) of given item.
        /// </summary>
        /// <param name="collection">sequence to filter. </param>
        /// <param name="item">child item to find it's ancestors. </param>
        /// <param name="depth">Level of inheritance to filter ancestors.
        /// 1 returns only direct parents, 2 - parents of parents etc.
        /// -1 to return all ancestors up to root items.
        /// -1 by default. </param>
        /// <param name="includeSelf">Whether or not to include given child item itself in list. false by default. </param>
        /// <typeparam name="TResult">Type of sequence containing ancestors elements. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in filter. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of ancestors of given item. </returns>
        public static IQueryable<TResult> AncestorsOf<TResult, TFilter>(
            this IQueryable<TResult> collection,
            TFilter item,
            int depth = -1,
            bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.AncestorsOf(item, depth, includeSelf));
        }

        /// <summary>
        /// Filters a sequence of values to include only descendants (children and all children of children) of given item.
        /// </summary>
        /// <param name="collection">sequence to filter. </param>
        /// <param name="item">parent item to find it's descendants. </param>
        /// <param name="depth">Level of inheritance to filter descendants.
        /// 1 returns only direct children, 2 - children of children etc.
        /// -1 to return all descendants down to deepest leaves.
        /// -1 by default. </param>
        /// <param name="includeSelf">Whether or not to include given parent item itself in list. false by default. </param>
        /// <typeparam name="TResult">Type of sequence containing descendant elements. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in filter. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of descendants of given item. </returns>
        public static IQueryable<TResult> DescendantsOf<TResult, TFilter>(
            this IQueryable<TResult> collection,
            TFilter item,
            int depth = -1,
            bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.DescendantsOf(item, depth, includeSelf));
        }

        /// <summary>
        /// Filters a sequence of values to return descendants of all given items.
        /// </summary>
        /// <param name="collection">sequence to filter. </param>
        /// <param name="items">sequence of parent items to find it's descendants. </param>
        /// <param name="depth">Level of inheritance to filter descendants.
        ///     1 returns only direct children, 2 - children of children etc.
        ///     -1 to return all descendants down to deepest leaves.
        ///     -1 by default. </param>
        /// <param name="includeSelf">Whether or not to include given parent items itself in list. false by default. </param>
        /// <typeparam name="TResult">Type of sequence containing ancestors elements. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced items to use in filter. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of descendants of given items. </returns>
        public static IQueryable<TResult> DescendantsOfAny<TResult, TFilter>(
            this IQueryable<TResult> collection,
            IEnumerable<TFilter> items,
            int depth = -1,
            bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.DescendantsOfAll(items, depth, includeSelf));
        }

        /// <summary>
        /// Filters a sequence of values to return descendants of all given items.
        /// </summary>
        /// <param name="collection">sequence to filter. </param>
        /// <param name="navigateExpr">Property expression returning path to IHasTreeEntry item. </param>
        /// <param name="items">sequence of parent items to find it's descendants. </param>
        /// <param name="depth">Level of inheritance to filter descendants.
        ///     1 returns only direct children, 2 - children of children etc.
        ///     -1 to return all descendants down to deepest leaves.
        ///     -1 by default. </param>
        /// <param name="includeSelf">Whether or not to include given parent items itself in list. false by default. </param>
        /// <typeparam name="TResult">Type of sequence containing descendants elements. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced items to use in filter. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of descendants of given items. </returns>
        public static IQueryable<TResult> DescendantsOfAny<TResult, TFilter>(
            this IQueryable<TResult> collection,
            Expression<Func<TResult, IHasTreeEntry>> navigateExpr,
            IEnumerable<TFilter> items,
            int depth = -1,
            bool includeSelf = false)
            where TFilter : IHasTreeEntry
        {
            return collection.Where(navigateExpr.Navigate(i => i.TreeEntry), NestedIntervalsSpec.DescendantsOfAll(items, depth, includeSelf));
        }

        /// <summary>
        /// Returns parent item of given child item.
        /// </summary>
        /// <param name="collection">Sequence to find parent. </param>
        /// <param name="item">child item. </param>
        /// <typeparam name="TResult">Type of sequence containing parent. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced child item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Parent item of given child item. </returns>
        public static TResult ParentOf<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return AncestorsOf(collection, item, 1).FirstOrDefault();
        }

        /// <summary>
        /// Filters a sequence of values to return children of given parent item.
        /// </summary>
        /// <param name="collection">sequence to filter. </param>
        /// <param name="item">parent item to find it's children. </param>
        /// <typeparam name="TResult">Type of sequence containing children. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced parent item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of children items of given parent item. </returns>
        public static IQueryable<TResult> ChildrenOf<TResult, TFilter>(
            this IQueryable<TResult> collection,
            TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.DescendantsOf(item, depth: 1);
        }

        /// <summary>
        /// Filter a sequence of values to return sibling elements of given item.
        /// Item considered as sibling if it's children of same parent item.
        /// </summary>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="item">Item to find siblings for. </param>
        /// <param name="includeSelf">Whether or not to include referenced item itself in list of siblings. false by default. </param>
        /// <typeparam name="TResult">Type of sequence containing item siblings. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of sibling items of given item. </returns>
        public static IQueryable<TResult> SiblingsOf<TResult, TFilter>(
            this IQueryable<TResult> collection,
            TFilter item,
            bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsOf(item.TreeEntry, includeSelf));
        }

        /// <summary>
        /// Filter a sequence of values to return sibling elements of given item, positioned before it.
        /// Item considered as sibling if it's children of same parent item.
        /// </summary>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="item">Item to find siblings for. </param>
        /// <param name="includeSelf">Whether or not to include referenced item itself in list of siblings. false by default. </param>
        /// <typeparam name="TResult">Type of sequence containing item siblings. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of sibling items of given item, positioned before it. </returns>
        public static IQueryable<TResult> SiblingsBefore<TResult, TFilter>(
            this IQueryable<TResult> collection,
            TFilter item,
            bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsBefore(item.TreeEntry, includeSelf));
        }

        /// <summary>
        /// Filter a sequence of values to return sibling elements of given item, positioned after it.
        /// Item considered as sibling if it's children of same parent item.
        /// </summary>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="item">Item to find siblings for. </param>
        /// <param name="includeSelf">Whether or not to include referenced item itself in list of siblings. false by default. </param>
        /// <typeparam name="TResult">Type of sequence containing item siblings. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of sibling items of given item, positioned after it. </returns>
        public static IQueryable<TResult> SiblingsAfter<TResult, TFilter>(
            this IQueryable<TResult> collection,
            TFilter item,
            bool includeSelf = false)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsAfter(item.TreeEntry, includeSelf));
        }

        /// <summary>
        /// Returns first sibling of given item, or default value if there are no siblings.
        /// </summary>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="item">Item to find siblings for. </param>
        /// <typeparam name="TResult">Type of sequence containing item siblings. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>first sibling of given item or default value if there are no siblings. </returns>
        public static TResult FirstSiblingOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsBefore(item.TreeEntry, true))
                .OrderBy(i => i.TreeEntry.Nv).FirstOrDefault();
        }

        /// <summary>
        /// Returns last sibling of given item, or default value if there are no siblings.
        /// </summary>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="item">Item to find siblings for. </param>
        /// <typeparam name="TResult">Type of sequence containing item siblings. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>last sibling of given item or default value if there are no siblings. </returns>
        public static TResult LastSiblingOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.SiblingsAfter(item.TreeEntry, true))
                .OrderBy(i => i.TreeEntry.Nv).LastOrDefault();
        }

        /// <summary>
        /// Returns sibling item of given item, positioned right after it, or default value if there are no next sibling.
        /// </summary>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="item">Item to find siblings for. </param>
        /// <typeparam name="TResult">Type of sequence containing item siblings. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>sibling item of given item, positioned right after it, or default value if there are no next sibling. </returns>
        public static TResult NextSiblingOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.Where(i => i.TreeEntry, NestedIntervalsSpec.NextSiblingOf(item.TreeEntry)).FirstOrDefault();
        }

        /// <summary>
        /// Returns first child item of given  parent item, or default value if there are no children.
        /// </summary>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="item">Parent item to find first child for. </param>
        /// <typeparam name="TResult">Type of sequence containing item children. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>First child item of given item, or default value if there are no children. </returns>
        public static TResult FirstChildOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.ChildrenOf(item).OrderBy(i => i.TreeEntry.Nv).FirstOrDefault();
        }

        /// <summary>
        /// Returns last child item of given  parent item, or default value if there are no children.
        /// </summary>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="item">Parent item to find first child for. </param>
        /// <typeparam name="TResult">Type of sequence containing item children. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <typeparam name="TFilter">Type of referenced item to use in query. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Last child item of given item, or default value if there are no children. </returns>
        public static TResult LastChildOrDefault<TResult, TFilter>(this IQueryable<TResult> collection, TFilter item)
            where TResult : IHasTreeEntry
            where TFilter : IHasTreeEntry
        {
            return collection.ChildrenOf(item).OrderBy(i => i.TreeEntry.Nv).LastOrDefault();
        }

        /// <summary>
        /// Returns first root item in given collection or default value if there are no items in hierarchy.
        /// </summary>
        /// <param name="collection">sequence to find first root. </param>
        /// <typeparam name="TResult">Type of sequence containing root item. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>First root item or default value if there are no items in hierarchy. </returns>
        public static TResult FirstRootOrDefault<TResult>(this IQueryable<TResult> collection)
            where TResult : IHasTreeEntry
        {
            return collection
                .Where(c => c.TreeEntry, NestedIntervalsSpec.FirstRootEntry())
                .OrderBy(i => i.TreeEntry.Nv)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns first root item in given collection or default value if there are no items in hierarchy.
        /// </summary>
        /// <param name="collection">sequence to find first root. </param>
        /// <typeparam name="TResult">Type of sequence containing root item. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>First root item or default value if there are no items in hierarchy. </returns>
        public static TResult LastRootOrDefault<TResult>(this IQueryable<TResult> collection)
            where TResult : IHasTreeEntry
        {
            return collection
                .Where(c => c.TreeEntry, NestedIntervalsSpec.RootEntries())
                .OrderByDescending(i => i.TreeEntry.Nv)
                .FirstOrDefault();
        }

        /// <summary>
        /// Filters a sequence of values to select elements by it's positions relative to parent. 
        /// </summary>
        /// <example>
        /// For path {2, 1, 3} it will select 2nd root item, then first child of 2nd root item, then third child of 2nd root's first child.
        /// </example>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="positionsPath">Elements positions path relative to it's parent. Position indexes starting from 1.</param>
        /// <typeparam name="TResult">Type of sequence to filter. Should implement <see cref="IHasTreeEntry"/>. </typeparam>
        /// <returns>Sequence of items filtered by given positions path, relative to parents. </returns>
        public static IQueryable<TResult> ByPositionsPath<TResult>(
            this IQueryable<TResult> collection,
            IEnumerable<long> positionsPath)
        where TResult : IHasTreeEntry
        {
            return collection
                .Where((i) => i.TreeEntry, NestedIntervalsSpec.ElementsByPositionsPath(positionsPath))
                .OrderBy(i => i.TreeEntry.Nv);
        }

        /// <summary>
        /// Filters a sequence of values by given predicate for property selected by given navigate expression.
        /// </summary>
        /// <typeparam name="T1">Type of sequence elements. </typeparam>
        /// <typeparam name="T2">Type of element to filter with predicate. </typeparam>
        /// <param name="collection">Sequence to filter. </param>
        /// <param name="propertySelector">Navigate expression to select property for predicate. </param>
        /// <param name="propertyPredicate">A function to test each element with condition. </param>
        /// <returns>Sequence of element filtered with given predicate. </returns>
        public static IQueryable<T1> Where<T1, T2>(
            this IQueryable<T1> collection, 
            Expression<Func<T1, T2>> propertySelector,
            Expression<Func<T2, bool>> propertyPredicate)
        {
            return collection.Where(propertySelector.Compose(propertyPredicate));
        }
    }
}