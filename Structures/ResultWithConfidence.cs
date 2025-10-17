using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Structures
{
    public readonly struct ResultWithConfidence<TResult>
    {
        public TResult Result { get; }
        public float Confidence { get; }
        public ResultWithConfidence(TResult result, float confidence)
        {
            Result = result;
            Confidence = confidence;
        }
    }
}
