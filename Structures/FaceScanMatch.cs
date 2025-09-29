using FaceScan.Enums;

namespace FaceScan.Structures
{
    public readonly struct FaceScanMatch
    {
        public float SimilarityScore { get; }
        public FaceScanResultType MatchType { get; }

        public FaceScanMatch(float similarityScore, FaceScanResultType matchType)
        {
            SimilarityScore = similarityScore;
            MatchType = matchType;
        }

        public FaceScanMatch(FaceScanComparisonResult result)
        {
            SimilarityScore = result.SimilarityScore;
            MatchType = result.ResultType;
        }

        public override string ToString()
        {
            return MatchType.ToString()+" ("+SimilarityScore+") ";
        }
    }
}
