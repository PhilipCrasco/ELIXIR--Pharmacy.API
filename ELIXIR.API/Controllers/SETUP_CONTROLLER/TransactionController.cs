using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{

    public class TransactionController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpGet]
        [Route("GetAllActiveTransactionName")]
        public async Task<IActionResult> GetAllActiveTransactionName()
        {
            var transact = await _unitOfWork.Transactions.GetAllTransactionName();

            return Ok(transact);
        }

        [HttpPost]
        [Route("AddNewTransaction")]
        public async Task<IActionResult> AddNewTransaction(Transaction transact)
        {
            if (await _unitOfWork.Transactions.TransactionNameExist(transact.TransactionName))
                return BadRequest("Transaction name already exist!, please try something else!");

                await _unitOfWork.Transactions.AddNewTransactionName(transact);
                await _unitOfWork.CompleteAsync();

            return Ok(transact);

        }

        [HttpPut]
        [Route("UpdateTransaction")]
        public async Task<IActionResult> UpdateSupplier([FromBody] Transaction transact)
        {

            if (await _unitOfWork.Transactions.TransactionNameExist(transact.TransactionName))
                return BadRequest("Transaction name already exist!, please try something else!");

            await _unitOfWork.Transactions.UpdateTransactionName(transact);
            await _unitOfWork.CompleteAsync();

            return Ok(transact);
        }


        [HttpPut]
        [Route("InActiveTransaction")]
        public async Task<IActionResult> InActiveTransaction([FromBody] Transaction transact)
        {
            if (await _unitOfWork.Transactions.TransactionNameExist(transact.TransactionName))
                return BadRequest("Transaction name already exist!, please try something else!");

            await _unitOfWork.Transactions.UpdateTransactionName(transact);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully updated transaction name!");
        }

        [HttpPut]
        [Route("InActiveTransaction/{id}")]
        public async Task<IActionResult> InActiveTransaction(int id, [FromBody] Transaction transact)
        {
            if (id != transact.Id)
                return BadRequest();

            await _unitOfWork.Transactions.InActiveTransactionName(transact);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive transaction name!");
        }

        [HttpPut]
        [Route("ActivateTransaction/{id}")]
        public async Task<IActionResult> ActivateTransaction(int id, [FromBody] Transaction transaction)
        {
            if (id != transaction.Id)
                return BadRequest();

            await _unitOfWork.Transactions.ActivateTransactionName(transaction);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully activate transaction name!");
        }


        [HttpGet]
        [Route("GetAllTransactionPagination/{status}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactionPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var transact = await _unitOfWork.Transactions.GetAllTransactionPagination(status, userParams);

            Response.AddPaginationHeader(transact.CurrentPage, transact.PageSize, transact.TotalCount, transact.TotalPages, transact.HasNextPage, transact.HasPreviousPage);

            var transactionResult = new
            {
                transact,
                transact.CurrentPage,
                transact.PageSize,
                transact.TotalCount,
                transact.TotalPages,
                transact.HasNextPage,
                transact.HasPreviousPage
            };

            return Ok(transactionResult);
        }

        [HttpGet]
        [Route("GetAllTransactionPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactionPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllTransactionPagination(status, userParams);

            var transact = await _unitOfWork.Suppliers.GetSupplierByStatusWithPaginationOrig(userParams, status, search);



            Response.AddPaginationHeader(transact.CurrentPage, transact.PageSize, transact.TotalCount, transact.TotalPages, transact.HasNextPage, transact.HasPreviousPage);

            var transactionResult = new
            {
                transact,
                transact.CurrentPage,
                transact.PageSize,
                transact.TotalCount,
                transact.TotalPages,
                transact.HasNextPage,
                transact.HasPreviousPage
            };

            return Ok(transactionResult);
        }




    }
}
