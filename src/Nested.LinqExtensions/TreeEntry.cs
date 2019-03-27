using System.Diagnostics;

namespace Nested.LinqExtensions
{
    /// <summary>
    /// Base class representing hierarchical data in database.
    /// Hierarchy is defined with nested intervals encoding.
    /// </summary>
    [DebuggerDisplay("Left: {Nv}/{Dv}; Right: {SNv}/{SDv}")]
    public class TreeEntry : IntervalQuadruple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeEntry"/> class.
        /// Using bounds given by <see cref="IIntervalQuadruple"/>.
        /// </summary>
        /// <param name="bounds">left/right bounds specified as (nv/dv:snv/sdv) for nested intervals encoding. </param>
        public TreeEntry(IIntervalQuadruple bounds)
        {
            this.Depth = bounds.Depth;
            this.Nv = bounds.Nv;
            this.Dv = bounds.Dv;
            this.SNv = bounds.SNv;
            this.SDv = bounds.SDv;
        }

        /// <summary>
        /// Gets or Sets Tree entry unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Set Nv/Dv, SNv/SDv and Depth values from given interval.
        /// </summary>
        /// <param name="source">left/right bounds specified as (nv/dv:snv/sdv) for nested intervals encoding. </param>
        public void SetFromInterval(IIntervalQuadruple source)
        {
            this.Nv = source.Nv;
            this.Dv = source.Dv;
            this.SNv = source.SNv;
            this.SDv = source.SDv;
        }
    }
}