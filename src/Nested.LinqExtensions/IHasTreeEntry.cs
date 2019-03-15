namespace Nested.LinqExtensions
{
    public interface IHasTreeEntry
    {
        long TreeEntryId { get; set; }
        TreeEntry TreeEntry { get; set; }
    }
}