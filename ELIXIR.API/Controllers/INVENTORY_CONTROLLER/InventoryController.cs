using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.INVENTORY_CONTROLLER
{

    public class InventoryController : BaseApiController
    {
        private IUnitOfWork _unitOfWork;

        public InventoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("RawmaterialInventory")]
        public async Task<IActionResult> GetAllAvailableRawmaterial()
        {
            var rawmaterial = await _unitOfWork.Inventory.GetAllAvailbleInRawmaterialInventory();

            return Ok(rawmaterial);
        }

        [HttpGet]
        [Route("MRPInventory")]
        public async Task<IActionResult> MRPInventory()
        {
            var inventory = await _unitOfWork.Inventory.GetAllItemForInventory();

            return Ok(inventory);
        }


        [HttpGet]
        [Route("GetAllItemForInventoryPagination")]
        public async Task<ActionResult<IEnumerable<MRPDto>>> GetAllItemForInventoryPagination([FromQuery] UserParams userParams)
        {
            var inventory = await _unitOfWork.Inventory.GetAllItemForInventoryPagination(userParams);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);
        }

        [HttpGet]
        [Route("GetAllItemForInventoryPaginationOrig")]
        public async Task<ActionResult<IEnumerable<MRPDto>>> GetAllMiscellaneousIssuePaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllItemForInventoryPagination(userParams);

            var inventory = await _unitOfWork.Inventory.GetAllItemForInventoryPaginationOrig(userParams, search);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);
        }

    }
}
