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

        /// <summary>
        /// Creates an average vector from a collection of vectors.
        /// </summary>
        /// <param name="vectors">The collection of vectors to generate an average from</param>
        /// <returns>A vector of the same size as the input vectors that represents an average of each vector point</returns>
        public static float[] CreateAverage(this IEnumerable<float[]> vectors)
        {
            return VectorOperations.CreateAverageOfVectors(vectors);
        }

        /// <summary>
        /// Adds a vector to an existing average, returning the new average.
        /// </summary>
        /// <param name="average">The current average vector</param>
        /// <param name="currentCount">The current number of unique vectors that make the average</param>
        /// <param name="vector">The new vector to add to the average</param>
        /// <returns>The updated average vector</returns>
        public static float[] AddToAverage(this float[] average, int count, float[] vector)
        {
            return VectorOperations.AddVectorToAverage(average, count, vector);
        }

        /// <summary>
        /// Removes a vector to an existing average, returning the new average.
        /// </summary>
        /// <param name="average">The current average vector</param>
        /// <param name="currentCount">The current number of unique vectors that make the average</param>
        /// <param name="vector">The vector to remove from the average</param>
        /// <returns>The updated average vector</returns>
        public static float[] RemoveFromAverage(this float[] average, int count, float[] vector)
        {
            return VectorOperations.RemoveVectorFromAverage(average, count, vector);
        }
    }
}
