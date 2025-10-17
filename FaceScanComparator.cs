using FaceScan.Enums;
using FaceScan.Extensions;
using FaceScan.Interfaces;
using FaceScan.Interfaces.Enums;
using FaceScan.Structures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FaceScan
{
    public class FaceScanComparator: IFaceScanComparator
    {
        public float PositiveMatchThreashold { get; protected set; }
        public float PotentialMatchThreshold { get; protected set; }
        public PositiveScanReturnType PositiveScanReturnType { get; protected set; }
        private ILogger Logger = NullLogger<FaceScanComparator>.Instance;

        private FaceScanComparator()
        {

        }

        /// <summary>
        /// Compares a scanned face model against a collection of face scans to identify potential and positive matches.
        /// </summary>
        /// <typeparam name="T">The type of the Scanned face model</typeparam>
        /// <param name="scannedFace">The scanned face to compare</param>
        /// <param name="faceScans">A list of facescans to compare the scanned face to</param>
        /// <returns></returns>
        public IIndividualFaceScanResult<T> CompareModelAgainstFaceScans<T>(T scannedFace, IEnumerable<T> faceScans) where T : IFaceModel
        {
            ArgumentNullException.ThrowIfNull(scannedFace, nameof(scannedFace));
            ArgumentNullException.ThrowIfNull(faceScans, nameof(faceScans));
            var result = new IndividualFaceScanResult<T>();
            foreach (var face in faceScans)
            {
                var comparisonResult = CompareFaceScanModels(scannedFace, face);
                if(comparisonResult.ResultType == FaceScanResultType.NoMatch)
                {
                    continue;
                }
                FaceScanMatch<T> faceScanMatch = new FaceScanMatch<T>(face, comparisonResult);
                result.PotentialMatches.Add(new FaceScanMatch<T>(face, comparisonResult));

                if (comparisonResult.ResultType == FaceScanResultType.PositiveMatch)
                {
                    if (PositiveScanReturnType == PositiveScanReturnType.ReturnFirstMatch)
                    {
                        result.PositiveMatch = faceScanMatch;
                        result.MaxConfidenceScore = faceScanMatch.SimilarityScore;
                        break;
                    }
                    else if (PositiveScanReturnType == PositiveScanReturnType.ReturnBestMatch)
                    {
                        if (!result.PositiveMatch.HasValue || faceScanMatch.SimilarityScore > result.PositiveMatch.Value.SimilarityScore)
                        {
                            result.PositiveMatch = faceScanMatch;
                            result.MaxConfidenceScore = faceScanMatch.SimilarityScore;
                        }
                    }
                }
            }
            if (!result.HasPositiveMatch && result.PotentialMatches.Count > 0)
            {
                result.MaxConfidenceScore = result.PotentialMatches.Max(x => x.SimilarityScore);
            }
            return result;
        }

        /// <summary>
        /// Compares two scanned face models and determines the similarity and match result between them.
        /// </summary>
        /// <remarks>The result type indicates whether the faces are considered a positive match, a
        /// potential match, or no match, based on predefined similarity thresholds.</remarks>
        /// <param name="detectedFace">The scanned face model to be compared.</param>
        /// <param name="comparison">The scanned face model to compare against.</param>
        /// <returns>A FaceScanComparisonResult containing the similarity score and the type of match result between the two face
        /// models.</returns>
        public FaceScanComparisonResult CompareFaceScanModels(IFaceModel detectedFace, IFaceModel comparison)
        {
            ArgumentNullException.ThrowIfNull(detectedFace, nameof(detectedFace));
            ArgumentNullException.ThrowIfNull(comparison, nameof(comparison));

            var similarityScore = detectedFace.Similarity(comparison);
            var resultType = FaceScanResultType.NoMatch;
            if (similarityScore >= PositiveMatchThreashold)
            {
                resultType = FaceScanResultType.PositiveMatch;
            }
            else if (similarityScore >= PotentialMatchThreshold)
            {
                resultType = FaceScanResultType.PotentialMatch;
            }
            Logger.LogTrace("Comparison result. Similarity Score: {similarityScore}, Result Type: {resultType}", similarityScore, resultType);
            return new FaceScanComparisonResult(resultType, similarityScore);
        }

        public class Builder
        {
            private float positiveMatchThreashold = 0.42f;
            private float potentialMatchThreshold = 0.18f;
            private ILogger logger = new NullLogger<FaceScanComparator>();
            private PositiveScanReturnType positiveScanReturnType = PositiveScanReturnType.ReturnFirstMatch;

            public FaceScanComparator Build()
            {
                return new FaceScanComparator() { PositiveMatchThreashold = positiveMatchThreashold, PotentialMatchThreshold = potentialMatchThreshold, Logger = logger, PositiveScanReturnType = positiveScanReturnType };
            }

            /// <summary>
            /// Sets the positive match threshold used to determine when a face scan is considered a match. Default is 0.42.
            /// </summary>
            /// <param name="threashold">The minimum similarity score, as a floating-point value, required to consider two face scans a positive
            /// match. Must be between 0.0 and 1.0.</param>
            /// <returns>The current instance of <see cref="Builder"/> with the updated positive match
            /// threshold.</returns>
            public Builder WithPositiveMatchThreashold(float threashold)
            {
                positiveMatchThreashold = threashold;
                return this;
            }
            /// <summary>
            /// Sets the threshold value used to determine potential face matches during scan generation. Default is 0.18.
            /// </summary>
            /// <param name="threashold">The minimum similarity score, as a floating-point value between 0.0 and 1.0, required for a face to be
            /// considered a potential match.</param>
            /// <returns>The current instance of <see cref="Builder"/> with the updated potential match threshold.</returns>
            public Builder WithPotentialMatchThreshold(float threashold)
            {
                potentialMatchThreshold = threashold;
                return this;
            }

            public Builder WithLogger(ILogger logger)
            {
                this.logger = logger;
                return this;
            }

            public Builder WithPositiveScanReturnType(PositiveScanReturnType returnType)
            {
                positiveScanReturnType = returnType;
                return this;
            }
        }
    }
}
