using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nested.LinqExtensions.Utils;

namespace Nested.LinqExtensions
{
    using TreeFilter = Expression<Func<TreeEntry, bool>>;
    public static class NestedIntervalsSpec
    {
        public static TreeFilter AncestorsOf(TreeEntry item, int depth, bool includeSelf)
        {
            TreeFilter result = null;
            
            if (includeSelf)
                result = r => 
                    item.Nv * r.Dv >= item.Dv * r.Nv &&
                    item.SNv * r.SDv <= item.SDv * r.SNv;
            else
                result = r => 
                    item.Nv * r.Dv > item.Dv * r.Nv &&
                    item.SNv * r.SDv < item.SDv * r.SNv;

            if (depth > 0)
                result = result.And(r => item.Depth - r.Depth <= depth);

            return result;
        }

        public static TreeFilter AncestorsOf(IHasTreeEntry item, int depth, bool includeSelf) 
            => AncestorsOf(item.TreeEntry, depth, includeSelf);

        public static TreeFilter DescendantsOf(TreeEntry item, bool includeSelf, int depth)
        {
            TreeFilter result = null;
            if (includeSelf)
                result = r =>
                    item.Nv * r.Dv <= item.Dv * r.Nv &&
                    item.SNv * r.SDv >= item.SDv * r.SNv;
            else
                result = r =>
                    item.Nv * r.Dv < item.Dv * r.Nv &&
                    item.SNv * r.SDv > item.SDv * r.SNv;

            if (depth > 0)
                result = result.And(r => r.Depth - item.Depth <= depth);

            return result;
        }

        public static TreeFilter DescendantsOf(IHasTreeEntry item, bool includeSelf, int depth) 
            => DescendantsOf(item.TreeEntry, includeSelf, depth);

        public static TreeFilter DescendantsOfAny(IEnumerable<TreeEntry> items, bool includeSelf, int depth)
        {
            var filter = PredicateBuilder.False<TreeEntry>();

            foreach (var item in items) filter = filter.Or(DescendantsOf(item, includeSelf, depth));

            return filter;
        }

        public static TreeFilter DescendantsOfAny<T>(IEnumerable<T> items, bool includeSelf, int depth)
            where T : IHasTreeEntry =>
            DescendantsOfAny(items.Select(i => i.TreeEntry), includeSelf, depth);

        public static TreeFilter SiblingsOf(TreeEntry item, bool includeSelf)
        {
            TreeFilter result = r => r.SNv - r.Nv == item.SNv - item.Nv && r.SDv - r.Dv == item.SDv - item.Dv;
            if (!includeSelf)
            {
                result = result.And(r => r.Id != item.Id);
            }

            return result;
        }

        public static TreeFilter SiblingsOf(IHasTreeEntry item, bool includeSelf)
            => SiblingsOf(item.TreeEntry, includeSelf);

        public static TreeFilter SiblingsBefore(TreeEntry item, bool includeSelf)
        {
            return SiblingsOf(item, includeSelf).And(r => r.Nv <= item.Nv);
        }

        public static TreeFilter SiblingsBefore(IHasTreeEntry item, bool includeSelf)
            => SiblingsBefore(item.TreeEntry, includeSelf);

        public static TreeFilter SiblingsAfter(TreeEntry item, bool includeSelf)
        {
            return SiblingsOf(item, includeSelf).And(r => r.Nv >= item.Nv);
        }

        public static TreeFilter SiblingsAfter(IHasTreeEntry item, bool includeSelf)
            => SiblingsAfter(item.TreeEntry, includeSelf);

        public static TreeFilter NextSiblingOf(TreeEntry item)
        {
            return r => r.Nv == item.SNv && r.Dv == item.SDv;
        }

        public static TreeFilter NextSiblingOf(IHasTreeEntry item)
            => NextSiblingOf(item.TreeEntry);

        public static TreeFilter FirstChildOf(TreeEntry item)
        {
            return NThChildOf(item, 1);
        }

        public static TreeFilter NThChildOf(TreeEntry item, int n)
        {
            return r => r.Nv == item.Nv + (item.SNv * n) && r.Dv == item.Dv + (item.SDv * n);
        }

        public static TreeFilter RootEntries()
        {
            return r => r.Dv == 1;
        }

        public static TreeFilter FirstRootEntry()
        {
            return NthRootEntry(0);
        }

        public static TreeFilter NthRootEntry(int n)
        {
            return RootEntries().And(r => r.Nv == n - 1);
        }

        public static TreeFilter ElementsByPositionsPath(IEnumerable<long> positionsPath)
        {
            TreeFilter conditions = PredicateBuilder.False<TreeEntry>();
            IntervalQuadruple parentInterval = null;
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