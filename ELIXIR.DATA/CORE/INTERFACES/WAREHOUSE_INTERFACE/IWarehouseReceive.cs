using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;
using ELIXIR.DATA.DTOs.WAREHOUSE_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE
{
    public interface IWarehouseReceive
    {

        Task<bool> ReceiveMaterialsFromWarehouse(WarehouseReceiving warehouse);
        Task<WareHouseScanBarcode> GetScanBarcodeByReceivedId(string itemcode);
        Task<bool> AddMaterialsInWarehouse(WarehouseReceiving warehouse);
        Task<IReadOnlyList<RejectWarehouseReceivingDto>> GetAllRejectRawmaterialsInWarehouse();
        Task<bool> AddRejectMaterialsInWarehouse(Warehouse_Reject reject);
        Task<bool> RejectRawMaterialsByWarehouse(Warehouse_Reject reject);
        Task<bool> ScanBarcode(WarehouseReceiving warehouse);
        Task<IReadOnlyList<WarehouseReceivingDto>> ListForWarehouseReceiving();
        Task<bool> ReturnRawmaterialsByWarehouse(PO_Receiving receiving);
        Task<bool> ValidateActualAndRejectInput (WarehouseReceiving warehouse);

        Task<IReadOnlyList<WarehouseRejectDto>> GetAllRejectedMaterialsByWarehouseId(int id);
        Task<bool> ValidateTotalReject(Warehouse_Reject reject);

        Task<IReadOnlyList<WarehouseReceived>> GetAllWarehouseReceived(string DateFrom, string DateTo);
        Task<bool> CancelAndReturnMaterialsForWarehouseReceive(WarehouseReceiving receive);

        Task<bool> CancelAndReturnMaterialsInPoSummary(WarehouseReceiving receive);


        Task<PagedList<WarehouseReceivingDto>> ListForWarehouseReceivingWithPagination(UserParams userParams);
        Task<PagedList<WarehouseReceivingDto>> ListForWarehouseReceivingWithPaginationOrig(UserParams userParams, string search);


        Task<PagedList<RejectWarehouseReceivingDto>> RejectRawMaterialsByWarehousePagination(UserParams userParams);
        Task<PagedList<RejectWarehouseReceivingDto>> RejectRawMaterialsByWarehousePaginationOrig(UserParams userParams, string search);

        Task<IReadOnlyList<WarehouseReceivingDto>> ListOfWarehouseReceivingId();
        Task<IReadOnlyList<WarehouseReceivingDto>> ListOfWarehouseReceivingId(string search);

        Task<PagedList<WarehouseReceivingDto>> GetAllWarehouseIdWithPagination(UserParams userParams);
        Task<PagedList<WarehouseReceivingDto>> GetAllWarehouseIdWithPaginationOrig(UserParams userParams, string search);

    }
}
