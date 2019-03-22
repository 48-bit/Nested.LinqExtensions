using System.Diagnostics;

namespace Nested.LinqExtensions
{
    [DebuggerDisplay("Left: {Nv}/{Dv}; Right: {SNv}/{SDv}")]
    public class IntervalQuadruple : IIntervalQuadruple
    {
        public long Nv { get; set; }
        public long Dv { get; set; }
        public long SNv { get; set; }
        public long SDv { get; set; }
        public int Depth { get; set; }
    }
}