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
            var firstChild = NestedIntervalMath.GetIntervalByPosition(firstRoot, 2);
            var firstChildChild = NestedIntervalMath.GetIntervalByPosition(firstChild, 1);
            
            var secondRoot = NestedIntervalMath.GetIntervalByPosition(2);
            var secondChild = NestedIntervalMath.GetIntervalByPosition(secondRoot, 5); // < - should be same as relocated firstroot
            var secondchildchild = NestedIntervalMath.GetIntervalByPosition(secondChild, 2); // < - should be same as relocated firstchild
            var second3child = NestedIntervalMath.GetIntervalByPosition(secondchildchild, 1); // < - should be same as relocated firstchildchild

            var relocationMatrix = NestedIntervalMath.BuildSubtreeRelocationMatrix(firstRoot, secondRoot, 5); 
            var firstRootRelocated =
                NestedIntervalMath.MultiplyMatrixToInterval(relocationMatrix, firstRoot);
            var firstChildRelocated = NestedIntervalMath.MultiplyMatrixToInterval(relocationMatrix, firstChild);

            var firstChildChildRelocated =
                NestedIntervalMath.MultiplyMatrixToInterval(relocationMatrix, firstChildChild);
            
            firstRootRelocated.Depth = 2; // Just set depth for equality test. it's not computed in math multiplication
            firstChildRelocated.Depth = 3;
            firstChildChildRelocated.Depth = 4;
            Assert.AreEqual(secondChild, firstRootRelocated);
            Assert.AreEqual(secondchildchild, firstChildRelocated);
            Assert.AreEqual(second3child, firstChildChildRelocated);
        }
    }
}