using FaceScan.Enums;
using FaceScan.Interfaces;
using FaceScan.Interfaces.Enums;
using FaceScan.Structures;

namespace FaceScan
{
    /// <summary>
    /// Default implementation of IIndividualFaceScanResult
    /// </summary>
    /// <typeparam name="TFace">The type of the Scanned face model</typeparam>
    public class IndividualFaceScanResult<TFace>: IIndividualFaceScanResult<TFace> where TFace : IFaceModel
    {
        public FaceScanMatch<TFace>? PositiveMatch { get; internal set; }
        public List<FaceScanMatch<TFace>> PotentialMatches { get; } = new List<FaceScanMatch<TFace>>();
        public bool HasPositiveMatch 
        { 
            get 
            {
                return PositiveMatch.HasValue;
            } 
        }
        public float MaxConfidenceScore { get; internal set; } = 0.0f;
        public FaceScanResultType ResultType
        {
            get
            {
                if (HasPositiveMatch)
                    return FaceScanResultType.PositiveMatch;
                else if (PotentialMatches.Count > 0)
                    return FaceScanResultType.PotentialMatch;
                else
                    return FaceScanResultType.NoMatch;
            }
        }

        public IndividualFaceScanResult()
        {

        }

        public IndividualFaceScanResult(FaceScanMatch<TFace>? positiveMatch, List<FaceScanMatch<TFace>> potentialMatches,float confidenceScore)
        {
            PositiveMatch = positiveMatch;
            if(potentialMatches != null)
                PotentialMatches = potentialMatches ?? new List<FaceScanMatch<TFace>>();
            MaxConfidenceScore = confidenceScore;
        }

        public FaceScanMatch<TFace>? GetPositiveMatch()
        {
            return PositiveMatch;
        }

        public float GetMaxConfidenceScore()
        {
            return MaxConfidenceScore;
        }

        public IEnumerable<FaceScanMatch<TFace>> GetPotentialMatchesBySimilarityScore()
        {
            return PotentialMatches.OrderByDescending(x => x.SimilarityScore);
        }

        public FaceScanResultType GetResultType()
        {
            return ResultType;
        }
    }
}
