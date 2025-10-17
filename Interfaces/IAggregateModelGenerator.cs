
using FaceScan.Enums;

namespace FaceScan.Interfaces
{
    public interface IAggregateModelGenerator
    {
        public IAggregateFaceModel CreateAggregateModel(IEnumerable<IFaceModel> faceModels);

        public TModel UpdateAggregateModel<TModel>(TModel existingModel, float[] updateVectors, AggregateFaceModelUpdateType updateType) where TModel : IAggregateFaceModel;
    }
}
