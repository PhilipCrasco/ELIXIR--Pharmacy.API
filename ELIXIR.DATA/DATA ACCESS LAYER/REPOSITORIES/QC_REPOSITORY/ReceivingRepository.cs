using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.WAREHOUSE_DTOs;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DTOs.REPORT_DTOs;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY
{
    public class ReceivingRepository : IReceivingRepository
    {
        private readonly StoreContext _context;
        public ReceivingRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewReceivingInformation(PO_Receiving receive)
        {
            await _context.QC_Receiving.AddAsync(receive);

            return true;
        }
        public async Task<bool> UpdateReceivingInfo(PO_Receiving receiving)
        {
            DateTime dateAdd = DateTime.Now.AddDays(30);

            var existingInfo = await _context.QC_Receiving.Where(x => x.PO_Summary_Id == receiving.PO_Summary_Id)
                                                          .FirstOrDefaultAsync();

            receiving.IsActive = true;

            if (receiving.Expiry_Date < dateAdd)
                receiving.IsNearlyExpire = true;

            if (receiving.Expiry_Date > dateAdd)
                receiving.ExpiryIsApprove = true;
 
            receiving.QC_ReceiveDate = DateTime.Now;
          
            return await AddNewReceivingInformation(receiving);

             var totalreject = await _context.QC_Reject.Where(x => x.PO_ReceivingId == existingInfo.Id)
                                                       .Select(x => x.Quantity).SumAsync();

            existingInfo.Manufacturing_Date = receiving.Manufacturing_Date;
            existingInfo.Expected_Delivery = receiving.Expected_Delivery;
            existingInfo.Expiry_Date = receiving.Expiry_Date;
            existingInfo.Actual_Delivered = receiving.Actual_Delivered;
            existingInfo.Batch_No = receiving.Batch_No;

            existingInfo.Truck_Approval1 = receiving.Truck_Approval1;
            existingInfo.Truck_Approval2 = receiving.Truck_Approval2;
            existingInfo.Truck_Approval3 = receiving.Truck_Approval3;
            existingInfo.Truck_Approval4 = receiving.Truck_Approval4;

            existingInfo.Truck_Approval1_Remarks = receiving.Truck_Approval1_Remarks;
            existingInfo.Truck_Approval2_Remarks = receiving.Truck_Approval2_Remarks;
            existingInfo.Truck_Approval3_Remarks = receiving.Truck_Approval3_Remarks;
            existingInfo.Truck_Approval4_Remarks = receiving.Truck_Approval4_Remarks;

            existingInfo.Unloading_Approval1 = receiving.Unloading_Approval1;
            existingInfo.Unloading_Approval2 = receiving.Unloading_Approval2;
            existingInfo.Unloading_Approval3 = receiving.Unloading_Approval3;
            existingInfo.Unloading_Approval4 = receiving.Unloading_Approval4;

            existingInfo.Unloading_Approval1_Remarks = receiving.Unloading_Approval1_Remarks;
            existingInfo.Unloading_Approval2_Remarks = receiving.Unloading_Approval2_Remarks;
            existingInfo.Unloading_Approval3_Remarks = receiving.Unloading_Approval3_Remarks;
            existingInfo.Unloading_Approval4_Remarks = receiving.Unloading_Approval4_Remarks;

            existingInfo.Checking_Approval1 = receiving.Checking_Approval1;
            existingInfo.Checking_Approval2 = receiving.Checking_Approval2;

            existingInfo.Checking_Approval1_Remarks = receiving.Checking_Approval1_Remarks;
            existingInfo.Checking_Approval2_Remarks = receiving.Checking_Approval2_Remarks;

            existingInfo.QA_Approval = receiving.QA_Approval;
            existingInfo.QA_Approval_Remarks = receiving.QA_Approval_Remarks;

            existingInfo.TotalReject = totalreject;
            existingInfo.IsWareHouseReceive = false;

            return true;
        }
        public async Task<bool> AddNewRejectInfo(PO_Reject reject)
        {
            await _context.QC_Reject.AddAsync(reject);

            return true;
        }
        public async Task<bool> UpdateRejectInfo(PO_Reject reject)
        {
            var existingInfo = await _context.QC_Reject.Where(x => x.PO_ReceivingId == reject.PO_ReceivingId)
                                                       .FirstOrDefaultAsync();

            var validateActualRemaining = await _context.QC_Receiving.Where(x => x.Id == reject.PO_ReceivingId)
                                                                     .Where(x => x.IsActive == true)
                                                                     .FirstOrDefaultAsync();
            if (validateActualRemaining == null)
                return false;

            if (existingInfo == null)
                return await AddNewRejectInfo(reject);

            existingInfo.Quantity = reject.Quantity;
            existingInfo.Remarks = reject.Remarks;

            return true;
        }
        public async Task<bool> ValidatePoId(int id)
        {
            var validateExisting = await _context.POSummary.Where(x => x.Id == id)
                                                           .Where(x => x.IsActive == true)
                                                           .FirstOrDefaultAsync();
            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<IReadOnlyList<PoSummaryChecklistDto>> GetAllAvailablePo()
        {
            var summaryx = (from posummary in _context.POSummary
                            where posummary.IsActive == true
                            join receive in _context.QC_Receiving
                            on posummary.Id equals receive.PO_Summary_Id into leftJ
                            from receive in leftJ.DefaultIfEmpty()

                            select new PoSummaryChecklistDto
                            {
                                Id = posummary.Id,
                                PO_Number = posummary.PO_Number,
                                ItemCode = posummary.ItemCode,
                                ItemDescription = posummary.ItemDescription,
                                Supplier = posummary.VendorName,
                                UOM = posummary.UOM,
                                QuantityOrdered = posummary.Ordered,
                                ActualGood = receive != null && receive.IsActive != false && receive.IsWareHouseReceive != false ? receive.Actual_Delivered : 0,
                                IsActive = posummary.IsActive,
                                IsQcReceiveIsActive = receive != null && receive.IsActive != false ? receive.IsActive : true,
                                ActualRemaining = 0,

                            });

            return await summaryx
                                    .GroupBy(x => new
                                    {
                                        x.Id,
                                        x.PO_Number,
                                        x.ItemCode,
                                        x.ItemDescription,
                                        x.UOM,
                                        x.Supplier,
                                        x.QuantityOrdered,
                                        x.IsActive,
                                        x.IsQcReceiveIsActive,
                                    })
                                                    .Select(receive => new PoSummaryChecklistDto
                                                    {
                                                        Id = receive.Key.Id,
                                                        PO_Number = receive.Key.PO_Number,
                                                        ItemCode = receive.Key.ItemCode,
                                                        ItemDescription = receive.Key.ItemDescription,
                                                        UOM = receive.Key.UOM,
                                                        Supplier = receive.Key.Supplier,
                                                        QuantityOrdered = receive.Key.QuantityOrdered,
                                                        ActualGood = receive.Sum(x => x.ActualGood),
                                                        ActualRemaining = receive.Key.QuantityOrdered - (receive.Sum(x => x.ActualGood)),
                                                        IsActive = receive.Key.IsActive,
                                                        IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive
                                                    })
                                                    .OrderBy(x => x.PO_Number)
                                                    .Where(x => x.ActualRemaining != 0)
                                                    .Where(x => x.IsActive == true)
                                                    .ToListAsync(); 
        }

        public async Task<bool> CancelPo(ImportPOSummary summary)
        {
            var existingPo = await _context.POSummary.Where(x => x.Id == summary.Id)
                                                     .FirstOrDefaultAsync();

            existingPo.IsActive = false;
            existingPo.Date_Cancellation = DateTime.Now;
            existingPo.Reason = summary.Reason;

            if (summary.Reason == null)
                existingPo.Reason = "Wrong Input";

            return true;
        }

        public async Task<IReadOnlyList<CancelledPoDto>> GetAllCancelledPo()
        {
            var cancelpo = (from posummary in _context.POSummary
                            join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                            from receive in leftJ.DefaultIfEmpty()
                            select new CancelledPoDto
                            {
                                Id = posummary.Id,
                                PO_Number = posummary.PO_Number,
                                ItemCode = posummary.ItemCode,
                                ItemDescription = posummary.ItemDescription,
                                Supplier = posummary.VendorName,
                                QuantityOrdered = posummary.Ordered,
                                QuantityCancel = receive != null ? receive.Actual_Delivered : 0,
                                QuantityGood = receive != null ? receive.Actual_Delivered : 0,
                                DateCancelled = posummary.Date_Cancellation.ToString(),
                                Remarks = posummary.Reason,
                                IsActive = posummary.IsActive
                            }).Where(x => x.IsActive == false)
                              .Where(x => x.DateCancelled != null)
                              .Where(x => x.Remarks != null);

            return await cancelpo.ToListAsync();   
        }
        public async Task<bool> ReturnPoInAvailableList(ImportPOSummary summary)
        {
            var existingInfo = await _context.POSummary.Where(x => x.Id == summary.Id)
                                                       .FirstOrDefaultAsync();
            if (existingInfo == null)
                return false;

            existingInfo.IsActive = true;
            existingInfo.Date_Cancellation = null;
            existingInfo.Reason = null;
    
            return true;
        }

        public async Task<IReadOnlyList<NearlyExpireDto>> GetAllNearlyExpireRawMaterial()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateadd = DateTime.Now.AddDays(30);
 
            var expiry = (from summary in _context.POSummary
                          join receiving in _context.QC_Receiving on summary.Id equals receiving.PO_Summary_Id
                          where receiving.Expiry_Date <= dateadd
                          select new NearlyExpireDto
                          {
                              Id = summary.Id,
                              PO_Number = summary.PO_Number,
                              ItemCode = summary.ItemCode,
                              ItemDescription = summary.ItemDescription,
                              Supplier = summary.VendorName,
                              Uom = summary.UOM,
                              QuantityOrdered = summary.Ordered,
                              ActualGood = receiving.Actual_Delivered,
                              ActualRemaining = summary.Ordered - receiving.Actual_Delivered,
                              ExpiryDate = receiving.Expiry_Date.ToString("MM/dd/yyyy"),
                              Days = receiving.Expiry_Date.Subtract(dateNow).Days,
                              IsActive = receiving.IsActive,
                              IsNearlyExpire = receiving.IsNearlyExpire != null,
                              ExpiryIsApprove = receiving.ExpiryIsApprove != null
                          });

            return await expiry
                                .Where(x => x.IsNearlyExpire == true)
                                .Where(x => x.ExpiryIsApprove == false)
                                .ToListAsync();
  
        }

        public async Task<bool> ApproveNearlyExpireRawMaterials(PO_Receiving receive)
        {
            var existingInfo = await _context.QC_Receiving.Where(x => x.Id == receive.Id)
                                                          .FirstOrDefaultAsync();
            if (existingInfo == null)
                return false;

            existingInfo.ExpiryIsApprove = true;
            existingInfo.ExpiryApproveBy = receive.ExpiryApproveBy;
            existingInfo.ExpiryDateOfApprove = DateTime.Now;



            return true;
        }

        public async Task<IReadOnlyList<WarehouseReceivingDto>> GetAllRawMaterialsForWarehouseReceiving()
        {

            var warehouse = (from posummary in _context.POSummary
                             join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                             select new WarehouseReceivingDto
                             {
                                 Id = receive.Id,
                                 PO_Number = posummary.PO_Number,
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 QuantityOrdered = posummary.Ordered,
                                 ActualGood = receive.Actual_Delivered,
                                 Reject = receive.TotalReject,
                                 ExpirationDate = receive.Expiry_Date.ToString("MM/dd/yyyy"),
                                 QC_ReceivedDate = receive.QC_ReceiveDate.ToString("MM/dd/yyyy"),
                                 IsActive = receive.IsActive,
                                 IsWareHouseReceive = receive.IsWareHouseReceive != null,
                                 IsExpiryApprove = receive.ExpiryIsApprove != null
                             });

            return await warehouse.Where(x => x.IsWareHouseReceive == false)
                                  .Where(x => x.IsExpiryApprove == true)
                                  .Where(x => x.IsActive == true)
                                  .ToListAsync();

        }

        public async Task<bool> CancelPartialRecevingInQC (PO_Receiving receiving)
        {
            var receivingpo = await _context.QC_Receiving.Where(x => x.Id == receiving.Id)
                                                         .FirstOrDefaultAsync();

            receivingpo.IsActive = false;
            receivingpo.CancelDate = DateTime.Now;
            receivingpo.CancelBy = receiving.CancelBy;
            receivingpo.CancelRemarks = receiving.CancelRemarks;
            receivingpo.Reason = receiving.Reason;

            return true;
        }

        public async Task<bool> RejectRawMaterialsNearlyExpire(PO_Receiving receiving)
        {
            var validateNearlyExpire = await _context.QC_Receiving.Where(x => x.Id == receiving.Id)
                                                                  .FirstOrDefaultAsync();


            if (validateNearlyExpire == null)
                return false;

            validateNearlyExpire.IsActive = false;
            validateNearlyExpire.ExpiryIsApprove = false;

            return true;
        }

        public async Task<bool> WarehouseConfirmRejectByQc(WarehouseReceiving warehouse)
        {
            var existingInfo = await _context.WarehouseReceived.Where(x => x.QcReceivingId == warehouse.QcReceivingId)
                                                               .FirstOrDefaultAsync();

            var validateQcReceiving = await _context.QC_Receiving.FirstOrDefaultAsync(x => x.Id == warehouse.QcReceivingId);


            var rejectWarehouse = await _context.Warehouse_Reject.Where(x => x.WarehouseReceivingId == existingInfo.Id)
                                                                 .SumAsync(x => x.Quantity);

            var validateWarehouseReject = await _context.Warehouse_Reject.Where(x => x.WarehouseReceivingId == existingInfo.Id)
                                                                         .ToListAsync();


            if (existingInfo == null)
                return false;

            validateQcReceiving.Actual_Delivered = validateQcReceiving.Actual_Delivered - rejectWarehouse;
            existingInfo.ConfirmRejectbyQc = true;
            validateQcReceiving.ConfirmRejectByQc = true;


            return true;
        }

        public async Task<bool> WarehouseReturnRejectByQc(PO_Receiving receiving)
        {
            var itemWarehouse = await _context.WarehouseReceived
                                                                .FirstOrDefaultAsync(x => x.QcReceivingId == receiving.Id);

            var itemRejectWarehouse = await _context.Warehouse_Reject
                                                                     .FirstOrDefaultAsync(x => x.WarehouseReceivingId == itemWarehouse.Id);
           var itemQc = await _context.QC_Receiving
                                                   .FirstOrDefaultAsync(x => x.Id == receiving.Id);

            receiving.Actual_Delivered = itemRejectWarehouse.Quantity;
            receiving.Expiry_Date = itemQc.Expiry_Date;
            receiving.Manufacturing_Date = itemQc.Manufacturing_Date;
            receiving.Expected_Delivery = itemQc.Expected_Delivery;
            receiving.Batch_No = itemQc.Batch_No;
            receiving.Expiry_Date = itemQc.Expiry_Date;
            receiving.ExpiryIsApprove = itemQc.ExpiryIsApprove;
            receiving.PO_Summary_Id = itemQc.PO_Summary_Id;
            receiving.IsActive = itemQc.IsActive;
            receiving.QC_ReceiveDate = itemQc.QC_ReceiveDate;
            receiving.Id = 0;

            itemWarehouse.ConfirmRejectbyWarehouse = false;

               await AddNewReceivingInformation(receiving);

            return true;

        }

        public async Task<IReadOnlyList<RejectWarehouseReceivingDto>> GetAllWarehouseConfirmReject()
        {
            var reject = (from posummary in _context.POSummary
                          join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                          select new
                          {
                              Id = receive.Id,
                              PO_Number = posummary.PO_Number,
                              ItemCode = posummary.ItemCode,
                              ItemDescription = posummary.ItemDescription,
                              Supplier = posummary.VendorName,
                              Uom = posummary.UOM,
                              QuantityOrderded = posummary.Ordered
                          });

            return await reject 
                               .Join(_context.WarehouseReceived,
                                        reject => reject.Id,
                                        warehouse => warehouse.QcReceivingId,
                                       (reject, warehouse) => new RejectWarehouseReceivingDto
                                       {
                                           Id = warehouse.Id,
                                           PO_Number = reject.PO_Number,
                                           ItemCode = reject.ItemCode,
                                           ItemDescription = reject.ItemDescription,
                                           Supplier = reject.Supplier,
                                           Uom = reject.Uom,
                                           QuantityOrdered = reject.QuantityOrderded,
                                           ActualGood = warehouse.ActualGood,
                                           ReceivingDate = warehouse.ReceivingDate.ToString("MM/dd/yyyy"),
                                           IsWarehouseReceived = warehouse.IsWarehouseReceive,
                                           Remarks = warehouse.Reason,
                                           ConfirmRejectByWarehouse = warehouse.ConfirmRejectbyWarehouse,
                                           ConfirmRejectByQc = warehouse.ConfirmRejectbyQc

                                       }).Where(x => x.IsWarehouseReceived == true)
                                         .Where(x => x.ConfirmRejectByWarehouse == true)
                                         .Where(x => x.ConfirmRejectByQc == true)
                                         .ToListAsync();

        }

        public async Task<bool> ValidateActualRemaining(PO_Receiving receiving)
        {
            var validateActualRemaining = await (from posummary in _context.POSummary
                                                 join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                                                 from receive in leftJ.DefaultIfEmpty()
                                                 where posummary.IsActive == true
                                                 select new PoSummaryChecklistDto
                                                 {
                                                     Id = posummary.Id,
                                                     PO_Number = posummary.PO_Number,
                                                     ItemCode = posummary.ItemCode,
                                                     ItemDescription = posummary.ItemDescription,
                                                     Supplier = posummary.VendorName,
                                                     UOM = posummary.UOM,
                                                     QuantityOrdered = posummary.Ordered,
                                                     ActualGood = receive != null && receive.IsActive != false ? receive.Actual_Delivered : 0,
                                                     IsActive = posummary.IsActive,
                                                     ActualRemaining = 0,
                                                     IsQcReceiveIsActive = receive != null ? receive.IsActive : true
                                                 })
                                                        .GroupBy(x => new
                                                        {
                                                            x.Id,
                                                            x.PO_Number,
                                                            x.ItemCode,
                                                            x.ItemDescription,
                                                            x.UOM,
                                                            x.QuantityOrdered,
                                                            x.IsActive,
                                                            x.IsQcReceiveIsActive
                                                        })
                                                   .Select(receive => new PoSummaryChecklistDto
                                                   {
                                                       Id = receive.Key.Id,
                                                       PO_Number = receive.Key.PO_Number,
                                                       ItemCode = receive.Key.ItemCode,
                                                       ItemDescription = receive.Key.ItemDescription,
                                                       UOM = receive.Key.UOM,
                                                       QuantityOrdered = receive.Key.QuantityOrdered,
                                                       ActualGood = receive.Sum(x => x.ActualGood),
                                                       ActualRemaining = ((receive.Key.QuantityOrdered + (receive.Key.QuantityOrdered / 100) * 10) - (receive.Sum(x => x.ActualGood))),
                                                       IsActive = receive.Key.IsActive,
                                                       IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive
                                                   }).Where(x => x.IsQcReceiveIsActive == true)
                                                     .FirstOrDefaultAsync(x => x.Id == receiving.PO_Summary_Id);

            if (validateActualRemaining == null)
                return true;
             
            if (validateActualRemaining.ActualRemaining < receiving.Actual_Delivered)
                return false;

            return true;

        }

        public async Task<bool> ValidateForCancelPo(ImportPOSummary summary)
        {
            var forcancel = await (from posummary in _context.POSummary
                                   join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                                   from receive in leftJ.DefaultIfEmpty()
                                   where posummary.IsActive == true
                                   select new PoSummaryChecklistDto
                                   {
                                       Id = posummary.Id,
                                       PO_Number = posummary.PO_Number,
                                       ItemCode = posummary.ItemCode,
                                       ItemDescription = posummary.ItemDescription,
                                       Supplier = posummary.VendorName,
                                       UOM = posummary.UOM,
                                       QuantityOrdered = posummary.Ordered,
                                       ActualGood = receive != null && receive.IsActive != false ? receive.Actual_Delivered : 0,
                                       IsActive = posummary.IsActive,
                                       IsQcReceiveIsActive = receive != null && receive.IsActive != false ? receive.IsActive : true,
                                       IsWarehouseReceived = receive.IsWareHouseReceive != null,
                                       ActualRemaining = 0

                                   }).GroupBy(x => new
                                   {
                                       x.Id,
                                       x.PO_Number,
                                       x.ItemCode,
                                       x.ItemDescription,
                                       x.UOM,
                                       x.Supplier,
                                       x.QuantityOrdered,
                                       x.IsActive,
                                       x.IsQcReceiveIsActive,
                                       x.IsWarehouseReceived,

                                   })
                                                   .Select(receive => new PoSummaryChecklistDto
                                                   {
                                                       Id = receive.Key.Id,
                                                       PO_Number = receive.Key.PO_Number,
                                                       ItemCode = receive.Key.ItemCode,
                                                       ItemDescription = receive.Key.ItemDescription,
                                                       UOM = receive.Key.UOM,
                                                       Supplier = receive.Key.Supplier,
                                                       QuantityOrdered = receive.Key.QuantityOrdered,
                                                       ActualGood = receive.Sum(x => x.ActualGood),
                                                       ActualRemaining = receive.Key.QuantityOrdered - (receive.Sum(x => x.ActualGood)),
                                                       IsActive = receive.Key.IsActive,
                                                       IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive,
                                                       IsWarehouseReceived = receive.Key.IsWarehouseReceived
                                                   })
                                                   .OrderBy(x => x.PO_Number)
                                                   .Where(x => x.ActualRemaining != 0)
                                                   .Where(x => x.IsActive == true)
                                                   .Where(x => x.IsQcReceiveIsActive == true)
                                                   .FirstOrDefaultAsync(x => x.Id == summary.Id);

            if (forcancel.ActualGood != 0)
                return false;

            return true;

        }
        public async Task<PagedList<PoSummaryChecklistDto>> GetAllPoSummaryWithPagination(UserParams userParams)
        {
            var poSummary = (from posummary in _context.POSummary
                             where posummary.IsActive == true
                             join receive in _context.QC_Receiving
                             on posummary.Id equals receive.PO_Summary_Id into leftJ
                             from receive in leftJ.DefaultIfEmpty()

                             select new PoSummaryChecklistDto
                             {
                                 Id = posummary.Id,
                                 PO_Number = posummary.PO_Number,
                                 PO_Date = posummary.PO_Date,
                                 PR_Number = posummary.PR_Number,
                                 PR_Date = posummary.PR_Date,
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 UOM = posummary.UOM,
                                 QuantityOrdered = posummary.Ordered,
                                 ActualGood = receive != null && receive.IsActive != false ? receive.Actual_Delivered : 0,
                                 IsActive = posummary.IsActive,
                                 IsQcReceiveIsActive = receive != null && receive.IsActive != false ? receive.IsActive : true,
                                 ActualRemaining = 0,

                             }).GroupBy(x => new
                             {
                                 x.Id,
                                 x.PO_Number,
                                 x.PO_Date,
                                 x.PR_Number,
                                 x.PR_Date,
                                 x.ItemCode,
                                 x.ItemDescription,
                                 x.UOM,
                                 x.Supplier,
                                 x.QuantityOrdered,
                                 x.IsActive,
                                 x.IsQcReceiveIsActive
                             })
                                                    .Select(receive => new PoSummaryChecklistDto
                                                    {
                                                        Id = receive.Key.Id,
                                                        PO_Number = receive.Key.PO_Number,
                                                        PO_Date = receive.Key.PO_Date,
                                                        PR_Number = receive.Key.PR_Number,
                                                        PR_Date = receive.Key.PR_Date,
                                                        ItemCode = receive.Key.ItemCode,
                                                        ItemDescription = receive.Key.ItemDescription,
                                                        UOM = receive.Key.UOM,
                                                        Supplier = receive.Key.Supplier,
                                                        QuantityOrdered = receive.Key.QuantityOrdered,
                                                        ActualGood = receive.Sum(x => x.ActualGood),
                                                        ActualRemaining = receive.Key.QuantityOrdered - (receive.Sum(x => x.ActualGood)),
                                                        IsActive = receive.Key.IsActive,
                                                        IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive

                                                    })
                                                    .OrderBy(x => x.PO_Number)
                                                    .Where(x => x.ActualRemaining != 0 && (x.ActualRemaining > 0))
                                                    .Where(x => x.IsActive == true);               

            return await PagedList<PoSummaryChecklistDto>.CreateAsync(poSummary, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<PoSummaryChecklistDto>> GetPoSummaryByStatusWithPaginationOrig(UserParams userParams, string search)
        {

            var poSummary = (from posummary in _context.POSummary
                             where posummary.IsActive == true
                             join receive in _context.QC_Receiving
                             on posummary.Id equals receive.PO_Summary_Id into leftJ
                             from receive in leftJ.DefaultIfEmpty()

                             select new PoSummaryChecklistDto
                             {
                                 Id = posummary.Id,
                                 PO_Number = posummary.PO_Number,
                                 PO_Date = posummary.PO_Date,
                                 PR_Number = posummary.PR_Number,
                                 PR_Date = posummary.PR_Date,        
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 UOM = posummary.UOM,
                                 QuantityOrdered = posummary.Ordered,
                                 ActualGood = receive != null && receive.IsActive != false ? receive.Actual_Delivered : 0,
                                 IsActive = posummary.IsActive,
                                 IsQcReceiveIsActive = receive != null && receive.IsActive != false ? receive.IsActive : true,
                                 ActualRemaining = 0,

                             }).GroupBy(x => new
                             {
                                 x.Id,
                                 x.PO_Number,
                                 x.PO_Date,
                                 x.PR_Number,
                                 x.PR_Date,
                                 x.ItemCode,
                                 x.ItemDescription,
                                 x.UOM,
                                 x.Supplier,
                                 x.QuantityOrdered,
                                 x.IsActive,
                                 x.IsQcReceiveIsActive
                             })
                                                    .Select(receive => new PoSummaryChecklistDto
                                                    {
                                                        Id = receive.Key.Id,
                                                        PO_Number = receive.Key.PO_Number,
                                                        PO_Date = receive.Key.PO_Date,
                                                        PR_Number = receive.Key.PR_Number,
                                                        PR_Date = receive.Key.PR_Date,
                                                        ItemCode = receive.Key.ItemCode,
                                                        ItemDescription = receive.Key.ItemDescription,
                                                        UOM = receive.Key.UOM,
                                                        Supplier = receive.Key.Supplier,
                                                        QuantityOrdered = receive.Key.QuantityOrdered,
                                                        ActualGood = receive.Sum(x => x.ActualGood),
                                                        ActualRemaining = receive.Key.QuantityOrdered - (receive.Sum(x => x.ActualGood)),
                                                        IsActive = receive.Key.IsActive,
                                                        IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive

                                                    })
                                                      .OrderBy(x => x.PO_Number)
                                                      .Where(x => x.ActualRemaining != 0 && (x.ActualRemaining > 0))
                                                      .Where(x => x.IsActive == true)
                                                      .Where(x => Convert.ToString(x.ItemDescription).ToLower()
                                                      .Contains(search.Trim().ToLower()));

            return await PagedList<PoSummaryChecklistDto>.CreateAsync(poSummary, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<WarehouseReceivingDto>> GetAllAvailableForWarehouseWithPagination(UserParams userParams)
        {
            var warehouse = (from posummary in _context.POSummary 
                             join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                             select new WarehouseReceivingDto
                             {
                                 Id = receive.Id,
                                 PO_Number = posummary.PO_Number,
                                 PO_Date = posummary.PO_Date.ToString("MM/dd/yyyy"),
                                 PR_Number = posummary.PR_Number,
                                 PR_Date = posummary.PR_Date.ToString("MM/dd/yyyy"),
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 QuantityOrdered = posummary.Ordered,
                                 ActualGood = receive.Actual_Delivered,
                                 Reject = receive.TotalReject,
                                 ExpirationDate = receive.Expiry_Date.ToString("MM/dd/yyyy"),
                                 QC_ReceivedDate = receive.QC_ReceiveDate.ToString("MM/dd/yyyy"),
                                 IsActive = receive.IsActive,
                                 IsWareHouseReceive = receive.IsWareHouseReceive != null,
                                 IsExpiryApprove = receive.ExpiryIsApprove != null,
                                 ManufacturingDate = receive.Manufacturing_Date.ToString("MM/dd/yyyy")

                             }).OrderBy(x => x.PO_Number)
                               .Where(x => x.IsWareHouseReceive == false)
                               .Where(x => x.IsExpiryApprove == true)
                               .Where(x => x.IsActive == true);

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouse, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<WarehouseReceivingDto>> GetAllAvailableForWarehouseWithPaginationOrig(UserParams userParams, string search)
        {
            var warehouse = (from posummary in _context.POSummary
                             join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                             select new WarehouseReceivingDto
                             {
                                 Id = receive.Id,
                                 PO_Number = posummary.PO_Number,
                                 PO_Date = posummary.PO_Date.ToString("MM/dd/yyyy"),
                                 PR_Number = posummary.PR_Number,
                                 PR_Date = posummary.PR_Date.ToString("MM/dd/yyyy"),
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 QuantityOrdered = posummary.Ordered,
                                 ActualGood = receive.Actual_Delivered,
                                 Reject = receive.TotalReject,
                                 ExpirationDate = receive.Expiry_Date.ToString("MM/dd/yyyy"),
                                 QC_ReceivedDate = receive.QC_ReceiveDate.ToString("MM/dd/yyyy"),
                                 IsActive = receive.IsActive,
                                 IsWareHouseReceive = receive.IsWareHouseReceive != null,
                                 IsExpiryApprove = receive.ExpiryIsApprove != null,
                                 ManufacturingDate = receive.Manufacturing_Date.ToString("MM/dd/yyyy")


                             }).OrderBy(x => x.PO_Number)
                               .Where(x => x.IsWareHouseReceive == false)
                               .Where(x => x.IsExpiryApprove == true)
                               .Where(x => x.IsActive == true)
                               .Where(x => Convert.ToString(x.PO_Number).ToLower()
                               .Contains(search.Trim().ToLower()));

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouse, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPagination(UserParams userParams)
        {
            var cancelpo = (from posummary in _context.POSummary
                            join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                            from receive in leftJ.DefaultIfEmpty()

    
                            select new CancelledPoDto
                            {
                                Id = posummary.Id,
                                PO_Number = posummary.PO_Number,
                                ItemCode = posummary.ItemCode,
                                ItemDescription = posummary.ItemDescription,
                                Supplier = posummary.VendorName,
                                QuantityOrdered = posummary.Ordered,
                                QuantityCancel = receive != null ? receive.Actual_Delivered : 0,
                                QuantityGood = receive != null ? receive.Actual_Delivered : 0,
                                DateCancelled = posummary.Date_Cancellation.ToString(),
                                Remarks = posummary.Reason,
                                IsActive = posummary.IsActive
                            }).Where(x => x.IsActive == false)
                              .Where(x => x.DateCancelled != null)
                              .Where(x => x.Remarks != null);

            return await PagedList<CancelledPoDto>.CreateAsync(cancelpo, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPaginationOrig(UserParams userParams, string search)
        {
            var cancelpo = (from posummary in _context.POSummary
                            join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                            from receive in leftJ.DefaultIfEmpty()

                            join material in _context.POSummary on posummary.ItemCode equals material.ItemCode

                            select new CancelledPoDto
                            {
                                Id = posummary.Id,
                                PO_Number = posummary.PO_Number,
                                ItemCode = posummary.ItemCode,
                                ItemDescription = material.ItemDescription,
                                Supplier = posummary.VendorName,
                                QuantityOrdered = posummary.Ordered,
                                QuantityCancel = receive != null ? receive.Actual_Delivered : 0,
                                QuantityGood = receive != null ? receive.Actual_Delivered : 0,
                                DateCancelled = posummary.Date_Cancellation.ToString(),
                                Remarks = posummary.Reason,
                                IsActive = posummary.IsActive
                            }).Where(x => x.IsActive == false)
                              .Where(x => x.DateCancelled != null)
                              .Where(x => x.Remarks != null)
                              .Where(x => Convert.ToString(x.PO_Number).ToLower()
                              .Contains(search.Trim().ToLower()));


            return await PagedList<CancelledPoDto>.CreateAsync(cancelpo, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<NearlyExpireDto>> GetAllNearlyExpireWithPagination(UserParams userParams)
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateadd = DateTime.Now.AddDays(30);

            var expiry = (from summary in _context.POSummary
                          join receiving in _context.QC_Receiving on summary.Id equals receiving.PO_Summary_Id
                          where receiving.Expiry_Date <= dateadd
                          select new NearlyExpireDto
                          {
                              Id = summary.Id,
                              PO_Number = summary.PO_Number,
                              ItemCode = summary.ItemCode,
                              ItemDescription = summary.ItemDescription,
                              Supplier = summary.VendorName,
                              Uom = summary.UOM,
                              QuantityOrdered = summary.Ordered,
                              ActualGood = receiving.Actual_Delivered,
                              ActualRemaining = summary.Ordered - receiving.Actual_Delivered,
                              ExpiryDate = receiving.Expiry_Date.ToString("MM/dd/yyyy"),
                              Days = receiving.Expiry_Date.Subtract(dateNow).Days,
                              IsActive = receiving.IsActive,
                              IsNearlyExpire = receiving.IsNearlyExpire != null,
                              ExpiryIsApprove = receiving.ExpiryIsApprove != null,
                              ReceivingId = receiving.Id

                          }).Where(x => x.IsNearlyExpire == true)
                            .Where(x => x.ExpiryIsApprove == false);
                          

            return await PagedList<NearlyExpireDto>.CreateAsync(expiry, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<NearlyExpireDto>> GetAllNearlyExpireWithPaginationOrig(UserParams userParams, string search)
        {

            DateTime dateNow = DateTime.Now;
            DateTime dateadd = DateTime.Now.AddDays(30);

            var expiry = (from summary in _context.POSummary
                          join receiving in _context.QC_Receiving on summary.Id equals receiving.PO_Summary_Id
                          where receiving.Expiry_Date <= dateadd
                          select new NearlyExpireDto
                          {
                              Id = summary.Id,
                              PO_Number = summary.PO_Number,
                              ItemCode = summary.ItemCode,
                              ItemDescription = summary.ItemDescription,
                              Supplier = summary.VendorName,
                              Uom = summary.UOM,
                              QuantityOrdered = summary.Ordered,
                              ActualGood = receiving.Actual_Delivered,
                              ActualRemaining = summary.Ordered - receiving.Actual_Delivered,
                              ExpiryDate = receiving.Expiry_Date.ToString("MM/dd/yyyy"),
                              Days = receiving.Expiry_Date.Subtract(dateNow).Days,
                              IsActive = receiving.IsActive,
                              IsNearlyExpire = receiving.IsNearlyExpire != null,
                              ExpiryIsApprove = receiving.ExpiryIsApprove != null,
                              ReceivingId = receiving.Id

                          }).Where(x => x.IsNearlyExpire == true)
                            .Where(x => x.ExpiryIsApprove == false)
                            .Where(x => Convert.ToString(x.PO_Number).ToLower()
                            .Contains(search.Trim().ToLower()));

            return await PagedList<NearlyExpireDto>.CreateAsync(expiry, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<RejectWarehouseReceivingDto>> GetAllConfirmRejectWithPagination(UserParams userParams)
        {
            var reject = (from posummary in _context.POSummary
                          join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                          select new
                          {
                              Id = receive.Id,
                              PO_Number = posummary.PO_Number,
                              ItemCode = posummary.ItemCode,
                              ItemDescription = posummary.ItemDescription,
                              Supplier = posummary.VendorName,
                              Uom = posummary.UOM,
                              QuantityOrderded = posummary.Ordered
                          }).Join(_context.WarehouseReceived,
                                        reject => reject.Id,
                                        warehouse => warehouse.QcReceivingId,
                                       (reject, warehouse) => new RejectWarehouseReceivingDto
                                       {
                                           Id = warehouse.Id,
                                           PO_Number = reject.PO_Number,
                                           ItemCode = reject.ItemCode,
                                           ItemDescription = reject.ItemDescription,
                                           Supplier = reject.Supplier,
                                           Uom = reject.Uom,
                                           QuantityOrdered = reject.QuantityOrderded,
                                           ActualGood = warehouse.ActualGood,
                                           ReceivingDate = warehouse.ReceivingDate.ToString("MM/dd/yyyy"),
                                           IsWarehouseReceived = warehouse.IsWarehouseReceive,
                                           Remarks = warehouse.Reason,
                                           ConfirmRejectByWarehouse = warehouse.ConfirmRejectbyWarehouse,
                                           ConfirmRejectByQc = warehouse.ConfirmRejectbyQc

                                       }).Where(x => x.IsWarehouseReceived == true)
                                         .Where(x => x.ConfirmRejectByWarehouse == true)
                                         .Where(x => x.ConfirmRejectByQc == true);
                                       
            return await PagedList<RejectWarehouseReceivingDto>.CreateAsync(reject, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<RejectWarehouseReceivingDto>> GetAllConfirmRejectWithPaginationOrig(UserParams userParams, string search)
        {
            var reject = (from posummary in _context.POSummary
                          join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                          select new
                          {
                              Id = receive.Id,
                              PO_Number = posummary.PO_Number,
                              ItemCode = posummary.ItemCode,
                              ItemDescription = posummary.ItemDescription,
                              Supplier = posummary.VendorName,
                              Uom = posummary.UOM,
                              QuantityOrderded = posummary.Ordered
                          }).Join(_context.WarehouseReceived,
                                       reject => reject.Id,
                                       warehouse => warehouse.QcReceivingId,
                                      (reject, warehouse) => new RejectWarehouseReceivingDto
                                      {
                                          Id = warehouse.Id,
                                          PO_Number = reject.PO_Number,
                                          ItemCode = reject.ItemCode,
                                          ItemDescription = reject.ItemDescription,
                                          Supplier = reject.Supplier,
                                          Uom = reject.Uom,
                                          QuantityOrdered = reject.QuantityOrderded,
                                          ActualGood = warehouse.ActualGood,
                                          ReceivingDate = warehouse.ReceivingDate.ToString("MM/dd/yyyy"),
                                          IsWarehouseReceived = warehouse.IsWarehouseReceive,
                                          Remarks = warehouse.Reason,
                                          ConfirmRejectByWarehouse = warehouse.ConfirmRejectbyWarehouse,
                                          ConfirmRejectByQc = warehouse.ConfirmRejectbyQc

                                      }).Where(x => x.IsWarehouseReceived == true)
                                        .Where(x => x.ConfirmRejectByWarehouse == true)
                                        .Where(x => x.ConfirmRejectByQc == true)
                                        .Where(x => Convert.ToString(x.PO_Number).ToLower()
                                        .Contains(search.Trim().ToLower()));

            return await PagedList<RejectWarehouseReceivingDto>.CreateAsync(reject, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<IReadOnlyList<NearlyExpireDto>> GetItemDetailsForNearlyExpire(int id)
        {

            var summary = (from posummary in _context.POSummary
                           join receiving in _context.QC_Receiving
                           on posummary.Id equals receiving.PO_Summary_Id
                           into leftJ1
                           from receiving in leftJ1.DefaultIfEmpty()

                           select new NearlyExpireDto
                           {
                               Id = receiving.Id, 
                               PO_Number = posummary.PO_Number,
                               PO_Date = posummary.PO_Date.ToString(),
                               PR_Number = posummary.PR_Number, 
                               PR_Date = posummary.PR_Date.ToString(),
                               ItemCode = posummary.ItemCode, 
                               ItemDescription = posummary.ItemDescription,
                               Supplier = posummary.VendorName, 
                               QuantityOrdered = receiving.Actual_Delivered,
                               ManufacturingDate = receiving.Manufacturing_Date.ToString(),
                               DateOfChecking = receiving.QC_ReceiveDate.ToString(),
                               ExpiryIsApprove = receiving.ExpiryIsApprove != null ,
                               ExpiryDate = receiving.Expiry_Date.ToString(),
                               TruckApproval1 = receiving.Truck_Approval1,
                               TruckApprovalRemarks1 = receiving.Truck_Approval1_Remarks,
                               TruckApproval2 = receiving.Truck_Approval2,
                               TruckApprovalRemarks2 = receiving.Truck_Approval2_Remarks,
                               TruckApproval3 = receiving.Truck_Approval3,
                               TruckApprovalRemarks3 = receiving.Truck_Approval3_Remarks,
                               TruckApproval4 = receiving.Truck_Approval4,
                               TruckApprovalRemarks4 = receiving.Truck_Approval4_Remarks,
                               UnloadingApproval1 = receiving.Unloading_Approval1,
                               UnloadingApprovalRemarks1 = receiving.Unloading_Approval1_Remarks,
                               UnloadingApproval2 = receiving.Unloading_Approval2,
                               UnloadingApprovalRemarks2 = receiving.Unloading_Approval2_Remarks,
                               UnloadingApproval3 = receiving.Unloading_Approval3,
                               UnloadingApprovalRemarks3 = receiving.Unloading_Approval3_Remarks,
                               UnloadingApproval4 = receiving.Unloading_Approval4,
                               UnloadingApprovalRemarks4 = receiving.Unloading_Approval4_Remarks,
                               CheckingApproval1 = receiving.Checking_Approval1,
                               CheckingApprovalRemarks1 = receiving.Checking_Approval1_Remarks,
                               CheckingApproval2 = receiving.Checking_Approval2,
                               CheckingApprovalRemarks2 = receiving.Checking_Approval2_Remarks,
                               QAApproval = receiving.QA_Approval,
                               QAApprovalRemarks = receiving.QA_Approval_Remarks
                           });

            return await summary.Where(x => x.Id == id)
                                .ToListAsync();

        }

        public async Task<bool> ValidatePOForCancellation(int id)
        {
            var validatePo = await _context.QC_Receiving.Where(x => x.PO_Summary_Id == id)
                                                        .Where(x => x.IsActive == true)
                                                        .Where(x => x.IsWareHouseReceive != true)
                                                        .ToListAsync();

            if (validatePo.Count != 0)
                return false;

            return true;
        }
    }
}   
  