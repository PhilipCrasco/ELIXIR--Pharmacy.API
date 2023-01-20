using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{
    public class UomController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public UomController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpGet]
        [Route("GetAllUOM")]
        public async Task<IActionResult> GetAll()
        {
            var uom = await _unitOfWork.Uoms.GetAll();

            return Ok(uom);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetUOMbyId(int id)
        {
            var uom = await _unitOfWork.Uoms.GetById(id);

            if (uom == null)
                return NotFound();

            return Ok(uom);
        }

        [HttpGet]
        [Route("GetAllUomWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllUomWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var uom = await _unitOfWork.Uoms.GetAllUomWithPagination(status, userParams);

            Response.AddPaginationHeader(uom.CurrentPage, uom.PageSize, uom.TotalCount, uom.TotalPages, uom.HasNextPage, uom.HasPreviousPage);

            var uomResult = new
            {
                uom,
                uom.CurrentPage,
                uom.PageSize,
                uom.TotalCount,
                uom.TotalPages,
                uom.HasNextPage,
                uom.HasPreviousPage
            };

            return Ok(uomResult);
        }

        [HttpGet]
        [Route("GetAllUomWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllUomWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllUomWithPagination(status, userParams);

            var uom = await _unitOfWork.Uoms.GetUomByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(uom.CurrentPage, uom.PageSize, uom.TotalCount, uom.TotalPages, uom.HasNextPage, uom.HasPreviousPage);

            var uomResult = new
            {
                uom,
                uom.CurrentPage,
                uom.PageSize,
                uom.TotalCount,
                uom.TotalPages,
                uom.HasNextPage,
                uom.HasPreviousPage
            };

            return Ok(uomResult);
        }

        [HttpPost]
        [Route("AddNewUOM")]
        public async Task<IActionResult> CreateNewUOM(UOM uom)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Uoms.UomCodeExist(uom.UOM_Code))
                    return BadRequest("Uom code already exist, please try something else!");

                if (await _unitOfWork.Uoms.UomDescription(uom.UOM_Description))
                    return BadRequest("Uom code description already exist, please try something else!");

                await _unitOfWork.Uoms.AddNewUom(uom);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAll", new { uom.Id }, uom);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateUom/{id}")]
        public async Task<IActionResult> UpdateModuleById(int id, [FromBody] UOM uom)
        {
            if (id != uom.Id)
                return BadRequest();

            await _unitOfWork.Uoms.UpdateUom(uom);
            await _unitOfWork.CompleteAsync();

            return Ok(uom);
        }

        [HttpPut]
        [Route("InActiveUom/{id}")]
        public async Task<IActionResult> InActiveModule(int id, [FromBody] UOM uom)
        {
            if (id != uom.Id)
                return BadRequest();

            await _unitOfWork.Uoms.InActiveUom(uom);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive UOM!");
        }

        [HttpPut]
        [Route("ActivateUom/{id}")]
        public async Task<IActionResult> ActivateUom(int id, [FromBody] UOM uom)
        {
            if (id != uom.Id)
                return BadRequest();

            await _unitOfWork.Uoms.ActivateUom(uom);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate UOM!");
        }

        [HttpGet]
        [Route("GetAllActiveUOM")]
        public async Task<IActionResult> GetAllActiveUom()
        {
            var uom = await _unitOfWork.Uoms.GetAllActiveUOM();

            return Ok(uom);
        }

        [HttpGet]
        [Route("GetAllInActiveUOM")]
        public async Task<IActionResult> GetAllInActiveUom()
        {
            var uom = await _unitOfWork.Uoms.GetAllInActiveUOM();

            return Ok(uom);
        }

    }
}
