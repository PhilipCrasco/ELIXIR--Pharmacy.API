using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.TRANSFORMATION_INTERFACE
{
    public interface ITransformationPreparation
    {


        Task<IReadOnlyList<TransformationPreparationDto>> GetAllListOfTransformationByTransformationId(TransformationPlanning planning);

        Task<bool> PrepareTransformationMaterials(TransformationPreparation preparation);
        Task<bool> AddPreparationMaterials(TransformationPreparation preparation);


        Task<bool> UpdatePrepareStatusInRequest(int id);
        Task<bool> ValidatePreparedMaterials(int id, string code); 
        Task<bool> ValidateIfApproved(int id);
        Task<IReadOnlyList<TransformationPlanningDto>> GetAllListOfTransformationRequestForMixing();
        Task<IReadOnlyList<TransformationMixingRequirements>> GetAllRequirementsForMixing(int id);

        Task<bool> AddMixingTransformation(WarehouseReceiving warehouse);
        Task<bool> FinishedMixedMaterialsForWarehouse(WarehouseReceiving warehouse, int id);

        Task<RawmaterialDetailsFromWarehouseDto>GetReceivingDetailsForRawmaterials(int id, string code);
        Task<bool> UpdatedWarehouseStock(string code);


        Task<IReadOnlyList<TransformationPlanningDto>> GetAllTransformationFormulaInformation();

        Task<decimal> ValidatePreparedItems(TransformationPreparation preparation);

        Task<IReadOnlyList<ForTesting>> GetAllAvailableStocks();

        Task<IReadOnlyList<ItemStocks>> GetAllRemainingStocksPerReceivingId(string itemcode);


        Task<RawmaterialDetailsFromWarehouseDto> GetQuantityAndBatch(int id, string code);

        Task<bool> UpdateRequestStatus(TransformationPreparation preparation);

        Task<bool> UpdatePlanningStatus(TransformationPreparation preparation);


        Task<bool> ValidateRequestAndPreparation(int id);


        Task<PagedList<TransformationPlanningDto>> GetAllTransformationFormulaInformationPagination(UserParams userParams);


        Task<IReadOnlyList<TransformationPlanningDto>> GetAllTransformationForMixing();


        Task<PagedList<TransformationPlanningDto>> GetAllTransformationForMixingPagination(UserParams userParams);


        Task<bool> CompareBatchCount(int id);

        Task<MixingValue> CountBatch(int id);



    }
}
