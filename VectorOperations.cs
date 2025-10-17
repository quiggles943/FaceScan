using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan
{
    public static class VectorOperations
    {

        /// <summary>
        /// Creates an average vector from a collection of vectors.
        /// </summary>
        /// <param name="vectors">The collection of vectors to generate an average from</param>
        /// <returns>A vector of the same size as the input vectors that represents an average of each vector point</returns>
        /// <exception cref="ArgumentException">Thrown if all the vectors are not of the same size</exception>
        public static float[] CreateAverageOfVectors(IEnumerable<float[]> vectors)
        {
            ArgumentNullException.ThrowIfNull(vectors, nameof(vectors));
            if (!vectors.Any())
            {
                throw new ArgumentException("No vectors provided.", nameof(vectors));
            }
            int length = vectors.First().Length;
            if (vectors.Any(v => v.Length != length))
            {
                throw new ArgumentException("Vectors must all be the same length.", nameof(vectors));
            }
            float[] average = new float[length];
            for(int i = 0; i < length; i++)
            {
                average[i] = vectors.Average(v => v[i]);
            }
            return average;
        }

        /// <summary>
        /// Adds a vector to an existing average, returning the new average.
        /// </summary>
        /// <param name="average">The current average vector</param>
        /// <param name="currentCount">The current number of unique vectors that make the average</param>
        /// <param name="vector">The new vector to add to the average</param>
        /// <returns>The updated average vector</returns>
        /// <exception cref="ArgumentException">Thrown if the provided vector is not of the same size as the average</exception>
        public static float[] AddVectorToAverage(float[] average, int currentCount, float[] vector)
        {
            ArgumentNullException.ThrowIfNull(average, nameof(average));
            ArgumentNullException.ThrowIfNull(vector, nameof(vector));
            if (average.Length != vector.Length)
            {
                throw new ArgumentException("Vectors must be the same length.", nameof(vector));
            }
            float[] newAverage = new float[average.Length];
            for (int i = 0; i < average.Length; i++)
            {
                var valueToAdd = (vector[i] - average[i]) / (currentCount + 1);
                newAverage[i] = average[i] + valueToAdd;
            }
            return newAverage;
        }

        public static float[] RemoveVectorFromAverage(float[] average, int currentCount, float[] vector)
        {
            ArgumentNullException.ThrowIfNull(average, nameof(average));
            ArgumentNullException.ThrowIfNull(vector, nameof(vector));
            if (average.Length != vector.Length)
            {
                throw new ArgumentException("Vectors must be the same length.", nameof(vector));
            }

            float[] newAverage = new float[average.Length];
            for (int i = 0; i < average.Length; i++)
            {
                newAverage[i] = ((average[i] * currentCount) - vector[i]) / (currentCount -1);
            }
            return newAverage;
        }
    }
}
