using System;
using System.Collections.Generic;
using System.Linq;
using Nested.LinqExtensions;

namespace Nested.Samples.RegionsHierarchy
{
    public class Region : IHasTreeEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long TreeEntryId { get; set; }
        public TreeEntry TreeEntry { get; set; }
        public ICollection<City> Cities { get; set; }

        public static implicit operator Region(string name)
        {
            return new Region() {Name = name};
        }

        public static implicit operator Region(ValueTuple<string, string[]> item)
        {
            return new Region() {Name = item.Item1, Cities = item.Item2.Select(i => (City)i).ToList()};
        }

        
        public static implicit operator Region(ValueTuple<string, string> item)
        {
            return new Region() {Name = item.Item1, Cities = new City[] {item.Item2}};
        }
    }
}