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
    public class RawMaterialController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public RawMaterialController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //----RAW MATERIAL-------

        [HttpGet]
        [Route("GetAllRawMaterials")]
        public async Task<IActionResult> GetAllRawMaterials()
        {
            var rawmaterial = await _unitOfWork.RawMaterials.GetAll();

            return Ok(rawmaterial);
        }

        [HttpGet]
        [Route("GetAllRawMaterialWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<RawMaterialDto>>> GetAllRawMaterialWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var rawmaterial = await _unitOfWork.RawMaterials.GetAllRawMaterialWithPagination(status, userParams);

            Response.AddPaginationHeader(rawmaterial.CurrentPage, rawmaterial.PageSize, rawmaterial.TotalCount, rawmaterial.TotalPages, rawmaterial.HasNextPage, rawmaterial.HasPreviousPage);

            var rawmaterialResult = new
            {
                rawmaterial,
                rawmaterial.CurrentPage,
                rawmaterial.PageSize,
                rawmaterial.TotalCount,
                rawmaterial.TotalPages,
                rawmaterial.HasNextPage,
                rawmaterial.HasPreviousPage
            };

            return Ok(rawmaterialResult);
        }

        [HttpGet]
        [Route("GetAllRawMaterialWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<RawMaterialDto>>> GetAllRawMaterialWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllRawMaterialWithPagination(status, userParams);

            var rawmaterial = await _unitOfWork.RawMaterials.GetRawMaterialByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(rawmaterial.CurrentPage, rawmaterial.PageSize, rawmaterial.TotalCount, rawmaterial.TotalPages, rawmaterial.HasNextPage, rawmaterial.HasPreviousPage);

            var rawmaterialResult = new
            {
                rawmaterial,
                rawmaterial.CurrentPage,
                rawmaterial.PageSize,
                rawmaterial.TotalCount,
                rawmaterial.TotalPages,
                rawmaterial.HasNextPage,
                rawmaterial.HasPreviousPage
            };

            return Ok(rawmaterialResult);
        }

        [HttpGet]
        [Route("GetRawMaterialsById/{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            var rawmaterial = await _unitOfWork.RawMaterials.GetById(id);

            if (rawmaterial == null)
                return NotFound();

            return Ok(rawmaterial);
        }

        [HttpGet]
        [Route("GetAllActiveRawMaterials")]
        public async Task<IActionResult> GetAllActiveRawMaterials()
        {
            var rawmaterial = await _unitOfWork.RawMaterials.GetAllActiveRawMaterial();

            return Ok(rawmaterial);
        }

        [HttpGet]
        [Route("GetAllInActiveRawMaterials")]
        public async Task<IActionResult> GetAllInActiveRawMaterials()
        {
            var rawmaterial = await _unitOfWork.RawMaterials.GetAllInActiveRawMaterial();

            return Ok(rawmaterial);
        }

        [HttpPost]
        [Route("CreateNewRawMaterials")]
        public async Task<IActionResult> CreateNewRawMaterials(RawMaterial rawmaterial)
        {
            if (ModelState.IsValid)
            {
                var itemCategoryId = await _unitOfWork.RawMaterials.ValidateItemCategoryId(rawmaterial.ItemCategoryId);
                var uomId = await _unitOfWork.RawMaterials.ValidateUOMId(rawmaterial.UomId);

                if (itemCategoryId == false)
                    return BadRequest("Item Category doesn't exist, Please add data first!");

                if (uomId == false)
                    return BadRequest("UOM doesn't exist, Please add data first!");

                if (await _unitOfWork.RawMaterials.ItemCodeExist(rawmaterial.ItemCode))
                    return BadRequest("Item Code already Exist!, Please try something else!");

                await _unitOfWork.RawMaterials.AddNewRawMaterial(rawmaterial);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllRawMaterials", new { rawmaterial.Id }, rawmaterial);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateRawMaterials/{id}")]
        public async Task<IActionResult> UpdateRawMaterials(int id, [FromBody] RawMaterial rawmaterial)
        {
            if (id != rawmaterial.Id)
                return BadRequest();

            var itemCategoryId = await _unitOfWork.RawMaterials.ValidateItemCategoryId(rawmaterial.ItemCategoryId);
            var uomId = await _unitOfWork.RawMaterials.ValidateUOMId(rawmaterial.UomId);

            if (itemCategoryId == false)
                return BadRequest("Item Category doesn't exist, Please add data first!");

            if (uomId == false)
                return BadRequest("UOM doesn't exist, Please add data first!");

            await _unitOfWork.RawMaterials.UpdateRawMaterialInfo(rawmaterial);
            await _unitOfWork.CompleteAsync();

            return Ok(rawmaterial);
        }

        [HttpPut]
        [Route("InActiveRawMaterial/{id}")]
        public async Task<IActionResult> InActiveSupplier(int id, [FromBody] RawMaterial rawmaterial)
        {
            if (id != rawmaterial.Id)
                return BadRequest();

            await _unitOfWork.RawMaterials.InActiveRawMaterial(rawmaterial);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Raw Material!");
        }

        [HttpPut]
        [Route("ActivateRawMaterial/{id}")]
        public async Task<IActionResult> ActivateModule(int id, [FromBody] RawMaterial rawMaterial)
        {
            if (id != rawMaterial.Id)
                return BadRequest();

            await _unitOfWork.RawMaterials.ActivateRawMaterial(rawMaterial);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Raw Material!");
        }

        //----ITEM CATEGORIES-------

        [HttpGet]
        [Route("GetAllItemCategory")]
        public async Task<IActionResult> GetAllItemCategory()
        {
            var category = await _unitOfWork.RawMaterials.GetAllItemCategory();

            return Ok(category);
        }

        [HttpGet]
        [Route("GetAllItemCategoryById/{id}")]
        public async Task<IActionResult> GetAllItemCategoryById(int id)
        {
            var category = await _unitOfWork.RawMaterials.GetCategoryById(id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpGet]
        [Route("GetAllActiveItemCategories")]
        public async Task<IActionResult> GetAllActiveItemCategories()
        {
            var category = await _unitOfWork.RawMaterials.GetAllActiveItemCategory();

            return Ok(category);
        }

        [HttpGet]
        [Route("GetAllInActiveItemCategories")]
        public async Task<IActionResult> GetAllInActiveItemCategories()
        {
            var category = await _unitOfWork.RawMaterials.GetAllInActiveItemCategory();

            return Ok(category);
        }

        [HttpPost]
        [Route("AddNewItemCategories")]
        public async Task<IActionResult> CreateNewIteCategories(ItemCategory category)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.RawMaterials.ItemCategoryExist(category.ItemCategoryName))
                    return BadRequest("Item Category already Exist!, Please try something else!");

                await _unitOfWork.RawMaterials.AddNewItemCategory(category);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllRawMaterials", new { category.Id }, category);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateItemCategories/{id}")]
        public async Task<IActionResult> UpdateItemCategories(int id, [FromBody] ItemCategory category)
        {
            if (id != category.Id)
                return BadRequest();

            if (await _unitOfWork.RawMaterials.ItemCategoryExist(category.ItemCategoryName))
                return BadRequest("Item Category already Exist!, Please try something else!");

            await _unitOfWork.RawMaterials.UpdateItemCategory(category);
            await _unitOfWork.CompleteAsync();

            return Ok(category);
        }

        [HttpPut]
        [Route("InActiveItemCategory/{id}")]
        public async Task<IActionResult> InActiveItemCategory(int id, [FromBody] ItemCategory category)
        {
            if (id != category.Id)
                return BadRequest();

            await _unitOfWork.RawMaterials.InActiveItemCategory(category);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Item Category!");
        }

        [HttpPut]
        [Route("ActivateItemCategory/{id}")]
        public async Task<IActionResult> ActivateItemCategory(int id, [FromBody] ItemCategory category)
        {
            if (id != category.Id)
                return BadRequest();

            await _unitOfWork.RawMaterials.ActivateItemCategory(category);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Item Category!");
        }


        [HttpGet]
        [Route("GetAllItemCategoryWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<RawMaterialDto>>> GetAllItemCategoryWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var category = await _unitOfWork.RawMaterials.GetAllItemCategoryWithPagination(status, userParams);

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
        [Route("GetAllItemCategoryWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<RawMaterialDto>>> GetAllItemCategoryWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllItemCategoryWithPagination(status, userParams);

            var category = await _unitOfWork.RawMaterials.GetAllItemCategoryWithPaginationOrig(userParams, status, search);


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
