using ELIXIR.DATA.CORE.ICONFIGURATION;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.REPORT_CONTROLLER
{

    public class ReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpGet]
        [Route("QcReceivingReport")]
        public async Task<IActionResult> QcReceivingReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var orders = await _unitOfWork.Report.QcRecevingReport(DateFrom, DateTo);

            return Ok(orders);

        }

        [HttpGet]
        [Route("WarehouseReceivingReport")]
        public async Task<IActionResult> WarehouseReceivingReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var orders = await _unitOfWork.Report.WarehouseRecivingReport(DateFrom, DateTo);

            return Ok(orders);

        }

        [HttpGet]
        [Route("TransformationHistoryReport")]
        public async Task<IActionResult> TransformationHistoryReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var orders = await _unitOfWork.Report.TransformationReport(DateFrom, DateTo);

            return Ok(orders);

        }

        [HttpGet]
        [Route("MoveOrderHistory")]
        public async Task<IActionResult> MoveOrderHistory([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var orders = await _unitOfWork.Report.MoveOrderReport(DateFrom, DateTo);

            return Ok(orders);

        }

        [HttpGet]
        [Route("MiscellaneousReceiptReport")]
        public async Task<IActionResult> MiscellaneousReceiptReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var receipt = await _unitOfWork.Report.MReceiptReport(DateFrom, DateTo);

            return Ok(receipt);

        }

        [HttpGet]
        [Route("MiscellaneousIssueReport")]
        public async Task<IActionResult> MiscellaneousIssueReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var issue = await _unitOfWork.Report.MIssueReport(DateFrom, DateTo);

            return Ok(issue);

        }

        [HttpGet]
        [Route("NearlyExpireItemsReport")]
        public async Task<IActionResult> NearlyExpireItemsReport([FromQuery] int expirydays)
        {

            var expiry = await _unitOfWork.Report.NearlyExpireItemsReport(expirydays);

            return Ok(expiry);

        }

        [HttpGet]
        [Route("TransactedMoveOrderReport")]
        public async Task<IActionResult> TransactedMoveOrderReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var transact = await _unitOfWork.Report.TransactedMoveOrderReport(DateFrom, DateTo);

            return Ok(transact);

        }

        [HttpGet]
        [Route("CancelledOrderReport")]
        public async Task<IActionResult> CancelledOrderReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var cancel = await _unitOfWork.Report.CancelledOrderedReports(DateFrom, DateTo);

            return Ok(cancel);

        }

        [HttpGet]
        [Route("InventoryMovementReport")]
        public async Task<IActionResult> InventoryMovementReport([FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string PlusOne)
        {

            var cancel = await _unitOfWork.Report.InventoryMovementReport(DateFrom, DateTo, PlusOne);

            return Ok(cancel);

        }

    }
}
