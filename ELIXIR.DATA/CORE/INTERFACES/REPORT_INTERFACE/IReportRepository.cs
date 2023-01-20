using ELIXIR.DATA.DTOs.REPORT_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE
{
    public interface IReportRepository
    {

        Task<IReadOnlyList<QCReport>> QcRecevingReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<WarehouseReport>> WarehouseRecivingReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<TransformationReport>> TransformationReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<MoveOrderReport>> MoveOrderReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<MiscellaneousReceiptReport>> MReceiptReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<MiscellaneousIssueReport>> MIssueReport(string DateFrom, string DateTo);

        Task<IReadOnlyList<MoveOrderReport>> TransactedMoveOrderReport(string DateFrom, string DateTo);

        Task<IReadOnlyList<WarehouseReport>> NearlyExpireItemsReport(int expirydays);

        Task<IReadOnlyList<CancelledOrderReport>> CancelledOrderedReports(string DateFrom, string DateTo);

        Task<IReadOnlyList<InventoryMovementReport>> InventoryMovementReport(string DateFrom, string DateTo, string PlusOne);


    }
}
