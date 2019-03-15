namespace Nested.LinqExtensions
{
    public interface ISiblingQuadruple
    {
        long Nv { get; set; }
        long Dv { get; set; }
        long SNv { get; set; }
        long SDv { get; set; }
        int Depth { get; set; }
    }
}