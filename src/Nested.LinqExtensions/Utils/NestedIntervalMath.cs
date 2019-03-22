using System;
using System.Collections.Generic;

namespace Nested.LinqExtensions.Utils
{
    /// <summary>
    /// Helper functions to work with Nested intervals, Dan Hazel's encoding.
    /// </summary>
    public static class NestedIntervalMath
    {
        /// <summary>
        /// Build list of nv/dv values, each of them identifies ancestor tree entry for an item given by it's nv/dv.
        /// </summary>
        /// <param name="nv">Item Nv (numerator value). </param>
        /// <param name="dv">Item Dv (denominator value). </param>
        /// <returns>List of nv/dv values for item's ancestors. </returns>
        public static List<(long nv, long dv)> BuildAncestorsList(long nv, long dv)
        {
            List<(long nv, long dv)> result = new List<(long nv, long dv)>();

            long num = nv, denum = dv, ancnv = 0, ancdv = 1, ancsnv = 1, ancsdv = 0;
            while (num > 0 && denum > 0)
            {
                var div = num / denum;
                var mod = num % denum;
                ancnv += div * ancsnv;
                ancdv += div * ancsdv;
                ancsnv += ancnv;
                ancsdv += ancdv;
                result.Add((ancnv, ancdv));
                num = mod;
                if (num != 0)
                {
                    denum %= mod;
                    if (denum == 0)
                    {
                        denum = 1;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get Tree Entry interval values by it's position relative to parent.
        /// </summary>
        /// <param name="parent">parent item's tree entry interval. </param>
        /// <param name="index">Item's position relative to parent. </param>
        /// <returns>Tree entry interval values for an item. </returns>
        public static ISiblingQuadruple GetIntervalByPosition(ISiblingQuadruple parent, long index)
        {
            // returning intervals for root entry if parent is null
            if (parent == null)
            {
                return GetIntervalByPosition(index);
            }

            return new IntervalQuadruple()
            {
                Depth = parent.Depth + 1,
                Nv = parent.Nv + (index * parent.SNv),
                Dv = parent.Dv + (index * parent.SDv),
                SNv = parent.Nv + ((index + 1) * parent.SNv),
                SDv = parent.Dv + ((index + 1) * parent.SDv),
            };
        }

        /// <summary>
        /// Get Tree Entry parent interval values.
        /// </summary>
        /// <param name="element">Tree entry interval values. </param>
        /// <returns>Tree Entry Parent interval values. </returns>
        public static ISiblingQuadruple GetParentInterval(ISiblingQuadruple element)
        {
            var parent = new IntervalQuadruple()
            {
                SNv = element.SNv - element.Nv,
                SDv = element.SDv - element.Dv,
            };
            parent.Nv = element.Nv % parent.SNv;
            if (parent.SDv == 1)
            {
                parent.Dv = 1;
            }
            else
            {
                parent.Dv = element.Dv % parent.SDv;
            }

            parent.Depth = element.Depth - 1;
            return parent;
        }

        /// <summary>
        /// Get Root Tree Entry interval values by it's position.
        /// </summary>
        /// <param name="index">Item's position relative to parent. </param>
        /// <returns>Tree entry interval values for an item. </returns>
        public static ISiblingQuadruple GetIntervalByPosition(long index)
        {
            return new IntervalQuadruple()
            {
                Depth = 1,
                Dv = 1,
                SDv = 1,
                Nv = index,
                SNv = index + 1,
            };
        }

        /// <summary>
        /// Get Tree Entry interval by it's positions relative to parent.
        /// </summary>
        /// <example>
        /// For path {2, 1, 3} it will return item identified as third child of 2nd root's first child.
        /// </example>
        /// <param name="path">Elements positions path relative to it's parent. Position indexes starting from 1.</param>
        /// <returns>Tree Entry interval by it's positions path. </returns>
        public static ISiblingQuadruple GetIntervalByPath(IEnumerable<long> path)
        {
            var targetIntervalMatrix = new long[,]
            {
                {
                    0, 1,
                },
                {
                    1, 0,
                },
            };

            int depth = 0;
            foreach (var position in path)
            {
                depth += 1;
                targetIntervalMatrix = MultiplyMatrix(targetIntervalMatrix, PositionMatrix(position));
            }

            return new TreeEntry(MatrixToInterval(targetIntervalMatrix)) { Depth = depth };
        }

        /// <summary>
        /// Get Tree Entry position relative to it's parent.
        /// </summary>
        /// <param name="parent">Tree entry parent interval values. </param>
        /// <param name="entry">Tree entry interval values. </param>
        /// <returns>Tree Entry position relative to it's parent. </returns>
        public static long GetPositionByInterval(ISiblingQuadruple parent, ISiblingQuadruple entry)
        {
            if (entry == null)
            {
                // If entry is null return 0 which indicates index BEFORE FIRST element
                return 0;
            }

            if (parent == null)
            {
                return GetPositionByInterval(entry);
            }

            return (entry.Nv - parent.Nv) / parent.SNv;
        }

        /// <summary>
        /// Get  Root Tree Entry position.
        /// </summary>
        /// <param name="entry">Tree entry interval values. </param>
        /// <returns>Root Tree Entry position. </returns>
        public static long GetPositionByInterval(ISiblingQuadruple entry)
        {
            // Dv is always 1 for root entries.
            if (entry != null && entry.Dv != 1)
            {
                throw new ArgumentException($"Given entry nv/dv ({entry.Nv}/{entry.Dv}) does not identify root entry");
            }

            // If entry is null return 0 which indicates index BEFORE FIRST element
            // For root entries Nv == index;
            return entry?.Nv ?? 0;
        }

        /// <summary>
        /// Build matrix with multipliers to use for subtree relocation (move) .
        /// </summary>
        /// <param name="sourceRoot">Tree entry to move descendants from. </param>
        /// <param name="targetRoot">Tree entry to move descendants into. </param>
        /// <returns>Matrix containing multipliers to use for subtree relocation. </returns>
        public static long[,] BuildSubtreeRelocationMatrix(ISiblingQuadruple sourceRoot, ISiblingQuadruple targetRoot)
        {
            var sourceParent = GetParentInterval(sourceRoot);
            var targetParent = GetParentInterval(targetRoot);
            var sourcePosition = GetPositionByInterval(sourceParent, sourceRoot);
            var targetPosition = GetPositionByInterval(targetParent, targetRoot);

            return MultiplyMatrix(
                MultiplyMatrix(
                    IntervalToMatrix(targetParent),
                    PositionDifferenceMatrix(targetPosition, sourcePosition)),
                IntervalToInverseMatrix(sourceParent));
        }

        /// <summary>
        /// Multiply matrix to Tree Entry interval values.
        /// </summary>
        /// <param name="matrix">2x2 Matrix to multiply.</param>
        /// <param name="target">Tree entry interval values. </param>
        /// <returns>Tree entry interval values multiplied with given matrix. </returns>
        public static ISiblingQuadruple MultiplyMatrixToInterval(long[,] matrix, ISiblingQuadruple target)
        {
            return new IntervalQuadruple()
            {
                Nv = (matrix[0, 0] * target.Nv) + (matrix[0, 1] * target.Dv),
                Dv = (matrix[1, 0] * target.Nv) + (matrix[1, 1] * target.Dv),
                SNv = (matrix[0, 0] * target.SNv) + (matrix[0, 1] * target.SDv),
                SDv = (matrix[1, 0] * target.SNv) + (matrix[1, 1] * target.SDv),
            };
        }

        private static long[,] PositionDifferenceMatrix(long target, long source)
        {
            return new long[,]
            {
                {
                    1, 0,
                },
                {
                    target - source, 1,
                },
            };
        }

        private static long[,] PositionMatrix(long position)
        {
            return new long[,]
            {
                {
                    1, 1,
                },
                {
                    position, position + 1,
                },
            };
        }

        private static long[,] IntervalToMatrix(ISiblingQuadruple source)
        {
            return new long[,]
            {
                {
                    source.Nv, source.SNv,
                },
                {
                    source.Dv, source.SDv,
                },
            };
        }

        private static long[,] IntervalToInverseMatrix(ISiblingQuadruple source)
        {
            return new long[,]
            {
                {
                    -source.SDv, source.SNv,
                },
                {
                    source.Dv, -source.Nv,
                },
            };
        }

        private static ISiblingQuadruple MatrixToInterval(long[,] source)
        {
            return new IntervalQuadruple()
            {
                Nv = source[0, 0],
                Dv = source[1, 0],
                SNv = source[0, 1],
                SDv = source[1, 1],
            };
        }

        private static long[,] MultiplyMatrix(long[,] lhv, long[,] rhv)
        {
            if (
                lhv.GetLength(0) != 2 ||
                lhv.GetLength(1) != 2 ||
                rhv.GetLength(0) != 2 ||
                rhv.GetLength(1) != 2)
            {
                throw new ArgumentException();
            }

            long[,] result =
            {
                {
                    (lhv[0, 0] * rhv[0, 0]) + (lhv[0, 1] * rhv[1, 0]), (lhv[0, 0] * rhv[0, 1]) + (lhv[0, 1] * rhv[1, 1]),
                },
                {
                    (lhv[1, 0] * rhv[0, 0]) + (lhv[1, 1] * rhv[1, 0]), (lhv[1, 0] * rhv[0, 1]) + (lhv[1, 1] * rhv[1, 1]),
                },
            };
            return result;
        }
    }
}