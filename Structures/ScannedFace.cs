using FaceScan.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.ObjectModel;
using System.Drawing;

namespace FaceScan.Structures
{
    public class ScannedFace: IScannedFace
    {
        public FaceScanCoordinates Coordinates { get; }
        public IReadOnlyCollection<FaceScanLandmark> Landmarks { get; }
        public IReadOnlyCollection<float> Vectors { get; }
        public float? Confidence { get; }
        public DateTime ScanTime { get; } = DateTime.Now;

        public string? Tag { get; set; }
        internal Image<Rgb24> FaceImage { get; }

        public ScannedFace(FaceScanCoordinates coordinates, List<FaceScanLandmark> landmarks, float[] vectors, float? confidence, Image<Rgb24> faceImage)
        {
            Coordinates = coordinates;
            Landmarks = new ReadOnlyCollection<FaceScanLandmark>(landmarks);
            Vectors = new ReadOnlyCollection<float>(vectors);
            Confidence = confidence;
            FaceImage = faceImage;
        }

        public IReadOnlyCollection<float> GetVectors()
        {
            return Vectors;
        }

        public IReadOnlyCollection<ILandmark> GetLandmarks()
        {
            return Landmarks;
        }

        public FaceScanCoordinates GetCoordinates()
        {
            return Coordinates;
        }

        public float? GetConfidence()
        {
            return Confidence;
        }
    }
}
