using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Structures
{
    public readonly struct FaceScanLandmark
    {
        public float XCoordinate { get; }
        public float YCoordinate { get; }

        public FaceScanLandmark(float xCoordinate, float yCoordinate)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
        }
    }
}
