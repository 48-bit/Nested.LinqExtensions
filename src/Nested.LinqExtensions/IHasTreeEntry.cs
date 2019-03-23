namespace Nested.LinqExtensions
{
    /// <summary>
    /// Base interface for hierarchical elements.
    /// Provides reference to TreeEntry object containing information about position in hierarchy.
    /// </summary>
    public interface IHasTreeEntry
    {
        /// <summary>
        /// Gets or sets TreeEntry Id.
        /// </summary>
        long TreeEntryId { get; set; }

        /// <summary>
        /// Gets or sets TreeEntry reference object.
        /// </summary>
        TreeEntry TreeEntry { get; set; }
    }
}