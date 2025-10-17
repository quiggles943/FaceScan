using FaceScan.Enums;
using FaceScan.Extensions;
using FaceScan.Interfaces;
using FaceScan.Structures;

namespace FaceScan
{
    public class AggregateModelGenerator: IAggregateModelGenerator
    {
        private AggregateModelGenerator()
        {

        }
        public IAggregateFaceModel CreateAggregateModel(IEnumerable<IFaceModel> faceModels)
        {
            IEnumerable<float[]> vectors = faceModels.Select(fm => fm.GetVectors().ToArray()).ToList();
            float[] averageVector = vectors.CreateAverage();
            return new AggregateFaceModel(faceModels.Count(), averageVector);
        }

        public TModel UpdateAggregateModel<TModel>(TModel existingModel, float[] updateVectors, AggregateFaceModelUpdateType updateType) where TModel : IAggregateFaceModel
        {
            return existingModel.UpdateAggregateModel(updateVectors,updateType);
        }

        public class Builder
        {
            public AggregateModelGenerator Build()
            {
                return new AggregateModelGenerator();
            }
        }
    }
}
