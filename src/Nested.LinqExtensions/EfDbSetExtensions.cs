using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nested.LinqExtensions.Utils;

namespace Nested.LinqExtensions
{
    /// <summary>
    /// Entity Framework DbSet extensions for inserting/updating hierarchical data.
    /// </summary>
    public static class EfDbSetExtensions
    {
        /// <summary>
        /// Add item as root to hierarchy.
        /// </summary>
        /// <param name="collection">Entity Framework Collection to add item. </param>
        /// <param name="item">Element to add to hierarchy. </param>
        /// <typeparam name="T">Type of elements in collection. Should Implement IHasTreeEntry. </typeparam>
        /// <returns>Entity Framework EntityEntry object. </returns>
        public static EntityEntry<T> AddRoot<T>(this DbSet<T> collection, T item)
            where T : class, IHasTreeEntry
        {
            return AddNextChild(collection, null, item);
        }

        /// <summary>
        /// Add item to hierarchy as next child of given parent item.
        /// </summary>
        /// <param name="collection">Entity Framework Collection to add item. </param>
        /// <param name="parent">Item's parent in hierarchy. </param>
        /// <param name="item">Element to add to hierarchy. </param>
        /// <typeparam name="T">Type of elements in collection. Should Implement IHasTreeEntry. </typeparam>
        /// <returns>Entity Framework EntityEntry object. </returns>
        public static EntityEntry<T> AddNextChild<T>(this DbSet<T> collection, T parent, T item)
            where T : class, IHasTreeEntry
        {
            TreeEntry parentEntry = null;
            if (parent != null)
            {
                parentEntry = collection.EnsureTreeEntryLoaded(parent);
            }

            var lastInterval = GetLastInsertedChildInterval(collection, true, parent);

            var nextPosition = NestedIntervalMath.GetPositionByInterval(parentEntry, lastInterval) + 1;

            var treeEntry = new TreeEntry(NestedIntervalMath.GetIntervalByPosition(parentEntry, nextPosition));
            item.TreeEntry = treeEntry;

            var state = collection.Add(item);

            // state.Reference(i => i.TreeEntry).EntityEntry.State = EntityState.Detached;
            return state;
        }

        /// <summary>
        /// Move descendant elements with parent node specified as 'from' to new parent node, specified as 'to'.
        /// </summary>
        /// <typeparam name="T">Type of elements. Should implement IHasTreeEntry interface. </typeparam>
        /// <param name="collection">Collection containing hierarchical data. </param>
        /// <param name="from">Parent node from where to move descendants. </param>
        /// <param name="to">Node to move descendants into. </param>
        /// <returns>List of TreeEntry items with recalculated positions. </returns>
        public static IEnumerable<TreeEntry> MoveSubtree<T>(this DbSet<T> collection, T from, T to)
            where T : class, IHasTreeEntry
        {
            var sourceInterval = collection.EnsureTreeEntryLoaded(from);
            var targetInterval = collection.EnsureTreeEntryLoaded(to);

            var relocation =
                NestedIntervalMath.BuildSubtreeRelocationMatrix(sourceInterval, targetInterval, 1);

            var depthDiff = targetInterval.Depth - sourceInterval.Depth;

            var elementsToUpdate = collection.Include(i => i.TreeEntry)
                .DescendantsOf(from)
                .ToList();

            var intervalsToUpdate = elementsToUpdate
                .Select(i => i.TreeEntry)
                .ToList();

            foreach (var item in elementsToUpdate)
            {
                item.TreeEntry.SetFromInterval(NestedIntervalMath.MultiplyMatrixToInterval(relocation, item.TreeEntry));
                item.TreeEntry.Depth += depthDiff;
            }

            collection.UpdateRange(elementsToUpdate);

            return intervalsToUpdate;
        }

        private static TreeEntry EnsureTreeEntryLoaded<T>(this DbSet<T> collection, T item)
            where T : class, IHasTreeEntry
        {
            if (item.TreeEntry != null)
            {
                return item.TreeEntry;
            }

            return collection
                .Include(i => i.TreeEntry)
                .First(i => i.TreeEntryId == item.TreeEntryId)
                .TreeEntry;
        }

        private static TreeEntry GetLastInsertedRootInterval<T>(DbSet<T> collection, bool includeLocal)
            where T : class, IHasTreeEntry
        {
            var lastRootDb = collection.Include(i => i.TreeEntry).LastRootOrDefault();
            if (!includeLocal)
            {
                return lastRootDb?.TreeEntry;
            }

            var lastRootLocal = collection.Local.AsQueryable().LastRootOrDefault();
            return (lastRootDb?.TreeEntry?.Nv > lastRootLocal?.TreeEntry?.Nv ? lastRootDb : lastRootLocal)?.TreeEntry;
        }

        private static IntervalQuadruple GetLastInsertedChildInterval<T>(DbSet<T> collection, bool includeLocal, T parent)
            where T : class, IHasTreeEntry
        {
            if (parent == null)
            {
                return GetLastInsertedRootInterval(collection, includeLocal);
            }

            var lastChildDb = collection.Include(i => i.TreeEntry).LastChildOrDefault(parent);

            if (!includeLocal)
            {
                return lastChildDb?.TreeEntry;
            }

            var lastChildLocal = collection.Local.AsQueryable().LastChildOrDefault(parent);
            return (lastChildDb?.TreeEntry?.Nv > lastChildLocal?.TreeEntry?.Nv ? lastChildDb : lastChildLocal)?.TreeEntry;
        }
    }
}