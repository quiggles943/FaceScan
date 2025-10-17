using FaceScan.Interfaces;

namespace FaceScan.Structures
{
    public class FaceScanLandmark: ILandmark, IEquatable<FaceScanLandmark>
    {
        public float XCoordinate { get; }
        public float YCoordinate { get; }

        public FaceScanLandmark(float xCoordinate, float yCoordinate)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
        }

        public float GetXCoordinate()
        {
            return XCoordinate;
        }

        public float GetYCoordinate()
        {
            return YCoordinate;
        }
        public override bool Equals(object? obj)
        {
            if (obj is FaceScanLandmark other)
                return Equals(other);
            else
                return false;
        }
        public bool Equals(FaceScanLandmark? other)
        {
            if(other == null) 
                return false;
            else
                return XCoordinate == other.XCoordinate && YCoordinate == other.YCoordinate;
        }
    }
}
