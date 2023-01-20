using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{
    public class ReasonController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReasonController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpGet]
        [Route("GetAllReason")]
        public async Task<IActionResult> GetAllReason()
        {
            var reason = await _unitOfWork.Reasons.GetAll();

            return Ok(reason);
        }

        [HttpGet]
        [Route("GetAllReasonWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<ReasonDto>>> GetAllReasonWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var reason = await _unitOfWork.Reasons.GetAllReasonWithPagination(status, userParams);

            Response.AddPaginationHeader(reason.CurrentPage, reason.PageSize, reason.TotalCount, reason.TotalPages, reason.HasNextPage, reason.HasPreviousPage);

            var reasonResult = new
            {
                reason,
                reason.CurrentPage,
                reason.PageSize,
                reason.TotalCount,
                reason.TotalPages,
                reason.HasNextPage,
                reason.HasPreviousPage
            };

            return Ok(reasonResult);
        }

        [HttpGet]
        [Route("GetAllReasonWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<ReasonDto>>> GetAllReasonWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllReasonWithPagination(status, userParams);

            var reason = await _unitOfWork.Reasons.GetReasonByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(reason.CurrentPage, reason.PageSize, reason.TotalCount, reason.TotalPages, reason.HasNextPage, reason.HasPreviousPage);

            var reasonResult = new
            {
                reason,
                reason.CurrentPage,
                reason.PageSize,
                reason.TotalCount,
                reason.TotalPages,
                reason.HasNextPage,
                reason.HasPreviousPage
            };

            return Ok(reasonResult);
        }

        [HttpGet]
        [Route("GetAllReasonById/{id}")]
        public async Task<IActionResult> GetAllReasonById(int id)
        {
            var reason = await _unitOfWork.Reasons.GetById(id);

            if (reason == null)
                return NotFound();

            return Ok(reason);
        }

        [HttpGet]
        [Route("GetAllActiveReason")]
        public async Task<IActionResult> GetAllActiveReason()
        {
            var reason = await _unitOfWork.Reasons.GetAllActiveReason();

            return Ok(reason);
        }

        [HttpGet]
        [Route("GetAllInActiveReason")]
        public async Task<IActionResult> GetAllInActiveSupplier()
        {
            var reason = await _unitOfWork.Reasons.GetAllInActiveReason();

            return Ok(reason);
        }

        [HttpPost]
        [Route("AddNewReason")]
        public async Task<IActionResult> CreateNewReason(Reason reason)
        {
            if (ModelState.IsValid)
            {
                var moduleId = await _unitOfWork.Reasons.ValidateModuleId(reason.MenuId);

                var validate = await _unitOfWork.Reasons.ValidateReasonEntry(reason);

                if (validate == false)
                    return BadRequest("Menu and reason already exist!");

                if (moduleId == false)
                    return BadRequest("Module doesn't exist, Please add data first!");

                await _unitOfWork.Reasons.AddnewReason(reason);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllReason", new { reason.Id }, reason);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateReason/{id}")]
        public async Task<IActionResult> UpdateReason(int id, [FromBody] Reason reason)
        {
            if (id != reason.Id)
                return BadRequest();


            var moduleId = await _unitOfWork.Reasons.ValidateModuleId(reason.MenuId);

            if (moduleId == false)
                return BadRequest("Module doesn't exist, Please add data first!");

            await _unitOfWork.Reasons.UpdateReason(reason);
            await _unitOfWork.CompleteAsync();

            return Ok(reason);
        }

        [HttpPut]
        [Route("InActiveReason/{id}")]
        public async Task<IActionResult> InActiveReason(int id, [FromBody] Reason reason)
        {
            if (id != reason.Id)
                return BadRequest();

            await _unitOfWork.Reasons.InActiveReason(reason);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Reason!");
        }

        [HttpPut]
        [Route("ActivateReason/{id}")]
        public async Task<IActionResult> ActivateReason(int id, [FromBody] Reason reason)
        {
            if (id != reason.Id)
                return BadRequest();

            await _unitOfWork.Reasons.ActivateReason(reason);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Reason!");
        }


    }
}
