using System;
using System.Collections.Generic;
using System.Linq;

namespace Nested.Samples.RegionsHierarchy.Utils
{
    public class RegionTree : EntityTree<Region>
    {
        public static implicit operator RegionTree(ValueTuple<Region, List<RegionTree>> item)
        {
            return new RegionTree() {Item = item.Item1, Children = item.Item2?.Cast<EntityTree<Region>>()?.ToList()};
        }
        public static implicit operator RegionTree(ValueTuple<string, List<RegionTree>> item)
        {
            return new RegionTree() {Item = item.Item1, Children = item.Item2?.Cast<EntityTree<Region>>()?.ToList()};
        }
        public static implicit operator RegionTree(ValueTuple<ValueTuple<string,string>, List<RegionTree>> item)
        {
            return new RegionTree() {Item = item.Item1, Children = item.Item2?.Cast<EntityTree<Region>>()?.ToList()};
        }
        public static implicit operator RegionTree(ValueTuple<ValueTuple<string,string[]>, List<RegionTree>> item)
        {
            return new RegionTree() {Item = item.Item1, Children = item.Item2?.Cast<EntityTree<Region>>()?.ToList()};
        }
    }
}