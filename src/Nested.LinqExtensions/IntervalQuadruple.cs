using System.Diagnostics;

namespace Nested.LinqExtensions
{
    /// <summary>
    /// Represent left (Nv/Dv) and right (SNv/SDv) bounds for nested interval encoding of hierarchical data.
    /// </summary>
    [DebuggerDisplay("Left: {Nv}/{Dv}; Right: {SNv}/{SDv}")]
    public class IntervalQuadruple : IIntervalQuadruple
    {

        /// <inheritdoc />
        public long Nv { get; set; }

        /// <inheritdoc />
        public long Dv { get; set; }

        /// <inheritdoc />
        public long SNv { get; set; }

        /// <inheritdoc />
        public long SDv { get; set; }

        /// <inheritdoc />
        public int Depth { get; set; }
    }
}