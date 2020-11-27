using Nested.LinqExtensions.Utils;
using NUnit.Framework;

namespace Nested.LinqExtensions.Tests
{
    public class NestedMath_Tests
    {
        [Test]
        public void RelocatedTreeHasSameValuesAsCreated()
        {
            var firstRoot = NestedIntervalMath.GetIntervalByPosition(1);
            var secondRoot = NestedIntervalMath.GetIntervalByPosition(2);
            var firstChild = NestedIntervalMath.GetIntervalByPosition(firstRoot, 1);
            var firstChildChild = NestedIntervalMath.GetIntervalByPosition(firstChild, 1);
            var secondChild = NestedIntervalMath.GetIntervalByPosition(secondRoot, 1);
            var secondchildchild = NestedIntervalMath.GetIntervalByPosition(secondChild, 1);

            var relocationMatrix = NestedIntervalMath.BuildSubtreeRelocationMatrix(firstRoot, secondRoot, 1); 
            var firstRootRelocated =
                NestedIntervalMath.MultiplyMatrixToInterval(relocationMatrix, firstRoot);
            var firstChildRelocated = NestedIntervalMath.MultiplyMatrixToInterval(relocationMatrix, firstChild);
            

        }
    }
}