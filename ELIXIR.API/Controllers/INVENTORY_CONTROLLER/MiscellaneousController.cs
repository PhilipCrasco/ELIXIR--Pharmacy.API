using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.MISCELLANEOUS_DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.INVENTORY_CONTROLLER
{


    [ApiController]
    public class MiscellaneousController : BaseApiController
    {

        private readonly IUnitOfWork _unitOfWork;

        public MiscellaneousController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }


        //--------------------------MISCELLANEOUS RECEIPT---------------------------------//


        [HttpPost]
        [Route("AddNewMiscellaneousReceipt")]
        public async Task<IActionResult> AddNewMiscellaneousReceipt([FromBody] MiscellaneousReceipt receipt)
        {

            receipt.IsActive = true;
            receipt.PreparedDate = DateTime.Now;

            await _unitOfWork.Miscellaneous.AddMiscellaneousReceipt(receipt);
            await _unitOfWork.CompleteAsync();

            return Ok(receipt);
        }

        [HttpPost]
        [Route("AddNewMiscellaneousReceiptInWarehouse")]
        public async Task<IActionResult> AddNewMiscellaneousReceiptInWarehouse([FromBody] WarehouseReceiving[] receive)
        {
            DateTime dateNow = DateTime.Now;

            foreach (WarehouseReceiving items in receive)
            {

                items.IsActive = true;
                items.ExpirationDays = items.Expiration.Subtract(dateNow).Days;
                items.ReceivingDate = DateTime.Now;
                items.IsWarehouseReceive = true;
                items.TransactionType = "MiscellaneousReceipt";
                items.ManufacturingDate = DateTime.Now;
                await _unitOfWork.Miscellaneous.AddMiscellaneousReceiptInWarehouse(items);
                await _unitOfWork.CompleteAsync();
            }

            return Ok("Successfully add new miscellaneous receipt in warehouse!");
        }

        [HttpPut]
        [Route("InActiveReceipt")]
        public async Task<IActionResult> InActiveReceipt([FromBody] MiscellaneousReceipt receipt)
        {

            var validate = await _unitOfWork.Miscellaneous.ValidateMiscellaneousReceiptInIssue(receipt);

            if (validate == false)
                return BadRequest("Inactive failed, you already use the receiving id");


            await _unitOfWork.Miscellaneous.InActivateMiscellaenousReceipt(receipt);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive receipt!");
        }

        [HttpPut]
        [Route("ActivateReceipt")]
        public async Task<IActionResult> ActivateReceipt([FromBody] MiscellaneousReceipt receipt)
        {

            await _unitOfWork.Miscellaneous.ActivateMiscellaenousReceipt(receipt);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully active receipt!");
        }

        [HttpGet]
        [Route("GetAllMiscellaneousReceiptPagination")]
        public async Task<ActionResult<IEnumerable<MReceiptDto>>> GetAllMiscellaneousReceiptPagination([FromQuery] UserParams userParams, [FromQuery] bool status)
        {
            var receipt = await _unitOfWork.Miscellaneous.GetAllMReceiptWithPagination(userParams, status);

            Response.AddPaginationHeader(receipt.CurrentPage, receipt.PageSize, receipt.TotalCount, receipt.TotalPages, receipt.HasNextPage, receipt.HasPreviousPage);

            var receiptResult = new
            {
                receipt,
                receipt.CurrentPage,
                receipt.PageSize,
                receipt.TotalCount,
                receipt.TotalPages,
                receipt.HasNextPage,
                receipt.HasPreviousPage
            };

            return Ok(receiptResult);
        }

        [HttpGet]
        [Route("GetAllMiscellaneousReceiptPaginationOrig")]
        public async Task<ActionResult<IEnumerable<MReceiptDto>>> GetAllMiscellaneousReceiptPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] bool status)
        {

            if (search == null)

                return await GetAllMiscellaneousReceiptPagination(userParams, status);

            var receipt = await _unitOfWork.Miscellaneous.GetAllMReceiptWithPaginationOrig(userParams, search, status);

            Response.AddPaginationHeader(receipt.CurrentPage, receipt.PageSize, receipt.TotalCount, receipt.TotalPages, receipt.HasNextPage, receipt.HasPreviousPage);

            var receiptResult = new
            {
                receipt,
                receipt.CurrentPage,
                receipt.PageSize,
                receipt.TotalCount,
                receipt.TotalPages,
                receipt.HasNextPage,
                receipt.HasPreviousPage
            };

            return Ok(receiptResult);
        }

        [HttpGet]
        [Route("GetAllDetailsFromWarehouseByMReceipt")]
        public async Task<IActionResult> GetAllListofOrders([FromQuery] int id)
        {

            var receipt = await _unitOfWork.Miscellaneous.GetWarehouseDetailsByMReceipt(id);

            return Ok(receipt);

        }



        //--------------------------MISCELLANEOUS ISSUE---------------------------------//

        [HttpPost]
        [Route("AddNewMiscellaneousIssueDetails")]
        public async Task<IActionResult> AddNewMiscellaneousIssueDetails([FromBody] MiscellaneousIssueDetails issue)
        {
            issue.IsActive = true;

            issue.PreparedDate = DateTime.Now;
            await _unitOfWork.Miscellaneous.AddMiscellaneousIssueDetails(issue);
            await _unitOfWork.CompleteAsync();


            return Ok("Successfully add new miscellaneous issue!");
        }

        [HttpPost]
        [Route("AddNewMiscellaneousIssue")]
        public async Task<IActionResult> AddNewMiscellaneousIssue([FromBody] MiscellaneousIssue issue)
        {

            issue.IsActive = true;
            issue.PreparedDate = DateTime.Now;
            issue.IsTransact = true;

            await _unitOfWork.Miscellaneous.AddMiscellaneousIssue(issue);
            await _unitOfWork.CompleteAsync();

            return Ok(issue);
        }

        [HttpPut]
        [Route("UpdateMiscellaneousIssuePKey")]
        public async Task<IActionResult> UpdateMiscellaneousIssuePKey([FromBody] MiscellaneousIssueDetails[] details)
        {


            foreach (MiscellaneousIssueDetails items in details)
            {
                items.IsActive = true;
                items.PreparedDate = DateTime.Now;

                await _unitOfWork.Miscellaneous.UpdateIssuePKey(items);
            }

            await _unitOfWork.CompleteAsync();

            return Ok(details);
        }

        [HttpGet]
        [Route("GetAllAvailableStocksForMIsssue")]
        public async Task<IActionResult> GetAllAvailableStocksForMIsssue([FromQuery] string itemcode)
        {

            var receipt = await _unitOfWork.Miscellaneous.GetAvailableStocksForIssue(itemcode);

            return Ok(receipt);

        }

        [HttpGet]
        [Route("GetAllMiscellaneousIssuePagination")]
        public async Task<ActionResult<IEnumerable<MReceiptDto>>> GetAllMiscellaneousIssuePagination([FromQuery] UserParams userParams, [FromQuery] bool status)
        {
            var issue = await _unitOfWork.Miscellaneous.GetAllMIssueWithPagination(userParams, status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);
        }

        [HttpGet]
        [Route("GetAllMiscellaneousIssuePaginationOrig")]
        public async Task<ActionResult<IEnumerable<MReceiptDto>>> GetAllMiscellaneousIssuePaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] bool status)
        {

            if (search == null)

                return await GetAllMiscellaneousIssuePagination(userParams, status);

            var issue = await _unitOfWork.Miscellaneous.GetAllMIssueWithPaginationOrig(userParams, search, status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);
        }

        [HttpPut]
        [Route("InActiveIssue")]
        public async Task<IActionResult> InActiveIssue([FromBody] MiscellaneousIssue issue)
        {



            await _unitOfWork.Miscellaneous.InActivateMiscellaenousIssue(issue);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive issue!");
        }

        [HttpPut]
        [Route("ActivateIssue")]
        public async Task<IActionResult> ActivateIssue([FromBody] MiscellaneousIssue issue)
        {

            await _unitOfWork.Miscellaneous.ActivateMiscellaenousIssue(issue);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully active issue!");
        }

        [HttpGet]
        [Route("GetAllDetailsInMiscellaneousIssue")]
        public async Task<IActionResult> GetAllDetailsInMiscellaneousIssue([FromQuery] int id)
        {

            var receipt = await _unitOfWork.Miscellaneous.GetAllDetailsInMiscellaneousIssue(id);

            return Ok(receipt);

        }

        [HttpGet]
        [Route("GetAllActiveMiscellaneousIssueTransaction")]
        public async Task<IActionResult> GetAllActiveMiscellaneousIssueTransaction([FromQuery] int empid)
        {

            var issue = await _unitOfWork.Miscellaneous.GetAllAvailableIssue(empid);

            return Ok(issue);

        }

        [HttpPut]
        [Route("CancelItemCodeInMiscellaneousIssue")]
        public async Task<IActionResult> CancelItemCodeInMiscellaneousIssue([FromBody] MiscellaneousIssueDetails[] issue)
        {

            foreach (MiscellaneousIssueDetails items in issue)
            {
                await _unitOfWork.Miscellaneous.CancelIssuePerItemCode(items);
                await _unitOfWork.CompleteAsync();
            }

            return new JsonResult("Successfully cancelled transaction!");
        }



    }
}
