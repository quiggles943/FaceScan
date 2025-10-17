using FaceScan.Interfaces.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Interfaces
{
    public interface IFaceScanComparator
    {

        /// <summary>
        /// Compares two scanned face models and determines the similarity and match result between them.
        /// </summary>
        /// <remarks>The result type indicates whether the faces are considered a positive match, a
        /// potential match, or no match, based on predefined similarity thresholds.</remarks>
        /// <param name="detectedFace">The scanned face model to be compared.</param>
        /// <param name="comparison">The scanned face model to compare against.</param>
        /// <returns>A FaceScanComparisonResult containing the similarity score and the type of match result between the two face
        /// models.</returns>
        public FaceScanComparisonResult CompareFaceScanModels(IFaceModel detectedFace, IFaceModel comparison);

        /// <summary>
        /// Compares a scanned face model against a collection of face scans to identify potential and positive matches.
        /// </summary>
        /// <typeparam name="T">The type of the Scanned face model</typeparam>
        /// <param name="scannedFace">The scanned face to compare</param>
        /// <param name="faceScans">A list of facescans to compare the scanned face to</param>
        /// <returns></returns>
        public IIndividualFaceScanResult<T> CompareModelAgainstFaceScans<T>(T scannedFace, IEnumerable<T> faceScans) where T : IFaceModel;
    }
}
