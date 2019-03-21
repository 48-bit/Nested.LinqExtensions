using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nested.LinqExtensions.Utils;

namespace Nested.LinqExtensions
{
    public static class EfDbSetExtensions
    {
        private static TreeEntry GetLastInsertedRootInterval<T>(DbSet<T> collection, bool includeLocal)
            where  T: class, IHasTreeEntry
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
            where T: class, IHasTreeEntry
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

        public static EntityEntry<T> AddRoot<T>(this DbSet<T> collection, T item) where T : class, IHasTreeEntry
        {

            return AddNextChild(collection, null, item);
        }

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
            state.Reference(i => i.TreeEntry).EntityEntry.State = EntityState.Detached;
            return state;
        }

        private static TreeEntry EnsureTreeEntryLoaded<T>(this DbSet<T> collection, T item)
            where T : class, IHasTreeEntry
        {
            if (item.TreeEntry != null) return item.TreeEntry;
            return collection
                .Include(i => i.TreeEntry)
                .First(i => i.TreeEntryId == item.TreeEntryId)
                .TreeEntry;

        }

        public static IEnumerable<TreeEntry> MoveSubtree<T>(this DbSet<T> collection, T from, T to)
            where T : class, IHasTreeEntry
        {
            var sourceInterval = collection.EnsureTreeEntryLoaded(from);
            var targetInterval = collection.EnsureTreeEntryLoaded(to);

            var relocation =
                NestedIntervalMath.BuildSubtreeRelocationMatrix(sourceInterval, targetInterval);

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
    }
}