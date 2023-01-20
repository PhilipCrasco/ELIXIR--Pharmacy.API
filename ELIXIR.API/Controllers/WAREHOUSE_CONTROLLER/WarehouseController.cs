
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.WAREHOUSE_CONTROLLER
{

    public class WarehouseController : BaseApiController
    {

        private readonly IUnitOfWork _unitOfWork;
        public WarehouseController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }


        [HttpGet]
        [Route("GetAllAvailableForWarehouseReceiving")]
        public async Task<IActionResult> GetAllAvailablePo()
        {
            var posummary = await _unitOfWork.Receives.GetAllRawMaterialsForWarehouseReceiving();

            return Ok(posummary);
        }


        [HttpPut]
        [Route("CancelPartialReceivingInQc/{id}")]
        public async Task<IActionResult> CancelPartialReceivingInQc(int id, [FromBody] PO_Receiving receiving)
        {
            if (id != receiving.Id)
                return BadRequest();

            await _unitOfWork.Receives.CancelPartialRecevingInQC(receiving);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully cancelled partial receiving for warehouse!");
        }


        [HttpGet]
        [Route("ScanBarcode/{itemcode}")]
        public async Task<IActionResult> GetUser(string itemcode)
        {
            var scanbarcode = await _unitOfWork.Warehouse.GetScanBarcodeByReceivedId(itemcode);

            if (scanbarcode == null)
                return BadRequest("Already scanned all available for warehouse receiving!");

            return Ok(scanbarcode);

        }

        [HttpPut]
        [Route("ReceiveRawMaterialsInWarehouse/{itemcode}")]
        public async Task<IActionResult> ReceiveRawMaterialsInWarehouse(string itemcode, [FromBody] WarehouseReceiving warehouse)
        {
            if (itemcode != warehouse.ItemCode)
                return BadRequest();

            var validateScanbarcode = await _unitOfWork.Warehouse.ScanBarcode(warehouse);

            if (validateScanbarcode == false)
                return BadRequest("Already scan all available items in list!");

            var actualgood = await _unitOfWork.Warehouse.ReceiveMaterialsFromWarehouse(warehouse);

            if (actualgood == false)
                return BadRequest("Receive failed! please check your input on actual good.");

            var validatetotal = await _unitOfWork.Warehouse.ValidateActualAndRejectInput(warehouse);

            if (validatetotal == false)
                return BadRequest("Received failed! actual good and reject are not equal to total goods.");

            await _unitOfWork.CompleteAsync();
            //return new JsonResult("Successfully Received Raw Materials!");

            return Ok(warehouse);
        }

        [HttpGet]
        [Route("GetRejectMaterialsForWarehouse")]
        public async Task<IActionResult> GetRejectMaterialsForWarehouse()
        {
            var warehouse = await _unitOfWork.Warehouse.GetAllRejectRawmaterialsInWarehouse();

            return Ok(warehouse);
        }

        [HttpPut]
        [Route("RejectMaterialFromWarehouse")]
        public async Task<IActionResult> RejectMaterialFromWarehouse([FromBody] Warehouse_Reject[] reject)
        {

            foreach (Warehouse_Reject items in reject)
            {
                var validate = await _unitOfWork.Warehouse.RejectRawMaterialsByWarehouse(items);

                if (validate == false)
                    return BadRequest("Reject failed!, you're trying to reject greater amount in total reject");


                var validareject = await _unitOfWork.Warehouse.ValidateTotalReject(items);

                if (validareject == false)
                    return BadRequest("Reject failed, there is no materials for reject!");
            }

            await _unitOfWork.CompleteAsync();

            return Ok(reject);

        }

        [HttpGet]
        [Route("ListForWarehouseReceiving")]
        public async Task<IActionResult> ListForWarehouseReceiving()
        {
            var warehouse = await _unitOfWork.Warehouse.ListForWarehouseReceiving();

            return Ok(warehouse);
        }

        [HttpPut]
        [Route("ReturnRawmaterialByWarehouse/{id}")]
        public async Task<IActionResult> ReturnRawmaterialByWarehouse(int id, [FromBody] PO_Receiving receiving)
        {
            if (id != receiving.Id)
                return BadRequest();

            var validate = await _unitOfWork.Warehouse.ReturnRawmaterialsByWarehouse(receiving);

            if (validate == false)
                return BadRequest("Rawmaterial does not exist!");

            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully return raw materials!");
        }

        [HttpGet]
        [Route("GetRejectMaterialsInWarehouse/{warehouseid}")]
        public async Task<IActionResult> GetRejectMaterialsInWarehouse(int warehouseid)
        {
            var reject = await _unitOfWork.Warehouse.GetAllRejectedMaterialsByWarehouseId(warehouseid);

            return Ok(reject);
        }

        [HttpGet]
        [Route("GetAllWarehouseReceived")]
        public async Task<IActionResult> GetAllWarehouseReceived([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {
            var warehouse = await _unitOfWork.Warehouse.GetAllWarehouseReceived(DateFrom, DateTo);

            return Ok(warehouse);
        }

        [HttpPut]
        [Route("CancelAndReturnForWarehouseReceive/{id}")]
        public async Task<IActionResult> CancelAndReturnForWarehouseReceive(int id, [FromBody] WarehouseReceiving receiving)
        {
            if (id != receiving.Id)
                return BadRequest();

            await _unitOfWork.Warehouse.CancelAndReturnMaterialsForWarehouseReceive(receiving);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully cancelled materials received in warehouse!");
        }

        [HttpPut]
        [Route("CancelAndReturnForPoSummary/{id}")]
        public async Task<IActionResult> CancelAndReturnForPoSummary(int id, [FromBody] WarehouseReceiving receiving)
        {
            if (id != receiving.Id)
                return BadRequest();

            await _unitOfWork.Warehouse.CancelAndReturnMaterialsInPoSummary(receiving);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully cancelled materials received in warehouse!");
        }

        [HttpGet]
        [Route("GetAllListForWarehouseWithPagination")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllListForWarehousePagination([FromQuery] UserParams userParams)
        {
            var warehouse = await _unitOfWork.Warehouse.ListForWarehouseReceivingWithPagination(userParams);

            Response.AddPaginationHeader(warehouse.CurrentPage, warehouse.PageSize, warehouse.TotalCount, warehouse.TotalPages, warehouse.HasNextPage, warehouse.HasPreviousPage);

            var warehouseResult = new
            {
                warehouse,
                warehouse.CurrentPage,
                warehouse.PageSize,
                warehouse.TotalCount,
                warehouse.TotalPages,
                warehouse.HasNextPage,
                warehouse.HasPreviousPage
            };

            return Ok(warehouseResult);
        }

        [HttpGet]
        [Route("GetAllListForWarehouseWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllListForWarehousePaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllListForWarehousePagination(userParams);

            var warehouse = await _unitOfWork.Warehouse.ListForWarehouseReceivingWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(warehouse.CurrentPage, warehouse.PageSize, warehouse.TotalCount, warehouse.TotalPages, warehouse.HasNextPage, warehouse.HasPreviousPage);

            var warehouseResult = new
            {
                warehouse,
                warehouse.CurrentPage,
                warehouse.PageSize,
                warehouse.TotalCount,
                warehouse.TotalPages,
                warehouse.HasNextPage,
                warehouse.HasPreviousPage
            };

            return Ok(warehouseResult);
        }

        [HttpGet]
        [Route("GetAllRejectMaterialsFromWarehouse")]
        public async Task<ActionResult<IEnumerable<NearlyExpireDto>>> GetAllRejectMaterialsFromWarehouse([FromQuery] UserParams userParams)
        {
            var reject = await _unitOfWork.Warehouse.RejectRawMaterialsByWarehousePagination(userParams);

            Response.AddPaginationHeader(reject.CurrentPage, reject.PageSize, reject.TotalCount, reject.TotalPages, reject.HasNextPage, reject.HasPreviousPage);

            var rejectResult = new
            {
                reject,
                reject.CurrentPage,
                reject.PageSize,
                reject.TotalCount,
                reject.TotalPages,
                reject.HasNextPage,
                reject.HasPreviousPage
            };

            return Ok(rejectResult);
        }

        [HttpGet]
        [Route("GetAllRejectMaterialsFromWarehouseOrig")]
        public async Task<ActionResult<IEnumerable<NearlyExpireDto>>> GetAllNearlyExpireWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllRejectMaterialsFromWarehouse(userParams);

            var reject = await _unitOfWork.Warehouse.RejectRawMaterialsByWarehousePaginationOrig(userParams, search);

            Response.AddPaginationHeader(reject.CurrentPage, reject.PageSize, reject.TotalCount, reject.TotalPages, reject.HasNextPage, reject.HasPreviousPage);

            var rejectResult = new
            {
                reject,
                reject.CurrentPage,
                reject.PageSize,
                reject.TotalCount,
                reject.TotalPages,
                reject.HasNextPage,
                reject.HasPreviousPage
            };

            return Ok(rejectResult);
        }

        [HttpGet]
        [Route("GetAllListOfWarehouseReceivingIdNull")]
        public async Task<IActionResult> GetAllListOfWarehouseReceivingIdNull()
        {
            var warehouse = await _unitOfWork.Warehouse.ListOfWarehouseReceivingId();

            return Ok(warehouse);
        }


        [HttpGet]
        [Route("GetAllListOfWarehouseReceivingId")]
        public async Task<IActionResult> GetAllListOfWarehouseReceivingId([FromQuery] string search)
        {

            if (search == null)

                return await GetAllListOfWarehouseReceivingIdNull();


            var warehouse = await _unitOfWork.Warehouse.ListOfWarehouseReceivingId(search);

            return Ok(warehouse);
        }

        [HttpGet]
        [Route("GetAllListOfWarehouseIdPagination")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllListOfWarehouseIdPagination([FromQuery] UserParams userParams)
        {
            var warehouse = await _unitOfWork.Warehouse.GetAllWarehouseIdWithPagination(userParams);

            Response.AddPaginationHeader(warehouse.CurrentPage, warehouse.PageSize, warehouse.TotalCount, warehouse.TotalPages, warehouse.HasNextPage, warehouse.HasPreviousPage);

            var warehouseResult = new
            {
                warehouse,
                warehouse.CurrentPage,
                warehouse.PageSize,
                warehouse.TotalCount,
                warehouse.TotalPages,
                warehouse.HasNextPage,
                warehouse.HasPreviousPage
            };

            return Ok(warehouseResult);
        }

        [HttpGet]
        [Route("GetAllListOfWarehouseIdPaginationOrig")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllListOfWarehouseIdPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllListOfWarehouseIdPagination(userParams);

            var warehouse = await _unitOfWork.Warehouse.GetAllWarehouseIdWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(warehouse.CurrentPage, warehouse.PageSize, warehouse.TotalCount, warehouse.TotalPages, warehouse.HasNextPage, warehouse.HasPreviousPage);

            var warehouseResult = new
            {
                warehouse,
                warehouse.CurrentPage,
                warehouse.PageSize,
                warehouse.TotalCount,
                warehouse.TotalPages,
                warehouse.HasNextPage,
                warehouse.HasPreviousPage
            };

            return Ok(warehouseResult);
        }





    }
}
