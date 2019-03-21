using System;
using System.Collections.Generic;

namespace Nested.LinqExtensions.Utils
{
    /// <summary>
    /// Helper functions to work with Nested intervals, Dan Hazel's encoding. 
    /// </summary>
    public static class NestedIntervalMath
    {
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

        public static IntervalQuadruple GetIntervalByPosition(IntervalQuadruple parent, long index)
        {
            if (parent == null) //returning intervals for root entry if parent is null
            {
                return GetIntervalByPosition(index);
            }

            return new IntervalQuadruple()
            {
                Depth = parent.Depth + 1,
                Nv = parent.Nv + (index * parent.SNv),
                Dv = parent.Dv + (index * parent.SDv),
                SNv = parent.Nv + ((index + 1) * parent.SNv),
                SDv = parent.Dv + ((index + 1) * parent.SDv)
            };
        }

        public static IntervalQuadruple GetParentInterval(IntervalQuadruple element)
        {
            var parent = new IntervalQuadruple()
            {
                SNv = element.SNv - element.Nv,
                SDv = element.SDv - element.Dv
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

        public static IntervalQuadruple GetIntervalByPosition(long index)
        {
            return new IntervalQuadruple()
            {
                Depth = 1,
                Dv = 1,
                SDv = 1,
                Nv = index,
                SNv = index + 1
            };
        }

        public static long GetPositionByInterval(IntervalQuadruple parent, IntervalQuadruple entry)
        {
            if (entry == null)
            {
                //If entry is null return 0 which indicates index BEFORE FIRST element
                return 0;
            }

            if (parent == null)
            {
                return GetPositionByInterval(entry);
            }

            return (entry.Nv - parent.Nv) / parent.SNv;
        }

        public static long GetPositionByInterval(IntervalQuadruple entry)
        {
            //If entry is null return 0 which indicates index BEFORE FIRST element
            //For root entries Nv == index;
            return entry?.Nv ?? 0;
        }

        public static long[,] BuildSubtreeRelocationMatrix(
            IntervalQuadruple sourceRoot,
            IntervalQuadruple targetRoot)
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

        public static IntervalQuadruple MultiplyMatrixToInterval(long[,] matrix, IntervalQuadruple target)
        {
            return new IntervalQuadruple()
            {
                Nv = matrix[0, 0] * target.Nv + matrix[0, 1] * target.Dv,
                Dv = matrix[1, 0] * target.Nv + matrix[1, 1] * target.Dv,
                SNv = matrix[0, 0] * target.SNv + matrix[0, 1] * target.SDv,
                SDv = matrix[1, 0] * target.SNv + matrix[1, 1] * target.SDv,
            };
        }

        private static long[,] PositionDifferenceMatrix(long target, long source)
        {
            return new long[,]
            {
                {1,               0},
                {target - source, 1}
            };

        }

        public static IntervalQuadruple GetIntervalByPath(IEnumerable<long> path)
        {
            var targetIntervalMatrix = new long[,]
            {
                {0, 1},
                {1, 0}
            };
            int depth = 0;
            foreach (var position in path)
            {
                depth += 1;
                targetIntervalMatrix = MultiplyMatrix(targetIntervalMatrix, PositionMatrix(position));
            }
            return new TreeEntry(MatrixToInterval(targetIntervalMatrix)) {Depth = depth};

        }

        private static long[,] PositionMatrix(long position)
        {
            return new long[,]
            {
                { 1,        1           },
                { position, position +1 }
            };
        }

        public static long[,] IntervalToMatrix(IntervalQuadruple source)
        {
            return new long[,]
            {
                { source.Nv, source.SNv },
                { source.Dv, source.SDv }
            };
        }

        public static long[,] IntervalToInverseMatrix(IntervalQuadruple source)
        {
            return new long[,]
            {
                {-source.SDv, source.SNv},
                {source.Dv,   -source.Nv}
            };
        }

        public static IntervalQuadruple MatrixToInterval(long[,] source)
        {
            return new IntervalQuadruple()
            {
                Nv = source[0, 0],
                Dv = source[1, 0],
                SNv = source[0, 1],
                SDv = source[1, 1]
            };
        }

        public static long[,] MultiplyMatrix(long[,] lhv, long[,] rhv)
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
                {lhv[0, 0] * rhv[0, 0] + lhv[0, 1] * rhv[1, 0], lhv[0, 0] * rhv[0, 1] + lhv[0, 1] * rhv[1, 1]},
                {lhv[1, 0] * rhv[0, 0] + lhv[1, 1] * rhv[1, 0], lhv[1, 0] * rhv[0, 1] + lhv[1, 1] * rhv[1, 1]}
            };
            return result;
        }
    }
}