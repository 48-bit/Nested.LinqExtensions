using System;
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
                parentEntry = parent.TreeEntry ?? 
                                  collection.Include(i => i.TreeEntry)
                                      .First(i => i.TreeEntryId == parent.TreeEntryId).TreeEntry;
            }
            var lastInterval = GetLastInsertedChildInterval(collection, true, parent);

            var nextPosition = NestedIntervalMath.GetPositionByInterval(parentEntry, lastInterval) + 1;

            var treeEntry = new TreeEntry(NestedIntervalMath.GetIntervalByPosition(parentEntry, nextPosition));
            item.TreeEntry = treeEntry;

            return collection.Add(item);
        }
    }
}