using FaceScan.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Interfaces
{
    public interface IScannedFace
    {
        public string? Tag { get; } 
        public FaceScanCoordinates Coordinates { get; }
        public IReadOnlyCollection<FaceScanLandmark> Landmarks { get; }
        public IReadOnlyCollection<float> Vectors { get; }
    }
}
