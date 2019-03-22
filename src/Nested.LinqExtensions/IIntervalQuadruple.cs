namespace Nested.LinqExtensions
{
    /// <summary>
    /// Base Interface representing Tree entry interval (left and right) values.
    /// </summary>
    /// <remarks>
    /// For Nested Intervals encoding, each item have unique left value as Nv/Dv and right value or next sibling value as SNv/SDv.
    /// </remarks>
    public interface IIntervalQuadruple
    {
        /// <summary>
        /// Gets or sets interval left part numerator value.
        /// </summary>
        long Nv { get; set; }

        /// <summary>
        /// Gets or sets interval left part denominator value.
        /// </summary>
        long Dv { get; set; }

        /// <summary>
        /// Gets or sets interval right part (next sibling) numerator value.
        /// </summary>
        long SNv { get; set; }

        /// <summary>
        /// Gets or sets interval right part (next sibling) denominator value.
        /// </summary>
        long SDv { get; set; }

        /// <summary>
        /// Gets or sets elements depth (Distance from root entry to element) in hierarchy.
        /// </summary>
        int Depth { get; set; }
    }
}