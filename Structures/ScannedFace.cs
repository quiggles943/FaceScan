using FaceScan.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Structures
{
    public class ScannedFace: IScannedFace
    {
        public FaceScanCoordinates Coordinates { get; }
        public IReadOnlyCollection<FaceScanLandmark> Landmarks { get; }
        public IReadOnlyCollection<float> Vectors { get; }
        public DateTime ScanTime { get; } = DateTime.Now;

        public string? Tag { get; set; }

        public ScannedFace(FaceScanCoordinates coordinates, List<FaceScanLandmark> landmarks, float[] vectors)
        {
            Coordinates = coordinates;
            Landmarks = new ReadOnlyCollection<FaceScanLandmark>(landmarks);
            Vectors = new ReadOnlyCollection<float>(vectors);
        }
    }
}
