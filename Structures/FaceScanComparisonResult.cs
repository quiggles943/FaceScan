using FaceScan.Enums;

namespace FaceScan.Structures
{
    public readonly struct FaceScanComparisonResult
    {
        public FaceScanResultType ResultType { get; }
        public float SimilarityScore { get; }

        public FaceScanComparisonResult(FaceScanResultType resultType, float similarityScore)
        {
            ResultType = resultType;
            SimilarityScore = similarityScore;
        }
        public override string ToString()
        {
            return $"ResultType: {ResultType}, SimilarityScore: {SimilarityScore}";
        }
    }
}
