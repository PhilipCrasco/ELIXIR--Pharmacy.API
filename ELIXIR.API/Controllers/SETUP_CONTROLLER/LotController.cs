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
    public class LotController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LotController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        //--------LOT NAME----------

        [HttpGet]
        [Route("GetAllLotNames")]
        public async Task<IActionResult> GetAllLots()
        {
            var lotname = await _unitOfWork.Lots.GetAll();

            return Ok(lotname);
        }

        [HttpGet]
        [Route("GetAllLotNameWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<LotNameDto>>> GetAllLotNameWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var lotname = await _unitOfWork.Lots.GetAllLotNameWithPagination(status, userParams);

            Response.AddPaginationHeader(lotname.CurrentPage, lotname.PageSize, lotname.TotalCount, lotname.TotalPages, lotname.HasNextPage, lotname.HasPreviousPage);

            var lotnameResult = new
            {
                lotname,
                lotname.CurrentPage,
                lotname.PageSize,
                lotname.TotalCount,
                lotname.TotalPages,
                lotname.HasNextPage,
                lotname.HasPreviousPage
            };

            return Ok(lotnameResult);
        }

        [HttpGet]
        [Route("GetAllLotNameWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<LotNameDto>>> GetAllLotNameWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllLotNameWithPagination(status, userParams);

            var lotname = await _unitOfWork.Lots.GetLotNameByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(lotname.CurrentPage, lotname.PageSize, lotname.TotalCount, lotname.TotalPages, lotname.HasNextPage, lotname.HasPreviousPage);

            var lotnameResult = new
            {
                lotname,
                lotname.CurrentPage,
                lotname.PageSize,
                lotname.TotalCount,
                lotname.TotalPages,
                lotname.HasNextPage,
                lotname.HasPreviousPage
            };

            return Ok(lotnameResult);
        }

        [HttpGet]
        [Route("GetLotNameById/{id}")]
        public async Task<IActionResult> GetLotNameById(int id)
        {
            var lotname = await _unitOfWork.Lots.GetById(id);

            return Ok(lotname);
        }

        [HttpGet]
        [Route("GetAllActiveLotName")]
        public async Task<IActionResult> GetAllActiveLotName()
        {
            var lotname = await _unitOfWork.Lots.GetAllActiveLotName();

            return Ok(lotname);
        }

        [HttpGet]
        [Route("GetAllInActiveLotName")]
        public async Task<IActionResult> GetAllInActiveSupplier()
        {
            var lotname = await _unitOfWork.Lots.GetAllInActiveLotName();
            return Ok(lotname);
        }

        [HttpPost]
        [Route("AddNewLotName")]
        public async Task<IActionResult> CreateNewLotName(LotName lotname)
        {
            if (ModelState.IsValid)
            {

                var categoryId = await _unitOfWork.Lots.ValidateLotCategoryId(lotname.LotCategoryId);

                if (categoryId == false)
                    return BadRequest("Lot Category doesn't exist, Please add data first!");

                //if (await _unitOfWork.Lots.SectionNameExist(lotname.SectionName))
                //    return BadRequest("Section Name already Exist!, Please try something else!");
                var validate = await _unitOfWork.Lots.ValidateLotNameAndSection(lotname);

                if (validate == true)
                    return BadRequest("Lotname and section already exist!");

                await _unitOfWork.Lots.AddNewLot(lotname);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllLots", new { lotname.Id }, lotname);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateLotName/{id}")]
        public async Task<IActionResult> UpdateLotName(int id, [FromBody] LotName lotname)
        {
            if (id != lotname.Id)
                return BadRequest();

            var categoryId = await _unitOfWork.Lots.ValidateLotCategoryId(lotname.LotCategoryId);

            if (categoryId == false)
                return BadRequest("Lot category doesn't exist, please add data first!");

            await _unitOfWork.Lots.UpdateLotName(lotname);
            await _unitOfWork.CompleteAsync();

            return Ok(lotname);
        }

        [HttpPut]
        [Route("InActiveLotName/{id}")]
        public async Task<IActionResult> InActiveLotName(int id, [FromBody] LotName lotname)
        {
            if (id != lotname.Id)
                return BadRequest();

            await _unitOfWork.Lots.InActiveLotName(lotname);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Lot Name!");
        }

        [HttpPut]
        [Route("ActivateLotName/{id}")]
        public async Task<IActionResult> ActivateLotName(int id, [FromBody] LotName lotname)
        {
            if (id != lotname.Id)
                return BadRequest();

            await _unitOfWork.Lots.ActivateLotName(lotname);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Lot Name!");
        }

        //--------LOT CATEGORY----------

        [HttpGet]
        [Route("GetAllLotCategories")]
        public async Task<IActionResult> GetAllLotCategories()
        {
            var category = await _unitOfWork.Lots.GetAllLotCategory();

            return Ok(category);
        }

        [HttpGet]
        [Route("GetLotCategoryById/{id}")]
        public async Task<IActionResult> GetLotCategoryById(int id)
        {
            var category = await _unitOfWork.Lots.GetCategoryById(id);

            return Ok(category);
        }

        [HttpGet]
        [Route("GetAllActiveLotCategories")]
        public async Task<IActionResult> GetAllActiveLotCategories()
        {
            var category = await _unitOfWork.Lots.GetAllActiveLotCategory();

            return Ok(category);
        }

        [HttpGet]
        [Route("GetAllInActiveLotCategory")]
        public async Task<IActionResult> GetAllInActiveLotCategory()
        {
            var category = await _unitOfWork.Lots.GetAllInActiveLotCategory();

            return Ok(category);
        }

        [HttpPost]
        [Route("AddNewLotCategory")]
        public async Task<IActionResult> CreateNewLotCategory(LotCategory category)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Lots.LotCategoryNameExist(category.LotCategoryName))
                    return BadRequest("Category Name already Exist!, Please try something else!");

                await _unitOfWork.Lots.AddNewLotCategory(category);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllLotCategories", new { category.Id }, category);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateLotCategory/{id}")]
        public async Task<IActionResult> UpdateLotCategory(int id, [FromBody] LotCategory category)
        {
            if (id != category.Id)
                return BadRequest();

            if (await _unitOfWork.Lots.LotCategoryNameExist(category.LotCategoryName))
                return BadRequest("Category Name already Exist!, Please try something else!");

            await _unitOfWork.Lots.UpdateLotCategory(category);
            await _unitOfWork.CompleteAsync();

            return Ok(category);
        }

        [HttpPut]
        [Route("InActiveLotCategory/{id}")]
        public async Task<IActionResult> InActiveLotCategory(int id, [FromBody] LotCategory category)
        {
            if (id != category.Id)
                return BadRequest();

            await _unitOfWork.Lots.InActiveLotCategory(category);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Lot Category!");
        }

        [HttpPut]
        [Route("ActivateLotCategory/{id}")]
        public async Task<IActionResult> ActivateLotCategory(int id, [FromBody] LotCategory category)
        {
            if (id != category.Id)
                return BadRequest();

            await _unitOfWork.Lots.ActivateLotCategory(category);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Lot Category!");
        }

        [HttpGet]
        [Route("GetAllLotCategoryWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<LotNameDto>>> GetAllLotCategoryWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var category = await _unitOfWork.Lots.GetAllLotCategoryWithPagination(status, userParams);

            Response.AddPaginationHeader(category.CurrentPage, category.PageSize, category.TotalCount, category.TotalPages, category.HasNextPage, category.HasPreviousPage);

            var categoryResult = new
            {
                category,
                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasNextPage,
                category.HasPreviousPage
            };

            return Ok(categoryResult);
        }

        [HttpGet]
        [Route("GetAllLotCategoryWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<LotNameDto>>> GetAllLotCategoryWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllLotCategoryWithPagination(status, userParams);

            var category = await _unitOfWork.Lots.GetAllLotCategoryWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(category.CurrentPage, category.PageSize, category.TotalCount, category.TotalPages, category.HasNextPage, category.HasPreviousPage);


            var categoryResult = new
            {
                category,
                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasNextPage,
                category.HasPreviousPage
            };

            return Ok(categoryResult);
        }

    }
}
