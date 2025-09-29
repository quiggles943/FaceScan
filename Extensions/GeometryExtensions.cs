using SimpleSimd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Extensions
{
    public static class GeometryExtensions
    {
        /// <summary>Returns the dot product of two vectors.</summary>
        /// <param name="x">The first vector.</param>
        /// <param name="y">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static float Dot(this IEnumerable<float> x, IEnumerable<float> y)
        {
            return SimdOps<float>.Dot(x.ToArray(), y.ToArray());
        }
    }
}
