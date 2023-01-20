using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.TRANSFORMATION_INTERFACE
{
    public interface ITransformationPlanning
    {
        Task<IReadOnlyList<TransformationFormulaDto>> GetAllVersionByItemCode(string itemcode);

        Task<bool> AddNewTransformationRequest(TransformationPlanning planning);
        Task<IReadOnlyList<TransformationPlanningDto>> GetAllListOfTransformationRequest();
        Task<bool> UpdateTransformPlanning(TransformationPlanning planning);
        Task<bool> AddNewTransformationRequirements(TransformationRequest request);
        Task<IReadOnlyList<TransformationWithRequirements>> GetAllRequirements(TransformationRequest request);
        Task<TransformationPlanningDto> ValidateTransformationPlanning(int id);
        Task<bool> ValidateAllRequirementsWithFormula(TransformationRequest Request);
        Task<bool> ValidateInputDate(string date);
        Task<IReadOnlyList<MaterialRequirements>> GetAllListOfRequirementsByTransformId(int id);
        Task<bool> ValidateStocksInRequirement(TransformationPlanning planning);
        Task<bool> ApproveTransformationRequest(TransformationPlanning planning);
        Task<IReadOnlyList<TransformationPlanningDto>> GetAllApprovedRequest();
        Task<IReadOnlyList<TransformationPlanningDto>> GetAllPendingRequest(string status);
        Task<IReadOnlyList<MaterialRequirements>> GetAllPendingRequestWithRequriements(int id);
        Task<bool> ValidateVersionInRequest(TransformationPlanning planning);
        Task<bool> ValidateFormulaCode(TransformationPlanning planning);
        Task<bool> CancelTransformationRequest(TransformationPlanning planning);
        Task<IReadOnlyList<TransformationPlanningDto>> GetAllCancelledRequest();
        Task<bool> AddNewTransformationReject(TransformationReject reject);
        Task<IReadOnlyList<TransformationRejectDto>> GetAllRequestForReject(int id);
        Task<bool> EditRejectTransformationPlanning(TransformationRequest request);
        Task<bool> RequestRejectTransformationRequest(TransformationPlanning planning);
        Task<bool> RejectTransformationRequest(TransformationReject reject);
        Task<bool> ValidateStatusRemarks(TransformationPlanning planning);
        Task<bool> ValidateIfApproved(int id);
        Task<bool> ValidatePlanningRequestIfPrepared(int id);
        Task<IReadOnlyList<TransformationPlanningDto>> GetAllPlanningRequest();
        Task<IReadOnlyList<TransformationFormulaDto>> GetAllItemCode();
        Task<decimal> GetAllItemsWithStock(string itemcode);
        Task<bool> ValidateRequirement(string itemCode, int batch, decimal quantity);
        Task<TransformationPlanningDto> GetBatchByTransformId(int id);
        Task<IReadOnlyList<TransformationWithRequirements>> GetRequirementsStock(string itemcode, int version);



        Task<IReadOnlyList<TransformationPlanningDto>> GetAllPendingRequestNotif();
        Task<IReadOnlyList<TransformationPlanningDto>> GetAllRejectRequestNotif();
        Task<IReadOnlyList<MaterialRequirements>> GetAllRejectRequirements(int id);

        Task<IReadOnlyList<MaterialRequirements>> GetAllCancelRequirements(int id);


        Task<IReadOnlyList<TransformationPlanningDto>> GetAllRejectRequest();

        Task<bool> ValidateIfPrepared(int id);

        Task<bool> ValidateIfDecimal(int batch);





    }
}
