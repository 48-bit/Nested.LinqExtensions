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

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((IntervalQuadruple)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Nv.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Dv.GetHashCode();
                hashCode = (hashCode * 397) ^ this.SNv.GetHashCode();
                hashCode = (hashCode * 397) ^ this.SDv.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Depth;
                return hashCode;
            }
        }

        private bool Equals(IntervalQuadruple other)
        {
            return this.Nv == other.Nv && this.Dv == other.Dv && this.SNv == other.SNv && this.SDv == other.SDv &&
                   this.Depth == other.Depth;
        }
    }
}