using FaceScan.Interfaces;
using FaceScan.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Extensions
{
    public static class FaceScanExtensions
    {
        public static float Similarity(this IScannedFace a, IScannedFace b)
        {
            ArgumentNullException.ThrowIfNull(a, nameof(a));
            ArgumentNullException.ThrowIfNull(b, nameof(b));
            if (a.GetVectors().Count != b.GetVectors().Count)
            {
                throw new ArgumentException("Vector lengths do not match.");
            }
            return a.GetVectors().Dot(b.GetVectors());
        }
    }
}
