using System.Collections.Generic;
using System.Linq;
using Nested.LinqExtensions.Utils;
using TreeFilter = System.Linq.Expressions.Expression<System.Func<Nested.LinqExtensions.TreeEntry, bool>>;

namespace Nested.LinqExtensions
{
    /// <summary>
    /// Predicates to filter hierarchical collections specified with Nested Intervals pattern.
    /// </summary>
    public static class NestedIntervalsSpec
    {
        /// <summary>
        /// Build predicate selecting ancestors for given element.
        /// </summary>
        /// <param name="item">element to select ancestors for. </param>
        /// <param name="depth">Max inheritance level. </param>
        /// <param name="includeSelf">Whether or not to add element itself to result. </param>
        /// <returns>Predicate selecting ancestors of element. </returns>
        public static TreeFilter AncestorsOf(TreeEntry item, int depth, bool includeSelf)
        {
            TreeFilter result;

            if (includeSelf)
            {
                result = r =>
                    item.Nv * r.Dv >= item.Dv * r.Nv &&
                    item.SNv * r.SDv <= item.SDv * r.SNv;
            }
            else
            {
                result = r =>
                    item.Nv * r.Dv > item.Dv * r.Nv &&
                    item.SNv * r.SDv < item.SDv * r.SNv;
            }

            if (depth > 0)
            {
                result = result.And(r => item.Depth - r.Depth <= depth);
            }

            return result;
        }

        /// <summary>
        /// Build predicate selecting ancestors for tree entry of given element.
        /// </summary>
        /// <param name="item">element to select ancestors for. </param>
        /// <param name="depth">Max inheritance level. </param>
        /// <param name="includeSelf">Whether or not to add element itself to result. </param>
        /// <returns>Predicate selecting ancestors of element. </returns>
        public static TreeFilter AncestorsOf(IHasTreeEntry item, int depth, bool includeSelf)
            => AncestorsOf(item.TreeEntry, depth, includeSelf);

        /// <summary>
        /// Returns predicate selecting descendants of given element.
        /// </summary>
        /// <param name="item">element to select ancestors for. </param>
        /// <param name="depth">Max inheritance level. </param>
        /// <param name="includeSelf">Whether or not to add element itself in result. </param>
        /// <returns>Descendants of element. </returns>
        public static TreeFilter DescendantsOf(TreeEntry item, int depth, bool includeSelf)
        {
            TreeFilter result;
            if (includeSelf)
            {
                result = r =>
                    item.Nv * r.Dv <= item.Dv * r.Nv &&
                    item.SNv * r.SDv >= item.SDv * r.SNv;
            }
            else
            {
                result = r =>
                    item.Nv * r.Dv < item.Dv * r.Nv &&
                    item.SNv * r.SDv > item.SDv * r.SNv;
            }

            if (depth > 0)
            {
                result = result.And(r => r.Depth - item.Depth <= depth);
            }

            return result;
        }

        /// <summary>
        /// Build predicate selecting descendants for tree entry of given element.
        /// </summary>
        /// <param name="item">element to select ancestors for. </param>
        /// <param name="depth">Max inheritance level. </param>
        /// <param name="includeSelf">Whether or not to add element itself to result. </param>
        /// <returns>Predicate selecting descendants of element. </returns>
        public static TreeFilter DescendantsOf(IHasTreeEntry item, int depth, bool includeSelf)
            => DescendantsOf(item.TreeEntry, depth, includeSelf);

        /// <summary>
        /// Build predicate selecting descendants for all given elements.
        /// </summary>
        /// <param name="items">elements to select ancestors for. </param>
        /// <param name="depth">Max inheritance level. </param>
        /// <param name="includeSelf">Whether or not to ad element itself to result. </param>
        /// <returns>Predicate selecting descendants of element. </returns>
        public static TreeFilter DescendantsOfAll(IEnumerable<TreeEntry> items, int depth,  bool includeSelf)
        {
            var filter = PredicateBuilder.False<TreeEntry>();

            foreach (var item in items)
            {
                filter = filter.Or(DescendantsOf(item, depth, includeSelf));
            }

            return filter;
        }

        /// <summary>
        /// Build predicate selecting descendants for tree entries of all given elements.
        /// </summary>
        /// <param name="items">elements to select ancestors for. </param>
        /// <param name="depth">Max inheritance level. </param>
        /// <param name="includeSelf">Whether or not to return element itself in result. </param>
        /// <typeparam name="T">Type of elements having tree entry. </typeparam>
        /// <returns>Predicate selecting descendants of element. </returns>
        public static TreeFilter DescendantsOfAll<T>(IEnumerable<T> items, int depth, bool includeSelf)
            where T : IHasTreeEntry =>
            DescendantsOfAll(items.Select(i => i.TreeEntry), depth, includeSelf);

        /// <summary>
        /// Build predicate selecting siblings of given element.
        /// </summary>
        /// <param name="item">Element to select siblings for. </param>
        /// <param name="includeSelf">Whether or not to add element itself to result. </param>
        /// <returns>Predicate selecting siblings of element. </returns>
        public static TreeFilter SiblingsOf(TreeEntry item, bool includeSelf)
        {
            TreeFilter result = r => r.SNv - r.Nv == item.SNv - item.Nv && r.SDv - r.Dv == item.SDv - item.Dv;
            if (!includeSelf)
            {
                result = result.And(r => r.Nv != item.Nv);
            }

            return result;
        }

