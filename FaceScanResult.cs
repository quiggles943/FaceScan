using FaceScan.Enums;
using FaceScan.Structures;

namespace FaceScan
{
    public class FaceScanResult
    {
        public List<FaceScanMatch> Matches { get; set; } = new List<FaceScanMatch>();
        public bool HasPositiveMatch 
        { 
            get 
            {
                return Matches.Any(x => x.MatchType == FaceScanResultType.PositiveMatch);
            } 
        }
        public float MaxConfidenceScore { get; set; } = 0.0f;
        public FaceScanResultType ResultType
        {
            get
            {
                if (HasPositiveMatch)
                    return FaceScanResultType.PositiveMatch;
                else if (Matches.Count > 0)
                    return FaceScanResultType.PotentialMatch;
                else
                    return FaceScanResultType.NoMatch;
            }
        }

        public FaceScanMatch? GetPositiveMatch()
        {
            if (Matches.Any(x => x.MatchType == FaceScanResultType.PositiveMatch))
            {
                return Matches.Find(x => x.MatchType == FaceScanResultType.PositiveMatch);
            }
            else
                return null;
        }

        public IEnumerable<FaceScanMatch> GetMatchesBySimilarityScore()
        {
            return Matches.OrderByDescending(x => x.SimilarityScore);
        }

    }
}
