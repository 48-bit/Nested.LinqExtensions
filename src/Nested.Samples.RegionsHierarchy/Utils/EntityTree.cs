using System;
using System.Collections.Generic;

namespace EFNested.Utils
{
    public class EntityTree<T>
    {
        public T Item { get; set; }
        public List<EntityTree<T>> Children { get; set; }
        public static implicit operator EntityTree<T>(ValueTuple<T,List<EntityTree<T>>> item)
        {
            return new EntityTree<T>() {Item = item.Item1, Children = item.Item2};
        }
    }
}