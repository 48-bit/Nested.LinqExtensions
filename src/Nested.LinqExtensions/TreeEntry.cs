using System.Diagnostics;

namespace Nested.LinqExtensions
{
    [DebuggerDisplay("Left: {Nv}/{Dv}; Right: {SNv}/{SDv}")]
    public class TreeEntry: IntervalQuadruple
    {
        public TreeEntry()
        {
        }

        public long Id { get; set; }

        public TreeEntry(ISiblingQuadruple bounds)
        {
            this.Depth = bounds.Depth;
            this.Nv = bounds.Nv;
            this.Dv = bounds.Dv;
            this.SNv = bounds.SNv;
            this.SDv = bounds.SDv;
        }

        public void SetFromInterval(ISiblingQuadruple source)
        {
            this.Nv = source.Nv;
            this.Dv = source.Dv;
            this.SNv = source.SNv;
            this.SDv = source.SDv;
        }
    }
}