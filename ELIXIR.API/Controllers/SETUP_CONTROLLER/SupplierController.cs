using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{
    public class SupplierController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public SupplierController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpGet]
        [Route("GetAllSupplier")]
        public async Task<IActionResult> GetAllSupplier()
        {
            var supplier = await _unitOfWork.Suppliers.GetAll();

            return Ok(supplier);
        }

        [HttpGet]
        [Route("GetSupplierById/{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetById(id);

            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }

        [HttpGet]
        [Route("GetAllSupplierWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAllSupplierWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var supplier = await _unitOfWork.Suppliers.GetAllSupplierWithPagination(status, userParams);

            Response.AddPaginationHeader(supplier.CurrentPage, supplier.PageSize, supplier.TotalCount, supplier.TotalPages, supplier.HasNextPage, supplier.HasPreviousPage);

            var supplierResult = new
            {
                supplier,
                supplier.CurrentPage,
                supplier.PageSize,
                supplier.TotalCount,
                supplier.TotalPages,
                supplier.HasNextPage,
                supplier.HasPreviousPage
            };

            return Ok(supplierResult);
        }

        [HttpGet]
        [Route("GetAllSupplierWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAllSupplierWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllSupplierWithPagination(status, userParams);

            var supplier = await _unitOfWork.Suppliers.GetSupplierByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(supplier.CurrentPage, supplier.PageSize, supplier.TotalCount, supplier.TotalPages, supplier.HasNextPage, supplier.HasPreviousPage);

            var supplierResult = new
            {
                supplier,
                supplier.CurrentPage,
                supplier.PageSize,
                supplier.TotalCount,
                supplier.TotalPages,
                supplier.HasNextPage,
                supplier.HasPreviousPage
            };

            return Ok(supplierResult);
        }

        [HttpGet]
        [Route("GetAllActiveSupplier")]
        public async Task<IActionResult> GetAllActiveSupplier()
        {
            var supplier = await _unitOfWork.Suppliers.GetAllActiveSupplier();

            return Ok(supplier);
        }

        [HttpGet]
        [Route("GetAllInActiveSupplier")]
        public async Task<IActionResult> GetAllInActiveSupplier()
        {
            var supplier = await _unitOfWork.Suppliers.GetAllInActiveSupplier();

            return Ok(supplier);
        }

        [HttpPost]
        [Route("AddNewSupplier")]
        public async Task<IActionResult> CreateNewSupplier(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Suppliers.SupplierCodeExist(supplier.SupplierCode))
                    return BadRequest("Supplier code already exist!, please try something else!");

                await _unitOfWork.Suppliers.AddnewSupplier(supplier);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllSupplier", new { supplier.Id }, supplier);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateSupplier/{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] Supplier supplier)
        {
            if (id != supplier.Id)
                return BadRequest();

            await _unitOfWork.Suppliers.UpdateSupplierInfo(supplier);
            await _unitOfWork.CompleteAsync();

            return Ok(supplier);
        }

        [HttpPut]
        [Route("InActiveSupplier/{id}")]
        public async Task<IActionResult> InActiveSupplier(int id, [FromBody] Supplier supplier)
        {
            if (id != supplier.Id)
                return BadRequest();

            await _unitOfWork.Suppliers.InActiveSupplier(supplier);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Supplier!");
        }

        [HttpPut]
        [Route("ActivateSupplier/{id}")]
        public async Task<IActionResult> ActivateModule(int id, [FromBody] Supplier supplier)
        {
            if (id != supplier.Id)
                return BadRequest();

            await _unitOfWork.Suppliers.ActivateSupplier(supplier);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Supplier!");
        }


    }
}
