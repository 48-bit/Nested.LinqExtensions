using System.Collections.Generic;

namespace Nested.LinqExtensions.Utils
{
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
    }
}