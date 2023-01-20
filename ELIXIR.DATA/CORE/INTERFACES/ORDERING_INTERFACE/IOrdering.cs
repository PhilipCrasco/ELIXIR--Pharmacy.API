using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.ORDERING_INTERFACE
{
    public interface IOrdering
    {

        Task<IReadOnlyList<OrderDto>> GetAllListofOrders(string farms);
        Task<bool> EditQuantityOrder(Ordering orders);
        Task<bool> SchedulePreparedDate(Ordering orders);
        Task<IReadOnlyList<OrderDto>> GetAllListOfPreparedDate();
        Task<bool> ApprovePreparedDate(Ordering orders);
        Task<bool> RejectPreparedDate(Ordering orders);
        Task<bool> AddNewOrders(Ordering orders);
        Task<IReadOnlyList<OrderDto>> OrderSummary(string DateFrom, string DateTo);
        Task<bool> ValidateCustomerName(Ordering orders);
        Task<bool> ValidateCustomerCode(Ordering orders);
        Task<bool> ValidateRawMaterial(Ordering orders);
        Task<bool> ValidateUom(Ordering orders);
        Task<bool> ValidateExistingOrders(Ordering orders);
        Task<PagedList<OrderDto>> GetAllListofOrdersPagination(UserParams userParams);
      
        Task<bool> ValidateOrderAndDateNeeded(Ordering orders);
        Task<bool> CancelOrders(Ordering orders);
        Task<IReadOnlyList<OrderDto>> GetAllListOfCancelledOrders();
        Task<bool> ReturnCancellOrdersInList(Ordering orders);
        Task<PagedList<OrderDto>> GetAllListForMoveOrderPagination(UserParams userParams);
        Task<IReadOnlyList<OrderDto>> TotalListOfApprovedPreparedDate(string farm);
        Task<IReadOnlyList<OrderDto>> DetailedListOfOrders(string farm);
        Task<IReadOnlyList<OrderDto>> GetAllListForApprovalOfSchedule();
        Task<IReadOnlyList<OrderDto>> GetAllOrdersForScheduleApproval(int id);
        Task<int> CountOrderNoKey();
        Task<ItemStocks> GetActualItemQuantityInWarehouse(int id, string itemcode);
        Task<bool> GenerateNumber(GenerateOrderNo generate);
        Task<bool> PrepareItemForMoveOrder(MoveOrder orders);
        Task<IReadOnlyList<OrderDto>> GetAllOutOfStockByItemCodeAndOrderDate(string itemcode, string orderdate);
        Task<IReadOnlyList<OrderDto>> ListOfOrdersForMoveOrder(int id);
        Task<OrderDto> GetMoveOrderDetailsForMoveOrder(int orderid);
        Task<IReadOnlyList<MoveOrderDto>> ListOfPreparedItemsForMoveOrder(int id);
        Task<bool> CancelMoveOrder(MoveOrder moveorder);
        Task<bool> AddPlateNumberInMoveOrder(Ordering order);
        Task<bool> AddDeliveryStatus(Ordering order);
        Task<bool> ApprovalForMoveOrder(MoveOrder moveorder);
        Task<bool> RejectForMoveOrder(MoveOrder moveorder);
        Task<bool> RejectApproveMoveOrder(MoveOrder moveorder);
        Task<PagedList<MoveOrderDto>> ForApprovalMoveOrderPagination(UserParams userParams);

        Task<PagedList<MoveOrderDto>> ForApprovalMoveOrderPaginationOrig(UserParams userParams, string search);

        Task<IReadOnlyList<MoveOrderDto>>ViewMoveOrderForApproval (int orderid);

        Task<PagedList<MoveOrderDto>> ApprovedMoveOrderPagination(UserParams userParams);

        Task<PagedList<MoveOrderDto>> ApprovedMoveOrderPaginationOrig(UserParams userParams, string search);

        Task<PagedList<MoveOrderDto>> RejectedMoveOrderPagination(UserParams userParams);

        Task<PagedList<MoveOrderDto>> RejectedMoveOrderPaginationOrig(UserParams userParams, string search);

        Task<bool> ReturnMoveOrderForApproval(MoveOrder moveorder);

        Task<bool> UpdatePrintStatus(MoveOrder moveorder);

        Task<MoveOrderDto> GetAllApprovedMoveOrder(int id);

        Task<ItemStocks> GetFirstExpiry(string itemcode);

        Task<bool> CancelControlInMoveOrder(Ordering order);

        Task<IReadOnlyList<OrderDto>> GetAllApprovedOrdersForCalendar();


        //--------------------Transact Move Order--------------------
        Task<IReadOnlyList<OrderDto>> TotalListForTransactMoveOrder(bool status);
        Task<IReadOnlyList<MoveOrderDto>> ListOfMoveOrdersForTransact(int orderid);
        Task<bool> TransanctListOfMoveOrders(TransactMoveOrder transact);





        //-----------------Notifaction---------------------
        Task<IReadOnlyList<OrderDto>> GetOrdersForNotification();
        Task<IReadOnlyList<OrderDto>> GetMoveOrdersForNotification();
        Task<IReadOnlyList<OrderDto>> GetAllForTransactMoveOrderNotification();
        Task<IReadOnlyList<MoveOrderDto>> GetForApprovalMoveOrderNotification();
        Task<IReadOnlyList<MoveOrderDto>> GetRejectMoveOrderNotification();






    }
}
