using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;
using ELIXIR.DATA.DTOs.REPORT_DTOs;
using ELIXIR.DATA.DTOs.WAREHOUSE_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE
{
    public interface IReceivingRepository
    {
        Task<bool> AddNewReceivingInformation(PO_Receiving receive);
        Task<bool> AddNewRejectInfo(PO_Reject reject);
        Task<bool> UpdateReceivingInfo(PO_Receiving receiving);
        Task<bool> UpdateRejectInfo(PO_Reject reject);
        Task<bool> CancelPo(ImportPOSummary summary);
        Task<bool> ReturnPoInAvailableList(ImportPOSummary summary);
      
        Task<IReadOnlyList<PoSummaryChecklistDto>> GetAllAvailablePo();
        Task<IReadOnlyList<CancelledPoDto>> GetAllCancelledPo();
        Task<IReadOnlyList<NearlyExpireDto>> GetAllNearlyExpireRawMaterial();
        Task<bool> ApproveNearlyExpireRawMaterials(PO_Receiving receive);
        Task<IReadOnlyList<WarehouseReceivingDto>> GetAllRawMaterialsForWarehouseReceiving();
        Task<bool> CancelPartialRecevingInQC(PO_Receiving receiving);

        Task<bool> RejectRawMaterialsNearlyExpire(PO_Receiving receiving);
        Task<bool> WarehouseConfirmRejectByQc(WarehouseReceiving warehouse);
        Task<bool> WarehouseReturnRejectByQc(PO_Receiving receiving);
        Task<bool> ValidateActualRemaining(PO_Receiving receiving);
        Task<IReadOnlyList<RejectWarehouseReceivingDto>> GetAllWarehouseConfirmReject();
        Task<bool> ValidateForCancelPo(ImportPOSummary summary);
        Task<PagedList<PoSummaryChecklistDto>> GetAllPoSummaryWithPagination(UserParams userParams);
        Task<PagedList<PoSummaryChecklistDto>> GetPoSummaryByStatusWithPaginationOrig(UserParams userParams, string search);
        Task<PagedList<WarehouseReceivingDto>> GetAllAvailableForWarehouseWithPagination(UserParams userParams);
        Task<PagedList<WarehouseReceivingDto>> GetAllAvailableForWarehouseWithPaginationOrig(UserParams userParams, string search);
        Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPagination (UserParams userParams);
        Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPaginationOrig(UserParams userParams, string search);
        Task<PagedList<NearlyExpireDto>> GetAllNearlyExpireWithPagination(UserParams userParams);
        Task<PagedList<NearlyExpireDto>> GetAllNearlyExpireWithPaginationOrig(UserParams userParams, string search);
        Task<PagedList<RejectWarehouseReceivingDto>> GetAllConfirmRejectWithPagination(UserParams userParams);
        Task<PagedList<RejectWarehouseReceivingDto>> GetAllConfirmRejectWithPaginationOrig(UserParams userParams, string search);


        // Validation 

        Task<bool> ValidatePoId(int id);

        Task<IReadOnlyList<NearlyExpireDto>> GetItemDetailsForNearlyExpire(int id);

        Task<bool> ValidatePOForCancellation(int id);

    }
}