        /// <summary>
        /// Build predicate selecting siblings for tree entry of given element.
        /// </summary>
        /// <param name="item">Element to select siblings for. </param>
        /// <param name="includeSelf">Whether or not to add element itself to result. </param>
        /// <returns>Predicate selecting siblings of element. </returns>
        public static TreeFilter SiblingsOf(IHasTreeEntry item, bool includeSelf)
            => SiblingsOf(item.TreeEntry, includeSelf);

        /// <summary>
        /// Build predicate selecting siblings positioned before given element.
        /// </summary>
        /// <param name="item">Element to select siblings for. </param>
        /// <param name="includeSelf">Whether or not to add element itself to result. </param>
        /// <returns>Predicate selecting siblings positioned before element. </returns>
        public static TreeFilter SiblingsBefore(TreeEntry item, bool includeSelf)
        {
            return SiblingsOf(item, includeSelf).And(r => r.Nv <= item.Nv);
        }

        /// <summary>
        /// Build predicate selecting siblings positioned before tree entry of given element.
        /// </summary>
        /// <param name="item">Element to select siblings for. </param>
        /// <param name="includeSelf">Whether or not to add element itself to result. </param>
        /// <returns>Predicate selecting siblings positioned before element. </returns>
        public static TreeFilter SiblingsBefore(IHasTreeEntry item, bool includeSelf)
            => SiblingsBefore(item.TreeEntry, includeSelf);

        /// <summary>
        /// Build predicate selecting siblings positioned after given element.
        /// </summary>
        /// <param name="item">Element to select siblings for. </param>
        /// <param name="includeSelf">Whether or not to add element itself to result. </param>
        /// <returns>Predicate selecting siblings positioned after element. </returns>
        public static TreeFilter SiblingsAfter(TreeEntry item, bool includeSelf)
        {
            return SiblingsOf(item, includeSelf).And(r => r.Nv >= item.Nv);
        }

        /// <summary>
        /// Build predicate selecting siblings positioned after tree entry of given element.
        /// </summary>
        /// <param name="item">Element to select siblings for. </param>
        /// <param name="includeSelf">Whether or not to add element itself to result. </param>
        /// <returns>Predicate selecting siblings positioned after element. </returns>
        public static TreeFilter SiblingsAfter(IHasTreeEntry item, bool includeSelf)
            => SiblingsAfter(item.TreeEntry, includeSelf);

        /// <summary>
        /// Build predicate selecting sibling element positioned next to tree entry of given element.
        /// </summary>
        /// <param name="item">Element to select next sibling for. </param>
        /// <returns>Predicate selecting  next siblings of element. </returns>
        public static TreeFilter NextSiblingOf(TreeEntry item)
        {
            return r => r.Nv == item.SNv && r.Dv == item.SDv;
        }

        /// <summary>
        /// Build predicate selecting sibling element positioned next to tree entry of given element.
        /// </summary>
        /// <param name="item">Element to select next sibling for. </param>
        /// <returns>Predicate selecting  next siblings of element. </returns>
        public static TreeFilter NextSiblingOf(IHasTreeEntry item)
            => NextSiblingOf(item.TreeEntry);

        /// <summary>
        /// Build predicate selecting first child element of given element.
        /// </summary>
        /// <param name="item">Element to select first child for. </param>
        /// <returns>Predicate selecting siblings positioned before element. </returns>
        public static TreeFilter FirstChildOf(TreeEntry item)
        {
            return NThChildOf(item, 1);
        }

        /// <summary>
        /// Build predicate selecting Nth child element of given element.
        /// </summary>
        /// <param name="item">Element to select child for. </param>
        /// <param name="n">Index of child element relative to parent, starting from 1. </param>
        /// <returns>Predicate selecting siblings positioned before element. </returns>
        public static TreeFilter NThChildOf(TreeEntry item, int n)
        {
            return r => r.Nv == item.Nv + (item.SNv * n) && r.Dv == item.Dv + (item.SDv * n);
        }

        /// <summary>
        /// Build predicate selecting root elements.
        /// </summary>
        /// <returns>Predicate selecting root elements. </returns>
        public static TreeFilter RootEntries()
        {
            return r => r.Dv == 1;
        }

        /// <summary>
        /// Build predicate selecting first root element.
        /// </summary>
        /// <returns>Predicate selecting first root element. </returns>
        public static TreeFilter FirstRootEntry()
        {
            return NthRootEntry(1);
        }

        /// <summary>
        /// Build predicate selecting Nth root element.
        /// </summary>
        /// <param name="n">Root element index. Starting from 1.</param>
        /// <returns>Predicate selecting Nth root element. </returns>
        public static TreeFilter NthRootEntry(int n)
        {
            return RootEntries().And(r => r.Nv == n);
        }
        
        /// <summary>
        /// Build predicate selecting elements by it's positions relative to parent.
        /// </summary>
        /// <example>
        /// For path {2, 1, 3} it will select first root item, then first child of 2nd root item, then third child of 2nd root's first child.
        /// </example>
        /// <param name="positionsPath">Elements positions path relative to it's parent. Position indexes starting from 1.</param>
        /// <returns>Predicate selecting list of elements by given path. </returns>
        public static TreeFilter ElementsByPositionsPath(IEnumerable<long> positionsPath)
        {
            TreeFilter conditions = PredicateBuilder.False<TreeEntry>();
            IIntervalQuadruple parentInterval = null;
            foreach (var position in positionsPath)
            {
                var childInterval = NestedIntervalMath.GetIntervalByPosition(parentInterval, position);
                conditions.Or(i => i.Nv == childInterval.Nv && i.Dv == childInterval.Dv);
                parentInterval = childInterval;
            }

            return conditions;
        }
    }
}