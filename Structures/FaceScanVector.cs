using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Structures
{
    public readonly struct FaceScanVector
    {
        public int Number { get; }
        public float Value { get; }
        public FaceScanVector(int number, float value)
        {
            Number = number;
            Value = value;
        }
    }
}
