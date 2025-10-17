using FaceScan.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Structures
{
    public class AggregateFaceModel: IAggregateFaceModel
    {
        public int ModelCount { get; private set; }
        public IReadOnlyCollection<float> Vectors { get; private set; }

        public AggregateFaceModel(int modelCount, float[] vectors)
        {
            ModelCount = modelCount;
            Vectors = new ReadOnlyCollection<float>(vectors);
        }

        public int GetModelCount()
        {
            return ModelCount;
        }

        public void UpdateModel(float[] vectors, int modelCount)
        {
            Vectors = new ReadOnlyCollection<float>(vectors);
            ModelCount = modelCount;
        }

        public IReadOnlyCollection<float> GetVectors()
        {
            return Vectors;
        }
    }
}
