using FaceScan.Interfaces;
using FaceScan.Interfaces.Enums;
using System.Collections.ObjectModel;

namespace FaceScan.Structures
{
    public class ScannedFaceWithClassifiers: IScannedFaceWithClassifiers
    {
        public FaceScanCoordinates Coordinates { get; }
        public IReadOnlyCollection<ILandmark> Landmarks { get; }
        public IReadOnlyCollection<float> Vectors { get; }
        public float? Confidence { get; }
        public DateTime ScanTime { get; } = DateTime.Now;

        public Gender? Gender { get; private set; }
        public int? Age { get; private set; }

        public string? Tag { get; set; }

        public ScannedFaceWithClassifiers(FaceScanCoordinates coordinates, List<FaceScanLandmark> landmarks, float[] vectors, float? confidence)
        {
            Coordinates = coordinates;
            Landmarks = new ReadOnlyCollection<FaceScanLandmark>(landmarks);
            Vectors = new ReadOnlyCollection<float>(vectors);
            Confidence = confidence;
        }

        public ScannedFaceWithClassifiers(IScannedFace scannedFace)
        {
            Coordinates = scannedFace.GetCoordinates();
            Landmarks = scannedFace.GetLandmarks();
            Vectors = scannedFace.GetVectors();
            Confidence = scannedFace.GetConfidence();
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

        public Gender? GetGender()
        {
            return Gender;
        }

        internal void SetGender(Gender gender)
        {
            Gender = gender;
        }

        internal void SetAge(int age)
        {
            Age = age;
        }
        public int? GetAge()
        {
            return Age;
        }
    }
}
