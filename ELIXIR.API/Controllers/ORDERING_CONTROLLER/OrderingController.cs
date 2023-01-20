using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.ORDERING_CONTROLLER
{
    [ApiController]
    public class OrderingController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork; 
        public OrderingController(IUnitOfWork unitofwork)
        {

            _unitOfWork = unitofwork;

        }

        [HttpGet]
        [Route("GetAllListofOrders")]
        public async Task<IActionResult> GetAllListofOrders([FromQuery] string farms)
        {

            var orders = await _unitOfWork.Order.GetAllListofOrders(farms);

            return Ok(orders);

        }

        [HttpPut]
        [Route("EditOrderQuantity")]
        public async Task<IActionResult> EditOrderQuantity([FromBody] Ordering order)
        {

            await _unitOfWork.Order.EditQuantityOrder(order);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully edit order quantity!");
        }

        [HttpPut]
        [Route("SchedulePreparedOrderedDate")]
        public async Task<IActionResult> SchedulePreparedOrderedDate([FromBody] Ordering[] order)
        {

            var generate = new GenerateOrderNo();

            generate.IsActive = true;

            await _unitOfWork.Order.GenerateNumber(generate);
            await _unitOfWork.CompleteAsync();

            foreach (Ordering items in order)
            {

                items.OrderNoPKey = generate.Id;

                await _unitOfWork.Order.SchedulePreparedDate(items);

            }

            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully scheduled orders");
        }

        [HttpPut]
        [Route("ApprovePreparedDate")]
        public async Task<IActionResult> ApprovePreparedDate([FromBody] Ordering order)
        {
            await _unitOfWork.Order.ApprovePreparedDate(order);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully approved prepared date!");
        }

        [HttpPut]
        [Route("RejectPreparedDate")]
        public async Task<IActionResult> RejectPreparedDate([FromBody] Ordering order)
        {

            await _unitOfWork.Order.RejectPreparedDate(order);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully reject prepared date!");
        }

        [HttpPut]
        [Route("GetAllListofPreparedDate")]
        public async Task<IActionResult> GetAllListofPreparedDate()
        {

            await _unitOfWork.Order.GetAllListOfPreparedDate();
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully schedules ordered");
        }

        [HttpGet]
        [Route("OrderSummary")]
        public async Task<IActionResult> OrderSummary([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var orders = await _unitOfWork.Order.OrderSummary(DateFrom, DateTo);

            return Ok(orders);

        }

        [HttpPost]
        [Route("AddNewOrders")]
        public async Task<IActionResult> AddNewOrders([FromBody] Ordering[] order)
        {

            List<Ordering> notExistFarmName = new List<Ordering>();
            List<Ordering> notExistFarmCode = new List<Ordering>();
            List<Ordering> notExistRawMats = new List<Ordering>();
            List<Ordering> notExistUom = new List<Ordering>();
            List<Ordering> duplicateList = new List<Ordering>();
            List<Ordering> previousdateNeeded = new List<Ordering>();


            List<Ordering> filteredOrders = new List<Ordering>();


            foreach (Ordering items in order)
            {

                var validateDuplicate = await _unitOfWork.Order.ValidateExistingOrders(items);
                var validateFarmName = await _unitOfWork.Order.ValidateCustomerName(items);
                var validateFarmCode = await _unitOfWork.Order.ValidateCustomerCode(items);
                var validateRawMaterial = await _unitOfWork.Order.ValidateRawMaterial(items);
                var validateUom = await _unitOfWork.Order.ValidateUom(items);
                var validateDateNeeded = await _unitOfWork.Order.ValidateOrderAndDateNeeded(items);

                if (validateDuplicate == false)
                {
                    duplicateList.Add(items);
                }

                else if (validateFarmName == false)
                {
                    notExistFarmName.Add(items);
                }

                else if (validateFarmCode == false)
                {
                    notExistFarmCode.Add(items);
                }

                else if (validateRawMaterial == false)
                {
                    notExistRawMats.Add(items);
                }

                else if (validateUom == false)
                {
                    notExistUom.Add(items);
                }
                else if (validateDateNeeded == false)
                {
                    previousdateNeeded.Add(items);
                }

                else
                    filteredOrders.Add(items);


                await _unitOfWork.Order.AddNewOrders(items);
            }


            var resultList = new
            {
                duplicateList,
                filteredOrders,
                notExistFarmName,
                notExistFarmCode,
                notExistRawMats,
                notExistUom,
                previousdateNeeded

            };

            if (notExistFarmName.Count == 0 && notExistFarmCode.Count == 0 && notExistRawMats.Count == 0
                                    && notExistUom.Count == 0 && duplicateList.Count == 0 && previousdateNeeded.Count == 0)
            {
                await _unitOfWork.CompleteAsync();

                return Ok("Successfully add new orders!");
            }

            else
            {
                return BadRequest(resultList);
            }

        }

        [HttpGet]
        [Route("GetAllListOfOrdersPagination")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllListOfOrdersPagination([FromQuery] UserParams userParams)
        {

            var orders = await _unitOfWork.Order.GetAllListofOrdersPagination(userParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);
        }


        [HttpPut]
        [Route("CancelOrders")]
        public async Task<IActionResult> CancelOrders([FromBody] Ordering orders)
        {

            var validate = await _unitOfWork.Order.CancelOrders(orders);

            if (validate == false)
                return BadRequest("Orders is not exist!");

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully cancel orders");

        }

        [HttpGet]
        [Route("GetAllListOfCancelledOrders")]
        public async Task<IActionResult> GetAllListOfCancelledOrders()
        {

            var orders = await _unitOfWork.Order.GetAllListOfCancelledOrders();

            return Ok(orders);

        }

        [HttpPut]
        [Route("ReturnCancelledOrders")]
        public async Task<IActionResult> ReturnCancelledOrders([FromBody] Ordering orders)
        {

            var validate = await _unitOfWork.Order.ReturnCancellOrdersInList(orders);

            if (validate == false)
                return BadRequest("Orders is not exist!");

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully return orders");

        }

        [HttpGet]
        [Route("GetAllListForMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllListForMoveOrderPagination([FromQuery] UserParams userParams)
        {

            var orders = await _unitOfWork.Order.GetAllListForMoveOrderPagination(userParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);
        }

        [HttpGet]
        [Route("GetAllListOfApprovedPreparedForMoveOrder")]
        public async Task<IActionResult> GetAllListOfApprovedPreparedForMoveOrder([FromQuery] string farm)
        {

            var orders = await _unitOfWork.Order.TotalListOfApprovedPreparedDate(farm);

            return Ok(orders);

        }

        [HttpGet]
        [Route("DetailedListOfOrders")]
        public async Task<IActionResult> DetailedListOfOrders([FromQuery] string farm)
        {

            var orders = await _unitOfWork.Order.DetailedListOfOrders(farm);

            return Ok(orders);

        }

        [HttpGet]
        [Route("GetAllListForScheduleApproval")]
        public async Task<IActionResult> GetAllListForScheduleApproval()
        {

            var orders = await _unitOfWork.Order.GetAllListForApprovalOfSchedule();

            return Ok(orders);

        }

        [HttpGet]
        [Route("GetAllOrdersForScheduleApproval")]
        public async Task<IActionResult> GetAllOrdersForScheduleApproval([FromQuery] int id)
        {

            var orders = await _unitOfWork.Order.GetAllOrdersForScheduleApproval(id);

            return Ok(orders);

        }


        [HttpGet]
        [Route("GetAllOutOfStockByItemCodeAndOrderDate")]
        public async Task<IActionResult> GetAllOutOfStockByItemCodeAndOrderDate([FromQuery] string itemcode, [FromQuery] string orderdate)
        {

            var orders = await _unitOfWork.Order.GetAllOutOfStockByItemCodeAndOrderDate(itemcode, orderdate);

            return Ok(orders);

        }


        [HttpPost]
        [Route("PrepareItemsForMoveOrder")]
        public async Task<IActionResult> PrepareItemsForMoveOrder([FromBody] MoveOrder order)
        {

            var details = await _unitOfWork.Order.GetMoveOrderDetailsForMoveOrder(order.OrderNoPKey);

            order.OrderNoPKey = details.Id;
            order.OrderDate = Convert.ToDateTime(details.OrderDate);
            order.DateNeeded = Convert.ToDateTime(details.DateNeeded);
            order.PreparedDate = Convert.ToDateTime(details.PreparedDate);
            order.FarmName = details.Farm;
            order.FarmCode = details.FarmCode;
            order.FarmType = details.FarmType;
            order.ItemCode = details.ItemCode;
            order.ItemDescription = details.ItemDescription;
            order.Uom = details.Uom;
            order.Category = details.Category;
            order.IsActive = true;
            order.IsPrepared = true;

            await _unitOfWork.Order.PrepareItemForMoveOrder(order);
            await _unitOfWork.CompleteAsync();

            return Ok(order);

        }


        [HttpGet]
        [Route("GetAllListOfOrdersForMoveOrder")]
        public async Task<IActionResult> GetAllListOfOrdersForMoveOrder([FromQuery] int id)
        {

            var orders = await _unitOfWork.Order.ListOfOrdersForMoveOrder(id);

            return Ok(orders);

        }

        [HttpGet]
        [Route("GetAvailableStockFromWarehouse")]
        public async Task<IActionResult> GetAvailableStockFromWarehouse([FromQuery] int id, [FromQuery] string itemcode)
        {
            var orders = await _unitOfWork.Order.GetActualItemQuantityInWarehouse(id, itemcode);

            var getFirstExpiry = await _unitOfWork.Order.GetFirstExpiry(itemcode);

            var resultList = new
            {
                orders,
                getFirstExpiry.WarehouseId
            };

            return Ok(resultList);
        }

        [HttpGet]
        [Route("ListOfPreparedItemsForMoveOrder")]
        public async Task<IActionResult> ListOfPreparedItemsForMoveOrder([FromQuery] int id)
        {

            var orders = await _unitOfWork.Order.ListOfPreparedItemsForMoveOrder(id);

            return Ok(orders);

        }

        [HttpPut]
        [Route("CancelPreparedItems")]
        public async Task<IActionResult> CancelPreparedItems([FromBody] MoveOrder moveorder)
        {

            await _unitOfWork.Order.CancelMoveOrder(moveorder);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully cancel prepared items");

        }


        [HttpPut]
        [Route("AddPlateNumberInMoveOrder")]
        public async Task<IActionResult> AddPlateNumberInMoveOrder([FromBody] Ordering[] order)
        {

            foreach (Ordering items in order)
            {

                await _unitOfWork.Order.AddPlateNumberInMoveOrder(items);

            }

            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully added plate number!");
        }

        [HttpPut]
        [Route("AddDeliveryStatus")]
        public async Task<IActionResult> AddDeliveryStatus([FromBody] Ordering[] order)
        {

            foreach (Ordering items in order)
            {

                await _unitOfWork.Order.AddDeliveryStatus(items);

            }

            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully added delivery status!");
        }

        [HttpPut]
        [Route("ApproveListOfMoveOrder")]
        public async Task<IActionResult> ApproveListOfMoveOrder([FromBody] MoveOrder moveorder)
        {

            await _unitOfWork.Order.ApprovalForMoveOrder(moveorder);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully approved list for move order!");
        }

        [HttpPut]
        [Route("RejectListOfMoveOrder")]
        public async Task<IActionResult> RejectListOfMoveOrder([FromBody] MoveOrder moveorder)
        {

            await _unitOfWork.Order.RejectForMoveOrder(moveorder);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully reject list for move order!");
        }

        [HttpPut]
        [Route("RejectApproveListOfMoveOrder")]
        public async Task<IActionResult> RejectApproveListOfMoveOrder([FromBody] MoveOrder moveorder)
        {

            await _unitOfWork.Order.RejectApproveMoveOrder(moveorder);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully reject approved list for move order!");
        }

        [HttpPut]
        [Route("ReturnMoveOrderForApproval")]
        public async Task<IActionResult> ReturnMoveOrderForApproval([FromBody] MoveOrder moveorder)
        {

            await _unitOfWork.Order.ReturnMoveOrderForApproval(moveorder);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully return list for move order!");
        }


        [HttpPut]
        [Route("UpdatePrintStatus")]
        public async Task<IActionResult> UpdatePrintStatus([FromBody] MoveOrder moveorder)
        {

            await _unitOfWork.Order.UpdatePrintStatus(moveorder);
            await _unitOfWork.CompleteAsync();

            return Ok(moveorder);
        }


        [HttpGet]
        [Route("GetAllForApprovalMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> GetAllForApprovalMoveOrderPagination([FromQuery] UserParams userParams)
        {
            var moveorder = await _unitOfWork.Order.ForApprovalMoveOrderPagination(userParams);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("GetAllForApprovalMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> GetAllForApprovalMoveOrderPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllForApprovalMoveOrderPagination(userParams);

            var moveorder = await _unitOfWork.Order.ForApprovalMoveOrderPaginationOrig(userParams, search);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }


        [HttpGet]
        [Route("ViewMoveOrderForApproval")]
        public async Task<IActionResult> ViewMoveOrderForApproval([FromQuery] int id)
        {

            var orders = await _unitOfWork.Order.ViewMoveOrderForApproval(id);

            return Ok(orders);

        }

        [HttpGet]
        [Route("ApprovedMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> ApprovedMoveOrderPagination([FromQuery] UserParams userParams)
        {
            var moveorder = await _unitOfWork.Order.ApprovedMoveOrderPagination(userParams);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("ApprovedMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> ApprovedMoveOrderPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await ApprovedMoveOrderPagination(userParams);

            var moveorder = await _unitOfWork.Order.ApprovedMoveOrderPaginationOrig(userParams, search);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }


        [HttpGet]
        [Route("RejectedMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> RejectedMoveOrderPagination([FromQuery] UserParams userParams)
        {
            var moveorder = await _unitOfWork.Order.RejectedMoveOrderPagination(userParams);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("RejectedMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> RejectedMoveOrderPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await RejectedMoveOrderPagination(userParams);

            var moveorder = await _unitOfWork.Order.RejectedMoveOrderPaginationOrig(userParams, search);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("GetAllApprovedMoveOrder")]
        public async Task<IActionResult> GetAllApprovedMoveOrder([FromQuery] int id)
        {

            var orders = await _unitOfWork.Order.GetAllApprovedMoveOrder(id);

            return Ok(orders);

        }

        [HttpPut]
        [Route("CancelOrdersInMoveOrder")]
        public async Task<IActionResult> CancelOrdersInMoveOrder([FromBody] Ordering order)
        {

            await _unitOfWork.Order.CancelControlInMoveOrder(order);

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully cancel orders");

        }

        [HttpGet]
        [Route("GetAllApprovedOrderCalendar")]
        public async Task<IActionResult> GetAllApprovedOrderCalendar()
        {

            var orders = await _unitOfWork.Order.GetAllApprovedOrdersForCalendar();

            return Ok(orders);

        }

        //------------------TRANSACT MOVE ORDER-------------------------

        [HttpGet]
        [Route("GetTotalListForMoveOrder")]
        public async Task<IActionResult> GetTotalListForMoveOrder([FromQuery] bool status)
        {

            var orders = await _unitOfWork.Order.TotalListForTransactMoveOrder(status);

            return Ok(orders);

        }

        [HttpGet]
        [Route("ListOfMoveOrdersForTransact")]
        public async Task<IActionResult> ListOfMoveOrdersForTransact([FromQuery] int orderid)
        {

            var orders = await _unitOfWork.Order.ListOfMoveOrdersForTransact(orderid);

            return Ok(orders);

        }

        [HttpPost]
        [Route("TransactListOfMoveOrders")]
        public async Task<IActionResult> TransactListOfMoveOrders([FromBody] TransactMoveOrder[] transact)
        {

            foreach (TransactMoveOrder items in transact)
            {

                items.IsActive = true;
                items.IsTransact = true;
                items.PreparedDate = DateTime.Now;

                await _unitOfWork.Order.TransanctListOfMoveOrders(items);
            }

            await _unitOfWork.CompleteAsync();

            return Ok(transact);

        }
    }

}
