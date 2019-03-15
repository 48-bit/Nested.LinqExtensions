using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nested.LinqExtensions
{
    public static class EfDbSetExtensions
    {
        public static EntityEntry<T> AddRoot<T>(this DbSet<T> collection, T item) where T : class, IHasTreeEntry
        {
            var lastRoot = collection.Include(i => i.TreeEntry).LastRootOrDefault();
            var lastRootLocal = Queryable.AsQueryable<T>(collection.Local).LastRootOrDefault();

            var lastSnv = Math.Max(lastRoot?.TreeEntry?.SNv ?? 1, lastRootLocal?.TreeEntry?.SNv ?? 1);

            var treeEntry = new TreeEntry()
            {
                Depth = 1,
                Nv = lastSnv,
                Dv = 1,
                SNv = lastSnv + 1,
                SDv = 1
            };
            item.TreeEntry = treeEntry;

            return collection.Add(item);
        }

        public static EntityEntry<T> AddNextChild<T>(this DbSet<T> collection, T parent, T item)
            where T : class, IHasTreeEntry
        {
            if (parent == null)
            {
                return AddRoot(collection, item);
            }

            var parentEntry = parent.TreeEntry ?? 
                              collection.Include(i => i.TreeEntry)
                                  .First(i => i.TreeEntryId == parent.TreeEntryId).TreeEntry;

            var lastChildDb = collection.Include(i => i.TreeEntry).LastChildOrDefault(parent);
            var lastChildLocal = Queryable.AsQueryable<T>(collection.Local).LastChildOrDefault(parent);
            var lastChild = lastChildDb?.TreeEntry?.Nv > lastChildLocal?.TreeEntry?.Nv ? lastChildDb : lastChildLocal;

            var lastPosition = lastChild?.TreeEntry == null
                ? 0
                : (lastChild.TreeEntry.Nv - parentEntry.Nv) / parentEntry.SNv;

            var nextChildPosition = lastPosition + 1;

            var treeEntry = new TreeEntry()
            {
                Depth = parentEntry.Depth + 1,
                Nv = parentEntry.Nv + (nextChildPosition * parentEntry.SNv),
                Dv = parentEntry.Dv + (nextChildPosition * parentEntry.SDv),
                SNv = parentEntry.Nv + ((nextChildPosition + 1) * parentEntry.SNv),
                SDv = parentEntry.Dv + ((nextChildPosition + 1) * parentEntry.SDv)
            };
            item.TreeEntry = treeEntry;

            return collection.Add(item);
        }
    }
}