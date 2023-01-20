using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;
using ELIXIR.DATA.DTOs.WAREHOUSE_DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.QC_CONTROLLER
{
    [ApiController]
    public class ReceivingController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReceivingController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpPost]
        [Route("AddNewReceivingInformationInPO")]
        public async Task<IActionResult> CreateNewCustomer(PO_Receiving receive)
        {
            if (ModelState.IsValid)
            {

                await _unitOfWork.Receives.AddNewReceivingInformation(receive);
                await _unitOfWork.CompleteAsync();

                return Ok("Successfully Add!");
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPost]
        [Route("AddNewRejectInPo")]
        public async Task<IActionResult> AddNewRejectInPo([FromBody] PO_Reject[] reject)
        {
            if (ModelState.IsValid)
            {

                foreach (PO_Reject items in reject)
                {

                    await _unitOfWork.Receives.AddNewRejectInfo(items);

                }

                await _unitOfWork.CompleteAsync();

                return Ok("Successfully add reject materials!");
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("ReceiveRawMaterialsById/{id}")]
        public async Task<IActionResult> UpdateReceiveInfo(int id, [FromBody] PO_Receiving receiving)
        {

            if (id != receiving.PO_Summary_Id)
                return BadRequest();

            if (receiving.Actual_Delivered <= 0)
                return BadRequest("Received failed, please check your input in actual delivered!");

            if (receiving.TotalReject != 0)
                receiving.Actual_Delivered = receiving.Actual_Delivered - receiving.TotalReject;

            var validatePoId = await _unitOfWork.Receives.ValidatePoId(receiving.PO_Summary_Id);

            if (validatePoId == false)
                return BadRequest("Update failed, PO does not exist!");

            var validateActualgood = await _unitOfWork.Receives.ValidateActualRemaining(receiving);

            if (validateActualgood == false)
                return BadRequest("Receive failed, You're trying to input greater than the total received!");


            await _unitOfWork.Receives.UpdateReceivingInfo(receiving);
            await _unitOfWork.CompleteAsync();

            return Ok(receiving);
        }

        [HttpPut]
        [Route("RejectRawMaterialsByReceivingId")]
        public async Task<IActionResult> UpdateRejectInfo([FromBody] PO_Reject[] reject)
        {

            foreach (PO_Reject items in reject)
            {
                var validate = await _unitOfWork.Receives.UpdateRejectInfo(items);

                if (validate == false)
                    return BadRequest("Reject failed, Receiving Id does'nt exist!");

            }
            await _unitOfWork.CompleteAsync();

            return Ok(reject);
        }

        [HttpGet]
        [Route("GetAllAvailablePo")]
        public async Task<IActionResult> GetAllAvailablePo()
        {
            var posummary = await _unitOfWork.Receives.GetAllAvailablePo();

            return Ok(posummary);
        }

        [HttpGet]
        [Route("GetAllCancelledPo")]
        public async Task<IActionResult> GetAllCancelledPo()
        {
            var posummary = await _unitOfWork.Receives.GetAllCancelledPo();

            return Ok(posummary);
        }

        [HttpGet]
        [Route("GetAllNearlyExpireRawMaterials")]
        public async Task<IActionResult> GetAllNearlyExpireRawMaterials()
        {
            var posummary = await _unitOfWork.Receives.GetAllNearlyExpireRawMaterial();

            return Ok(posummary);
        }

        [HttpPut]
        [Route("CancelPO/{id}")]
        public async Task<IActionResult> CancelPO(int id, [FromBody] ImportPOSummary summary)
        {
            if (id != summary.Id)
                return BadRequest();


            var validate = await _unitOfWork.Receives.ValidatePOForCancellation(summary.Id);

            if (validate == false)
                return BadRequest("Cancel failed, you have materials for receiving in warehouse!");

            await _unitOfWork.Receives.CancelPo(summary);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Cancelled PO!");
        }

        [HttpPut]
        [Route("ReturnPoInAvailableList/{id}")]
        public async Task<IActionResult> ReturnPoInAvailableList(int id, [FromBody] ImportPOSummary summary)
        {
            if (id != summary.Id)
                return BadRequest();

            await _unitOfWork.Receives.ReturnPoInAvailableList(summary);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Returned PO!");
        }

        [HttpPut]
        [Route("ApproveNearlyExpire/{id}")]
        public async Task<IActionResult> ApproveNearlyExpire(int id, [FromBody] PO_Receiving receiving)
        {
            if (id != receiving.Id)
                return BadRequest();

            await _unitOfWork.Receives.ApproveNearlyExpireRawMaterials(receiving);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully approved nearly expire materials!");
        }

        [HttpPut]
        [Route("RejectNearlyExpire/{id}")]
        public async Task<IActionResult> RejectNearlyExpire(int id, [FromBody] PO_Receiving receiving)
        {
            if (id != receiving.Id)
                return BadRequest();

            await _unitOfWork.Receives.RejectRawMaterialsNearlyExpire(receiving);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully reject nearly expire materials!");
        }

        [HttpPut]
        [Route("WarehouseConfirmRejectbyQc/{id}")]
        public async Task<IActionResult> WarehouseConfirmRejectbyQc(int id, [FromBody] WarehouseReceiving warehouse)
        {
            if (id != warehouse.QcReceivingId)
                return BadRequest();

            await _unitOfWork.Receives.WarehouseConfirmRejectByQc(warehouse);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully confirm reject materials!");
        }

        [HttpPut]
        [Route("WarehouseReturnRejectbyQc/{id}")]
        public async Task<IActionResult> WarehouseReturnRejectbyQc(int id, [FromBody] PO_Receiving receiving)
        {
            if (id != receiving.Id)
                return BadRequest();

            await _unitOfWork.Receives.WarehouseReturnRejectByQc(receiving);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully return reject materials!");
        }

        [HttpGet]
        [Route("GetAllWarehouseReceivingConfirmReject")]
        public async Task<IActionResult> GetAllWarehouseReceivingConfirmReject()
        {
            var reject = await _unitOfWork.Receives.GetAllWarehouseConfirmReject();

            return Ok(reject);
        }

        [HttpGet]
        [Route("GetAllAvailablePoWithPagination")]
        public async Task<ActionResult<IEnumerable<PoSummaryChecklistDto>>> GetAllPoWithPagination([FromQuery] UserParams userParams)
        {
            var posummary = await _unitOfWork.Receives.GetAllPoSummaryWithPagination(userParams);

            Response.AddPaginationHeader(posummary.CurrentPage, posummary.PageSize, posummary.TotalCount, posummary.TotalPages, posummary.HasNextPage, posummary.HasPreviousPage);

            var posummaryResult = new
            {
                posummary,
                posummary.CurrentPage,
                posummary.PageSize,
                posummary.TotalCount,
                posummary.TotalPages,
                posummary.HasNextPage,
                posummary.HasPreviousPage
            };

            return Ok(posummaryResult);
        }

        [HttpGet]
        [Route("GetAllAvailablePoWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<PoSummaryChecklistDto>>> GetAllAvailablePoWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllPoWithPagination(userParams);

            var posummary = await _unitOfWork.Receives.GetPoSummaryByStatusWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(posummary.CurrentPage, posummary.PageSize, posummary.TotalCount, posummary.TotalPages, posummary.HasNextPage, posummary.HasPreviousPage);

            var posummaryResult = new
            {
                posummary,
                posummary.CurrentPage,
                posummary.PageSize,
                posummary.TotalCount,
                posummary.TotalPages,
                posummary.HasNextPage,
                posummary.HasPreviousPage
            };

            return Ok(posummaryResult);
        }

        [HttpGet]
        [Route("GetAllAvailableForWarehouseReceivingWithPagination")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllAvailableForWarehouseReceivingWithPagination([FromQuery] UserParams userParams)
        {
            var warehouse = await _unitOfWork.Receives.GetAllAvailableForWarehouseWithPagination(userParams);

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
        [Route("GetAllAvailableForWarehouseReceivingWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllAvailableForWarehouseReceivingWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllAvailableForWarehouseReceivingWithPagination(userParams);

            var warehouse = await _unitOfWork.Receives.GetAllAvailableForWarehouseWithPaginationOrig(userParams, search);

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
        [Route("GetAllCancelledPoWithPagination")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllCancelledPoWithPagination([FromQuery] UserParams userParams)
        {
            var cancel = await _unitOfWork.Receives.GetAllCancelledPOWithPagination(userParams);

            Response.AddPaginationHeader(cancel.CurrentPage, cancel.PageSize, cancel.TotalCount, cancel.TotalPages, cancel.HasNextPage, cancel.HasPreviousPage);

            var warehouseResult = new
            {
                cancel,
                cancel.CurrentPage,
                cancel.PageSize,
                cancel.TotalCount,
                cancel.TotalPages,
                cancel.HasNextPage,
                cancel.HasPreviousPage
            };

            return Ok(warehouseResult);
        }

        [HttpGet]
        [Route("GetAllCancelledPoWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllCancelledPoWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllCancelledPoWithPagination(userParams);

            var cancel = await _unitOfWork.Receives.GetAllCancelledPOWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(cancel.CurrentPage, cancel.PageSize, cancel.TotalCount, cancel.TotalPages, cancel.HasNextPage, cancel.HasPreviousPage);

            var warehouseResult = new
            {
                cancel,
                cancel.CurrentPage,
                cancel.PageSize,
                cancel.TotalCount,
                cancel.TotalPages,
                cancel.HasNextPage,
                cancel.HasPreviousPage
            };

            return Ok(warehouseResult);
        }

        [HttpGet]
        [Route("GetAllNearlyExpireWithPagination")]
        public async Task<ActionResult<IEnumerable<NearlyExpireDto>>> GetAllNearlyExpireWithPagination([FromQuery] UserParams userParams)
        {
            var expiry = await _unitOfWork.Receives.GetAllNearlyExpireWithPagination(userParams);

            Response.AddPaginationHeader(expiry.CurrentPage, expiry.PageSize, expiry.TotalCount, expiry.TotalPages, expiry.HasNextPage, expiry.HasPreviousPage);

            var nearlyResult = new
            {
                expiry,
                expiry.CurrentPage,
                expiry.PageSize,
                expiry.TotalCount,
                expiry.TotalPages,
                expiry.HasNextPage,
                expiry.HasPreviousPage
            };

            return Ok(nearlyResult);
        }

        [HttpGet]
        [Route("GetAllNearlyExpireWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<NearlyExpireDto>>> GetAllNearlyExpireWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllNearlyExpireWithPagination(userParams);

            var expiry = await _unitOfWork.Receives.GetAllNearlyExpireWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(expiry.CurrentPage, expiry.PageSize, expiry.TotalCount, expiry.TotalPages, expiry.HasNextPage, expiry.HasPreviousPage);

            var nearlyResult = new
            {
                expiry,
                expiry.CurrentPage,
                expiry.PageSize,
                expiry.TotalCount,
                expiry.TotalPages,
                expiry.HasNextPage,
                expiry.HasPreviousPage
            };

            return Ok(nearlyResult);
        }

        [HttpGet]
        [Route("GetNotification")]
        public async Task<IActionResult> GetNotification()
        {

            //QC Receiving
            var posummary = await _unitOfWork.Receives.GetAllAvailablePo();
            var warehouse = await _unitOfWork.Receives.GetAllRawMaterialsForWarehouseReceiving();
            var nearlyexpire = await _unitOfWork.Receives.GetAllNearlyExpireRawMaterial();
            var cancelledpo = await _unitOfWork.Receives.GetAllCancelledPo();
            var approvalreject = await _unitOfWork.Warehouse.GetAllRejectRawmaterialsInWarehouse();
            var confirmreject = await _unitOfWork.Receives.GetAllWarehouseConfirmReject();


            //Transformation
            var listOfRequest = await _unitOfWork.Planning.GetAllPendingRequestNotif();
            var listofReject = await _unitOfWork.Planning.GetAllRejectRequestNotif();
            var listofpreparation = await _unitOfWork.Preparation.GetAllTransformationFormulaInformation();
            var listofmixing = await _unitOfWork.Preparation.GetAllTransformationForMixing();

            //Ordering
            var orderingfarm = await _unitOfWork.Order.GetOrdersForNotification();
            var orderingapproval = await _unitOfWork.Order.GetAllListForApprovalOfSchedule();
            var moveorderlist = await _unitOfWork.Order.GetMoveOrdersForNotification();
            var transactmoveorderlist = await _unitOfWork.Order.GetAllForTransactMoveOrderNotification();
            var forapprovallist = await _unitOfWork.Order.GetForApprovalMoveOrderNotification();
            var rejectlist = await _unitOfWork.Order.GetRejectMoveOrderNotification();

            //QC ReceivingCount
            var posummarycount = posummary.Count();
            var warehousecount = warehouse.Count();
            var nearexpirecount = nearlyexpire.Count();
            var cancelledpocount = cancelledpo.Count();
            var approvalrejectcount = approvalreject.Count();
            var confirmrejectcount = confirmreject.Count();

            //TransformationCount
            var approvalrequestcount = listOfRequest.Count();
            var requestrejectcount = listofReject.Count();
            var preparationcount = listofpreparation.Count();
            var mixingcount = listofmixing.Count();

            //OrderingCount
            var orderingfarmcount = orderingfarm.Count();
            var orderingapprovalcount = orderingapproval.Count();
            var moveordercount = moveorderlist.Count();
            var transactmoveordercount = transactmoveorderlist.Count();
            var forapprovallistcount = forapprovallist.Count();
            var rejectlistcount = rejectlist.Count();


            var countList = new
            {
                QcReceiving = new
                {
                    posummarycount
                },
                WarehouseReceiving = new
                {
                    warehousecount
                },
                NearlyExpire = new
                {
                    nearexpirecount
                },
                CancelledPO = new
                {
                    cancelledpocount
                },
                ApprovalRejectWarehouse = new
                {
                    approvalrejectcount
                },
                ConfirmReject = new
                {
                    confirmrejectcount
                },
                ApprovalRequest = new
                {
                    approvalrequestcount
                },
                RejectRequest = new
                {
                    requestrejectcount
                },
                Preparation = new
                {
                    preparationcount
                },
                Mixing = new
                {
                    mixingcount
                },
                OrderingFarm = new 
                {
                    orderingfarmcount
                },
                OrderingApproval = new
                {
                    orderingapprovalcount
                },
                MoveOrderList = new
                {
                    moveordercount
                },
                TransactMoveOrderList = new
                {
                    transactmoveordercount
                },
                ForApprovalMoveOrder = new
                {
                    forapprovallistcount
                },
                RejectMoveOrder = new
                {
                    rejectlistcount
                }
                
            };

            return Ok(countList);
        }

        [HttpGet]
        [Route("GetAllWarehouseConfirmRejectWithPagination")]
        public async Task<ActionResult<IEnumerable<RejectWarehouseReceivingDto>>> GetAllWarehouseConfirmRejectWithPagination([FromQuery] UserParams userParams)
        {
            var reject = await _unitOfWork.Receives.GetAllConfirmRejectWithPagination(userParams);

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
        [Route("GetAllWarehouseConfirmRejectWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<RejectWarehouseReceivingDto>>> GetAllWarehouseConfirmRejectWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllWarehouseConfirmRejectWithPagination(userParams);

            var reject = await _unitOfWork.Receives.GetAllConfirmRejectWithPaginationOrig(userParams, search);


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
        [Route("GetDetailsForNearlyExpire")]
        public async Task<IActionResult> GetDetailsForNearlyExpire([FromQuery] int id)
        {
            var posummary = await _unitOfWork.Receives.GetItemDetailsForNearlyExpire(id);

            return Ok(posummary);
        }


    }
}
