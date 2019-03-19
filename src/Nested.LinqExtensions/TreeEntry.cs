using System.Diagnostics;

namespace Nested.LinqExtensions
{
    [DebuggerDisplay("Left: {Nv}/{Dv}; Right: {SNv}/{SDv}")]
    public class TreeEntry: IntervalQuadruple
    {
        public TreeEntry()
        {
        }

        public TreeEntry(IntervalQuadruple bounds)
        {
            this.Depth = bounds.Depth;
            this.Nv = bounds.Nv;
            this.Dv = bounds.Dv;
            this.SNv = bounds.SNv;
            this.SDv = bounds.SDv;
        }

        public void SetFromInterval(IntervalQuadruple source)
        {
            this.Nv = source.Nv;
            this.Dv = source.Dv;
            this.SNv = source.SNv;
            this.SDv = source.SDv;
        }
    }
}