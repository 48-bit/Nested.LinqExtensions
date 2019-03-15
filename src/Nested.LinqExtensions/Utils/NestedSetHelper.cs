using System.Collections.Generic;

namespace Nested.LinqExtensions.Utils
{
    public static class NestedSetHelper
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
    }
}