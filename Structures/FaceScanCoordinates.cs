using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Structures
{
    public readonly struct FaceScanCoordinates
    {
        public float FaceXCoordinate { get; }
        public float FaceYCoordinate { get; }
        public float Width { get; }
        public float Height { get; }

        public FaceScanCoordinates(float faceXCoordinate, float faceYCoordinate, float width, float height)
        {
            FaceXCoordinate = faceXCoordinate;
            FaceYCoordinate = faceYCoordinate;
            Width = width;
            Height = height;

        }
    }
}
