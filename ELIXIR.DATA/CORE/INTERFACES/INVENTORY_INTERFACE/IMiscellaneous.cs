using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.MISCELLANEOUS_DTOs;
using ELIXIR.DATA.DTOs.WAREHOUSE_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.INVENTORY_INTERFACE
{
    public interface IMiscellaneous
    {

        Task<bool> AddMiscellaneousReceipt(MiscellaneousReceipt receipt);
        Task<bool> AddMiscellaneousReceiptInWarehouse(WarehouseReceiving receive);
        Task<IReadOnlyList<MReceiptDto>> GetAllMiscellanousReceipt(bool status);
        Task<PagedList<MReceiptDto>> GetAllMReceiptWithPagination(UserParams userParams, bool status);
        Task<PagedList<MReceiptDto>> GetAllMReceiptWithPaginationOrig(UserParams userParams, string search, bool status);
        Task<bool> ActivateMiscellaenousReceipt(MiscellaneousReceipt receipt);
        Task<bool> InActivateMiscellaenousReceipt(MiscellaneousReceipt receipt);
        Task<IReadOnlyList<MReceiptDto>> GetWarehouseDetailsByMReceipt(int id);
        Task<bool> AddMiscellaneousIssue(MiscellaneousIssue issue);
        Task<bool> AddWarehouseReceiveForReceipt(WarehouseReceiving warehouse);

        Task<IReadOnlyList<MIssueDto>> GetAvailableStocksForIssue(string itemcode);
        Task<bool> AddMiscellaneousIssueDetails(MiscellaneousIssueDetails details);

        Task<PagedList<MIssueDto>> GetAllMIssueWithPagination(UserParams userParams, bool status);
        Task<PagedList<MIssueDto>> GetAllMIssueWithPaginationOrig(UserParams userParams, string search, bool status);

        Task<bool> ActivateMiscellaenousIssue(MiscellaneousIssue issue);
        Task<bool> InActivateMiscellaenousIssue(MiscellaneousIssue issue);

        Task<IReadOnlyList<MIssueDto>> GetAllDetailsInMiscellaneousIssue(int id);

        Task<bool> UpdateIssuePKey(MiscellaneousIssueDetails details);

        Task<IReadOnlyList<MIssueDto>> GetAllAvailableIssue(int empid);

        Task<bool> CancelIssuePerItemCode(MiscellaneousIssueDetails issue);

        Task<bool> ValidateMiscellaneousReceiptInIssue(MiscellaneousReceipt receipt);


    }
}
