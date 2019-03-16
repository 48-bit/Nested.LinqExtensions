namespace Nested.LinqExtensions
{
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

        public long Id { get; set; }
    }
}