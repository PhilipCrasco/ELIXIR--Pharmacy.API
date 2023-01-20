using ELIXIR.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class RawMaterialInventory : IRawMaterialInventory
    {
        private readonly StoreContext _context;
        public RawMaterialInventory(StoreContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<RawmaterialInventory>> GetAllAvailbleInRawmaterialInventory()
        {

            return await _context.WarehouseReceived
                                                        .GroupBy(x => new
                                                        {
                                                            x.ItemCode,
                                                            x.ItemDescription,
                                                            x.LotCategory,
                                                            x.Uom,
                                                            x.IsWarehouseReceive,
                                                        })
                                                         .Select(inventory => new RawmaterialInventory
                                                         {
                                                             ItemCode = inventory.Key.ItemCode,
                                                             ItemDescription = inventory.Key.ItemDescription,
                                                             LotCategory = inventory.Key.LotCategory,
                                                             Uom = inventory.Key.Uom,
                                                             SOH = inventory.Sum(x => x.ActualGood),
                                                             ReceiveIn = inventory.Sum(x => x.ActualGood),
                                                             IsWarehouseReceived = inventory.Key.IsWarehouseReceive

                                                         })
                                                          .OrderBy(x => x.ItemCode)
                                                          .Where(x => x.IsWarehouseReceived == true)
                                                          .ToListAsync();
        }

        public Task<PoSummaryInventory> GetPOSummary()
        {
            var getPoSummary = _context.POSummary.Where(x => x.IsActive == true)
               .GroupBy(x => new
               {
                   x.ItemCode,

               }).Select(x => new PoSummaryInventory
               {
                   ItemCode = x.Key.ItemCode,
                   UnitPrice = x.Sum(x => x.UnitPrice),
                   TotalPrice = x.Average(x => x.UnitPrice)

               });

            return (Task<PoSummaryInventory>)getPoSummary;


        }




        public async Task<IReadOnlyList<MRPDto>> GetAllItemForInventory()
        {
            var EndDate = DateTime.Now;
            var StartDate = EndDate.AddDays(-30);

            var getPoSummary = _context.POSummary.Where(x => x.IsActive == true)
              .GroupBy(x => new
              {
                  x.ItemCode,

              }).Select(x => new PoSummaryInventory
              {
                  ItemCode = x.Key.ItemCode,
                  UnitPrice = x.Sum(x => x.UnitPrice),
                  Ordered = x.Sum(x => x.Ordered),
                  TotalPrice = x.Average(x => x.UnitPrice)

              });

            var getWarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .GroupBy(x => new
            {
                x.ItemCode,

            }).Select(x => new WarehouseInventory
            {
                ItemCode = x.Key.ItemCode,
                ActualGood = x.Sum(x => x.ActualGood)

            });

            var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                     .Where(x => x.IsPrepared == true)
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new MoveOrderInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                   });

            var getQCReceivingIn = _context.QC_Receiving.Where(x => x.ExpiryIsApprove == true)
                                                        .Where(x => x.IsActive == true)
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new MoveOrderInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       QuantityOrdered = x.Sum(x => x.Actual_Delivered)

                   });

            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                         .Where(x => x.TransactionType == "MiscellaneousReceipt")
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new ReceiptInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       Quantity = x.Sum(x => x.ActualGood)
                   });

            var getIssueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsTransact == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new IssueInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)
                });

            var getTransformation = _context.Transformation_Preparation.Where(x => x.IsActive == true)                                                              
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
             .GroupBy(x => new
             {
                 x.ItemCode,

             }).Select(x => new WarehouseInventory
             {
                 ItemCode = x.Key.ItemCode,
                 ActualGood = x.Sum(x => x.ActualGood)
             });

            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                    .Where(x => x.PreparedDate != null)
           .GroupBy(x => new
           {
               x.ItemCode,

           }).Select(x => new OrderingInventory
           {
               ItemCode = x.Key.ItemCode,
               QuantityOrdered = x.Sum(x => x.QuantityOrdered)
           });

            var getTransformationReserve = _context.Transformation_Request.Where(x => x.IsActive == true)
           .GroupBy(x => new
           {
               x.ItemCode,

           }).Select(x => new OrderingInventory
           {
               ItemCode = x.Key.ItemCode,
               QuantityOrdered = x.Sum(x => x.Quantity)
           });

            var getSOH = (from warehouse in getWarehouseStock
                          join preparation in getTransformation
                          on warehouse.ItemCode equals preparation.ItemCode
                          into leftJ1
                          from preparation in leftJ1.DefaultIfEmpty()

                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ2
                          from issue in leftJ2.DefaultIfEmpty()

                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ3
                          from moveorder in leftJ3.DefaultIfEmpty()

                          join receipt in getReceiptIn
                          on warehouse.ItemCode equals receipt.ItemCode
                          into leftJ4
                          from receipt in leftJ4.DefaultIfEmpty()


                          group new
                          {
                              warehouse,
                              preparation,
                              moveorder,
                              receipt,
                              issue
                          }

                          by new
                          {
                              warehouse.ItemCode

                          } into total

                          select new SOHInventory
                          {
                              ItemCode = total.Key.ItemCode,
                              SOH = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                    total.Sum(x => x.preparation.WeighingScale == null ? 0 : x.preparation.WeighingScale) -
                                    total.Sum(x => x.moveorder.QuantityOrdered == null ? 0 : x.moveorder.QuantityOrdered) 
                 
                          });


            var getReserve = (from warehouse in getWarehouseStock
                              join request in getTransformationReserve
                              on warehouse.ItemCode equals request.ItemCode
                              into leftJ1
                              from request in leftJ1.DefaultIfEmpty()

                              join ordering in getOrderingReserve
                              on warehouse.ItemCode equals ordering.ItemCode
                              into leftJ2
                              from ordering in leftJ2.DefaultIfEmpty()

                              group new
                              {
                                  warehouse,
                                  request,
                                  ordering
                              }
                              by new
                              {
                                  warehouse.ItemCode,

                              } into total

                              select new ReserveInventory
                              {

                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                           (total.Sum(x => x.request.QuantityOrdered == null ? 0 : x.request.QuantityOrdered) +
                                            total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered))
                              });


            var getSuggestedPo = (from posummary in getPoSummary
                                  join receive in getQCReceivingIn
                                  on posummary.ItemCode equals receive.ItemCode
                                  into leftJ
                                  from receive in leftJ.DefaultIfEmpty()

                                  group new
                                  {
                                      posummary,
                                      receive
                                  }
                                  by new
                                  {
                                      posummary.ItemCode

                                  } into total

                                  select new PoSummaryInventory
                                  {
                                      ItemCode = total.Key.ItemCode,
                                      Ordered = total.Sum(x => x.posummary.Ordered == null ? 0 : x.posummary.Ordered) -
                                              total.Sum(x => x.receive.QuantityOrdered == null ? 0 : x.receive.QuantityOrdered)
                                  });

            var getTransformOutPerMonth = _context.Transformation_Preparation.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                                             .Where(x => x.IsActive == true)
                                                                             .Where(x => x.IsMixed == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getMoveOrderOutPerMonth = _context.MoveOrders.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                                           .Where(x => x.IsActive == true)
                                                                           .Where(x => x.IsPrepared == true)
              .GroupBy(x => new
              {
                  x.ItemCode,

              }).Select(x => new MoveOrderInventory
              {
                  ItemCode = x.Key.ItemCode,
                  QuantityOrdered = x.Sum(x => x.QuantityOrdered)
              });

            var getAverageIssuance = (from warehouse in getWarehouseStock
                                      join transformation in getTransformOutPerMonth
                                      on warehouse.ItemCode equals transformation.ItemCode
                                      into leftJ1
                                      from transformation in leftJ1.DefaultIfEmpty()

                                      join ordering in getMoveOrderOutPerMonth
                                      on warehouse.ItemCode equals ordering.ItemCode
                                      into leftJ2
                                      from ordering in leftJ2.DefaultIfEmpty()

                                      group new
                                      {
                                          warehouse,
                                          transformation,
                                          ordering
                                      }

                                      by new
                                      {

                                          warehouse.ItemCode

                                      } into total

                                      select new WarehouseInventory
                                      {

                                          ItemCode = total.Key.ItemCode,
                                          ActualGood = (total.Sum(x => x.transformation.WeighingScale == null ? 0 : x.transformation.WeighingScale) +
                                                       total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered)) / 30
                                      });


            var getReserveUsage = (from warehouse in getWarehouseStock
                              join request in getTransformationReserve
                              on warehouse.ItemCode equals request.ItemCode
                              into leftJ1
                              from request in leftJ1.DefaultIfEmpty()

                              join ordering in getOrderingReserve
                              on warehouse.ItemCode equals ordering.ItemCode
                              into leftJ2
                              from ordering in leftJ2.DefaultIfEmpty()

                              group new
                              {
                                  warehouse,
                                  request,
                                  ordering
                              }
                              by new
                              {
                                  warehouse.ItemCode,

                              } into total

                              select new ReserveInventory
                              {
                                  ItemCode = total.Key.ItemCode,
                                  Reserve = (total.Sum(x => x.request.QuantityOrdered == null ? 0 : x.request.QuantityOrdered) +
                                            total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered))
                              });

            
            var getTransformTo = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                           .Where(x => x.TransactionType == "Transformation")
              .GroupBy(x => new

              {
                  x.ItemCode,

              }).Select(x => new ReceiptInventory
              {
                  ItemCode = x.Key.ItemCode,
                  Quantity = x.Sum(x => x.ActualGood)
              });



            //var lastUsed = (from warehouse in getWarehouseStock
            //                join moveorder in getMoveOrderOut
            //                on warehouse.ItemCode equals moveorder.ItemCode
            //                into leftJ1
            //                from moveorder in leftJ1.DefaultIfEmpty()

            //                group new
            //                {
            //                    warehouse,
            //                    moveorder
            //                }
            //             by new
            //             {
            //                 warehouse.ItemCode, 

            //             } into total

            //var getLastUsed = from a in getWarehouseStock
            //                join b in _context.MoveOrders
            //                on a.ItemCode equals b.ItemCode
            //                into leftJ
            //                from b in leftJ.DefaultIfEmpty()
            //                orderby b.PreparedDate descending
            //                group b by a.ItemCode
            //                into groupA
            //                select new
            //                {
            //                    ItemCode = groupA.Key,
            //                    LastUsed = (from a in groupA                                 
            //                                 select a.PreparedDate.ToString()
            //                                ).FirstOrDefault()
            //                };

            //var x = getLastUsed;

            var inventory = (from rawmaterial in _context.RawMaterials
                             join posummary in getPoSummary
                             on rawmaterial.ItemCode equals posummary.ItemCode
                             into leftJ1
                             from posummary in leftJ1.DefaultIfEmpty()

                             join warehouse in getWarehouseIn
                             on rawmaterial.ItemCode equals warehouse.ItemCode
                             into leftJ2
                             from warehouse in leftJ2.DefaultIfEmpty()

                             join moveorders in getMoveOrderOut
                             on rawmaterial.ItemCode equals moveorders.ItemCode
                             into leftJ3
                             from moveorders in leftJ3.DefaultIfEmpty()

                             join qcreceive in getQCReceivingIn
                             on rawmaterial.ItemCode equals qcreceive.ItemCode
                             into leftJ4
                             from qcreceive in leftJ4.DefaultIfEmpty()

                             join receiptin in getReceiptIn
                             on rawmaterial.ItemCode equals receiptin.ItemCode
                             into leftJ5
                             from receiptin in leftJ5.DefaultIfEmpty()

                             join issueout in getIssueOut
                             on rawmaterial.ItemCode equals issueout.ItemCode
                             into leftJ6
                             from issueout in leftJ6.DefaultIfEmpty()

                             join SOH in getSOH
                             on rawmaterial.ItemCode equals SOH.ItemCode
                             into leftJ7
                             from SOH in leftJ7.DefaultIfEmpty()

                             join Reserve in getReserve
                             on rawmaterial.ItemCode equals Reserve.ItemCode
                             into leftJ8
                             from Reserve in leftJ8.DefaultIfEmpty()

                             join suggestedpo in getSuggestedPo
                             on rawmaterial.ItemCode equals suggestedpo.ItemCode
                             into leftJ9
                             from suggestedpo in leftJ9.DefaultIfEmpty()

                             join averageissuance in getAverageIssuance
                             on rawmaterial.ItemCode equals averageissuance.ItemCode
                             into leftJ10
                             from averageissuance in leftJ10.DefaultIfEmpty()

                             join reserveusage in getReserveUsage
                             on rawmaterial.ItemCode equals reserveusage.ItemCode
                             into leftJ11
                             from reserveusage in leftJ11.DefaultIfEmpty()

                             join transformto in getTransformTo
                             on rawmaterial.ItemCode equals transformto.ItemCode
                             into leftJ12
                             from transformto in leftJ12.DefaultIfEmpty()

                             join transformfrom in getTransformation
                             on rawmaterial.ItemCode equals transformfrom.ItemCode
                             into leftJ13
                             from transformfrom in leftJ13.DefaultIfEmpty()


                             //join lastused in getLastUsed
                             //on rawmaterial.ItemCode equals lastused.ItemCode
                             //into leftJ14
                             //from lastused in leftJ14.DefaultIfEmpty()


                                 //join lastused in last_used
                                 //on rawmaterial.ItemCode equals lastused.item_code
                                 //into leftJ14
                                 //from lastused in leftJ14.DefaultIfEmpty()



                                 //     join lastUsed in x
                                 //on rawmaterial.ItemCode equals lastUsed.ItemCode
                                 //into leftJ11
                                 //from lastUsed in leftJ11.DefaultIfEmpty()

                             group new { 

                                         posummary, 
                                         warehouse,
                                         moveorders,
                                         qcreceive,
                                         receiptin,
                                         issueout,
                                         SOH,
                                         Reserve,
                                         suggestedpo,
                                         averageissuance,
                                         reserveusage,
                                         transformto,
                                         transformfrom,
                                     //    lastused
                                         //lastused
                                 //   lastUsed
                             }
                             by new
                             {
                                 rawmaterial.ItemCode,
                                 rawmaterial.ItemDescription,
                                 rawmaterial.UOM.UOM_Code, 
                                 rawmaterial.ItemCategory.ItemCategoryName,
                                 rawmaterial.BufferLevel,
                                 UnitPrice = posummary.UnitPrice != null ? posummary.UnitPrice : 0,
                                 SuggestedPo = suggestedpo.Ordered != null ? suggestedpo.Ordered : 0,
                                 WarehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                 ReceiptIn = receiptin.Quantity != null ? receiptin.Quantity : 0,
                                 MoveOrderOut = moveorders.QuantityOrdered != null ? moveorders.QuantityOrdered : 0,
                                 QcReceiving = qcreceive.QuantityOrdered != null ? qcreceive.QuantityOrdered : 0,
                                 IssueOut = issueout.Quantity != null ? issueout.Quantity : 0,
                                 TotalPrice = posummary.TotalPrice != null ? posummary.TotalPrice : 0,
                                 SOH = SOH.SOH != null ? SOH.SOH : 0,
                                 Reserve = Reserve.Reserve != null ? Reserve.Reserve : 0,
                                 AverageIssuance = averageissuance.ActualGood != null ? averageissuance.ActualGood : 0,
                                 ReserveUsage = reserveusage.Reserve != null ? reserveusage.Reserve : 0,
                                 TransformTo = transformto.Quantity != null ? transformto.Quantity : 0,
                                 TransformFrom = transformfrom.WeighingScale != null ? transformfrom.WeighingScale : 0,
                                 //LastUsed
                              //   LastUsed = lastused.last_used != null ? lastused.last_used : null
                                 //  LastUsed = lastUsed.PreparedDate != null ? lastUsed.PreparedDate : null

                             } into total

                             select new MRPDto
                             {                          
                                 ItemCode = total.Key.ItemCode,
                                 ItemDescription = total.Key.ItemDescription,
                                 Uom = total.Key.UOM_Code,
                                 ItemCategory = total.Key.ItemCategoryName,
                                 BufferLevel = total.Key.BufferLevel,
                                 Price = total.Key.UnitPrice,
                                 ReceiveIn = total.Key.WarehouseActualGood,
                                 MoveOrderOut = total.Key.MoveOrderOut,
                                 QCReceiving = total.Key.QcReceiving,
                                 ReceiptIn = total.Key.ReceiptIn,
                                 IssueOut = total.Key.IssueOut,
                                 TotalPrice = Math.Round(Convert.ToDecimal(total.Key.TotalPrice),2),
                                 SOH = total.Key.SOH - total.Key.IssueOut,
                                 Reserve = total.Key.Reserve - total.Key.IssueOut,
                                 SuggestedPo = total.Key.SuggestedPo,
                                 AverageIssuance = Math.Round(Convert.ToDecimal(total.Key.AverageIssuance), 2),
                                 DaysLevel = Math.Round(Convert.ToDecimal(total.Key.Reserve / (total.Key.AverageIssuance != 0 ? total.Key.AverageIssuance : 1)),2),
                                 ReserveUsage = total.Key.ReserveUsage,
                                 TransformFrom = total.Key.TransformFrom,
                                 TransformTo = total.Key.TransformTo
                                 //LastUsed = total.Key.LastUsed.ToString()                       
                             });

            return await inventory.ToListAsync();

        }

        public async Task<PagedList<MRPDto>> GetAllItemForInventoryPagination(UserParams userParams)
        {
            var EndDate = DateTime.Now;
            var StartDate = EndDate.AddDays(-30);

            var getPoSummary = _context.POSummary.Where(x => x.IsActive == true)
              .GroupBy(x => new
              {
                  x.ItemCode,

              }).Select(x => new PoSummaryInventory
              {
                  ItemCode = x.Key.ItemCode,
                  UnitPrice = x.Sum(x => x.UnitPrice),
                  Ordered = x.Sum(x => x.Ordered),
                  TotalPrice = x.Average(x => x.UnitPrice)

              });

            var getWarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .GroupBy(x => new
            {
                x.ItemCode,

            }).Select(x => new WarehouseInventory
            {
                ItemCode = x.Key.ItemCode,
                ActualGood = x.Sum(x => x.ActualGood)

            });

            var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                     .Where(x => x.IsPrepared == true)
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new MoveOrderInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                   });

            var getQCReceivingIn = _context.QC_Receiving.Where(x => x.IsActive == true)
                                                        .Where(x => x.ExpiryIsApprove == true)
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new MoveOrderInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       QuantityOrdered = x.Sum(x => x.Actual_Delivered)

                   });

            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                         .Where(x => x.TransactionType == "MiscellaneousReceipt")
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new ReceiptInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       Quantity = x.Sum(x => x.ActualGood)
                   });

            var getIssueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new IssueInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)
                });

            var getTransformation = _context.Transformation_Preparation.Where(x => x.IsActive == true)                                                          
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
             .GroupBy(x => new
             {
                 x.ItemCode,

             }).Select(x => new WarehouseInventory
             {
                 ItemCode = x.Key.ItemCode,
                 ActualGood = x.Sum(x => x.ActualGood)
             });

            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                    .Where(x => x.PreparedDate != null)
           .GroupBy(x => new
           {
               x.ItemCode,

           }).Select(x => new OrderingInventory
           {
               ItemCode = x.Key.ItemCode,
               QuantityOrdered = x.Sum(x => x.QuantityOrdered)
           });

            var getTransformationReserve = _context.Transformation_Request.Where(x => x.IsActive == true)
           .GroupBy(x => new
           {
               x.ItemCode,

           }).Select(x => new OrderingInventory
           {
               ItemCode = x.Key.ItemCode,
               QuantityOrdered = x.Sum(x => x.Quantity)
           });

            var getSOH = (from warehouse in getWarehouseStock
                          join preparation in getTransformation
                          on warehouse.ItemCode equals preparation.ItemCode
                          into leftJ1
                          from preparation in leftJ1.DefaultIfEmpty()

                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ2
                          from issue in leftJ2.DefaultIfEmpty()

                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ3
                          from moveorder in leftJ3.DefaultIfEmpty()

                          join receipt in getReceiptIn
                          on warehouse.ItemCode equals receipt.ItemCode
                          into leftJ4
                          from receipt in leftJ4.DefaultIfEmpty()


                          group new
                          {
                              warehouse,
                              preparation,
                              moveorder,
                              receipt,
                              issue
                          }

                          by new
                          {
                              warehouse.ItemCode

                          } into total

                          select new SOHInventory
                          {
                              ItemCode = total.Key.ItemCode,
                              SOH = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                    total.Sum(x => x.preparation.WeighingScale == null ? 0 : x.preparation.WeighingScale) -
                                    total.Sum(x => x.moveorder.QuantityOrdered == null ? 0 : x.moveorder.QuantityOrdered)

                          });


            var getReserve = (from warehouse in getWarehouseStock
                              join request in getTransformationReserve
                              on warehouse.ItemCode equals request.ItemCode
                              into leftJ1
                              from request in leftJ1.DefaultIfEmpty()

                              join ordering in getOrderingReserve
                              on warehouse.ItemCode equals ordering.ItemCode
                              into leftJ2
                              from ordering in leftJ2.DefaultIfEmpty()

                              group new
                              {
                                  warehouse,
                                  request,
                                  ordering
                              }
                              by new
                              {
                                  warehouse.ItemCode,

                              } into total

                              select new ReserveInventory
                              {

                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                           (total.Sum(x => x.request.QuantityOrdered == null ? 0 : x.request.QuantityOrdered) +
                                            total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered))
                              });

            var getSuggestedPo = (from posummary in getPoSummary
                                  join receive in getQCReceivingIn
                                  on posummary.ItemCode equals receive.ItemCode
                                  into leftJ
                                  from receive in leftJ.DefaultIfEmpty()

                                  group new
                                  {
                                      posummary,
                                      receive
                                  }
                                  by new
                                  {
                                      posummary.ItemCode

                                  } into total

                                  select new PoSummaryInventory
                                  {
                                      ItemCode = total.Key.ItemCode,
                                      Ordered = total.Sum(x => x.posummary.Ordered == null ? 0 : x.posummary.Ordered) -
                                              total.Sum(x => x.receive.QuantityOrdered == null ? 0 : x.receive.QuantityOrdered)
                                  });

            var getTransformOutPerMonth = _context.Transformation_Preparation.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                                             .Where(x => x.IsActive == true)                                                                         
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getMoveOrderOutPerMonth = _context.MoveOrders.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                                           .Where(x => x.IsActive == true)
                                                                           .Where(x => x.IsPrepared == true)
              .GroupBy(x => new
              {
                  x.ItemCode,

              }).Select(x => new MoveOrderInventory
              {
                  ItemCode = x.Key.ItemCode,
                  QuantityOrdered = x.Sum(x => x.QuantityOrdered)
              });

            var getAverageIssuance = (from warehouse in getWarehouseStock
                                      join transformation in getTransformOutPerMonth
                                      on warehouse.ItemCode equals transformation.ItemCode
                                      into leftJ1
                                      from transformation in leftJ1.DefaultIfEmpty()

                                      join ordering in getMoveOrderOutPerMonth
                                      on warehouse.ItemCode equals ordering.ItemCode
                                      into leftJ2
                                      from ordering in leftJ2.DefaultIfEmpty()

                                      group new
                                      {
                                          warehouse,
                                          transformation,
                                          ordering
                                      }

                                      by new
                                      {

                                          warehouse.ItemCode

                                      } into total

                                      select new WarehouseInventory
                                      {

                                          ItemCode = total.Key.ItemCode,
                                          ActualGood = (total.Sum(x => x.transformation.WeighingScale == null ? 0 : x.transformation.WeighingScale) +
                                                       total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered)) / 30
                                      });


            var getReserveUsage = (from warehouse in getWarehouseStock
                                   join request in getTransformationReserve
                                   on warehouse.ItemCode equals request.ItemCode
                                   into leftJ1
                                   from request in leftJ1.DefaultIfEmpty()

                                   join ordering in getOrderingReserve
                                   on warehouse.ItemCode equals ordering.ItemCode
                                   into leftJ2
                                   from ordering in leftJ2.DefaultIfEmpty()

                                   group new
                                   {
                                       warehouse,
                                       request,
                                       ordering
                                   }
                                   by new
                                   {
                                       warehouse.ItemCode,

                                   } into total

                                   select new ReserveInventory
                                   {
                                       ItemCode = total.Key.ItemCode,
                                       Reserve = (total.Sum(x => x.request.QuantityOrdered == null ? 0 : x.request.QuantityOrdered) +
                                                 total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered))
                                   });

            var getTransformTo = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                       .Where(x => x.TransactionType == "Transformation")
          .GroupBy(x => new

          {
              x.ItemCode,

          }).Select(x => new ReceiptInventory
          {
              ItemCode = x.Key.ItemCode,
              Quantity = x.Sum(x => x.ActualGood)
          });

            //var getLastUsed = (from transform in _context.Transformation_Preparation
            //                   where transform.IsActive == true && transform.IsMixed == true
            //                   select new
            //                   {
            //                       ItemCode = transform.ItemCode,
            //                       PreparedDate = (DateTime?)transform.PreparedDate
            //                   }).Distinct()
            //                   .Union
            //                    (from moveorder in _context.MoveOrders
            //                     where moveorder.IsActive == true && moveorder.IsPrepared == true
            //                     select new
            //                     {
            //                         ItemCode = moveorder.ItemCode,
            //                         PreparedDate = moveorder.PreparedDate

            //                     });
            //.OrderByDescending(x => x.PreparedDate);


            //var x = getLastUsed.ToLookup(x => new
            //{ 
            //    x.PreparedDate,
            //    x.ItemCode
            //}).Distinct();

            // var xx = getLastUsed;

            var inventory = (from rawmaterial in _context.RawMaterials
                             join posummary in getPoSummary
                             on rawmaterial.ItemCode equals posummary.ItemCode
                             into leftJ1
                             from posummary in leftJ1.DefaultIfEmpty()

                             join warehouse in getWarehouseIn
                             on rawmaterial.ItemCode equals warehouse.ItemCode
                             into leftJ2
                             from warehouse in leftJ2.DefaultIfEmpty()

                             join moveorders in getMoveOrderOut
                             on rawmaterial.ItemCode equals moveorders.ItemCode
                             into leftJ3
                             from moveorders in leftJ3.DefaultIfEmpty()

                             join qcreceive in getQCReceivingIn
                             on rawmaterial.ItemCode equals qcreceive.ItemCode
                             into leftJ4
                             from qcreceive in leftJ4.DefaultIfEmpty()

                             join receiptin in getReceiptIn
                             on rawmaterial.ItemCode equals receiptin.ItemCode
                             into leftJ5
                             from receiptin in leftJ5.DefaultIfEmpty()

                             join issueout in getIssueOut
                             on rawmaterial.ItemCode equals issueout.ItemCode
                             into leftJ6
                             from issueout in leftJ6.DefaultIfEmpty()

                             join SOH in getSOH
                             on rawmaterial.ItemCode equals SOH.ItemCode
                             into leftJ7
                             from SOH in leftJ7.DefaultIfEmpty()

                             join Reserve in getReserve
                             on rawmaterial.ItemCode equals Reserve.ItemCode
                             into leftJ8
                             from Reserve in leftJ8.DefaultIfEmpty()

                             join suggestedpo in getSuggestedPo
                             on rawmaterial.ItemCode equals suggestedpo.ItemCode
                             into leftJ9
                             from suggestedpo in leftJ9.DefaultIfEmpty()

                             join averageissuance in getAverageIssuance
                             on rawmaterial.ItemCode equals averageissuance.ItemCode
                             into leftJ10
                             from averageissuance in leftJ10.DefaultIfEmpty()

                             join reserveusage in getReserveUsage
                             on rawmaterial.ItemCode equals reserveusage.ItemCode
                             into leftJ11
                             from reserveusage in leftJ11.DefaultIfEmpty()

                             join transformto in getTransformTo
                             on rawmaterial.ItemCode equals transformto.ItemCode
                             into leftJ12
                             from transformto in leftJ12.DefaultIfEmpty()

                             join transformfrom in getTransformation
                             on rawmaterial.ItemCode equals transformfrom.ItemCode
                             into leftJ13
                             from transformfrom in leftJ13.DefaultIfEmpty()

                             group new
                             {

                                 posummary,
                                 warehouse,
                                 moveorders,
                                 qcreceive,
                                 receiptin,
                                 issueout,
                                 SOH,
                                 Reserve,
                                 suggestedpo,
                                 averageissuance,
                                 reserveusage,
                                 transformto,
                                 transformfrom
                             }
                             by new
                             {
                                 rawmaterial.ItemCode,
                                 rawmaterial.ItemDescription,
                                 rawmaterial.UOM.UOM_Code,
                                 rawmaterial.ItemCategory.ItemCategoryName,
                                 rawmaterial.BufferLevel,
                                 UnitPrice = posummary.UnitPrice != null ? posummary.UnitPrice : 0,
                                 SuggestedPo = suggestedpo.Ordered != null ? suggestedpo.Ordered : 0,
                                 WarehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                 ReceiptIn = receiptin.Quantity != null ? receiptin.Quantity : 0,
                                 MoveOrderOut = moveorders.QuantityOrdered != null ? moveorders.QuantityOrdered : 0,
                                 QcReceiving = qcreceive.QuantityOrdered != null ? qcreceive.QuantityOrdered : 0,
                                 IssueOut = issueout.Quantity != null ? issueout.Quantity : 0,
                                 TotalPrice = posummary.TotalPrice != null ? posummary.TotalPrice : 0,
                                 SOH = SOH.SOH != null ? SOH.SOH : 0,
                                 Reserve = Reserve.Reserve != null ? Reserve.Reserve : 0,
                                 AverageIssuance = averageissuance.ActualGood != null ? averageissuance.ActualGood : 0,
                                 ReserveUsage = reserveusage.Reserve != null ? reserveusage.Reserve : 0,
                                 TransformFrom = transformfrom.WeighingScale != null ? transformfrom.WeighingScale : 0,
                                 TransformTo = transformto.Quantity != null ? transformto.Quantity : 0
                                 //  LastUsed = lastUsed.PreparedDate != null ? lastUsed.PreparedDate : null

                             } into total

                             select new MRPDto
                             {
                                 ItemCode = total.Key.ItemCode,
                                 ItemDescription = total.Key.ItemDescription,
                                 Uom = total.Key.UOM_Code,
                                 ItemCategory = total.Key.ItemCategoryName,
                                 BufferLevel = total.Key.BufferLevel,
                                 Price = total.Key.UnitPrice,
                                 ReceiveIn = total.Key.WarehouseActualGood,
                                 MoveOrderOut = total.Key.MoveOrderOut,
                                 QCReceiving = total.Key.QcReceiving,
                                 ReceiptIn = total.Key.ReceiptIn,
                                 IssueOut = total.Key.IssueOut,
                                 TotalPrice = Math.Round(Convert.ToDecimal(total.Key.TotalPrice), 2),
                                 SOH = total.Key.SOH - total.Key.IssueOut,
                                 Reserve = total.Key.Reserve - total.Key.IssueOut,
                                 SuggestedPo = total.Key.SuggestedPo,
                                 AverageIssuance = Math.Round(Convert.ToDecimal(total.Key.AverageIssuance), 2),
                                 DaysLevel = Math.Round(Convert.ToDecimal(total.Key.Reserve / (total.Key.AverageIssuance != 0 ? total.Key.AverageIssuance : 1)), 2),
                                 ReserveUsage = total.Key.ReserveUsage,
                                 TransformFrom = total.Key.TransformFrom,
                                 TransformTo = total.Key.TransformTo
                                 //LastUsed = (from a in _context.MoveOrders
                                 //            orderby a.PreparedDate descending
                                 //            select new MoveOrderDto
                                 //            {
                                 //              PreparedDate = a.PreparedDate.ToString()
                                 //            }).FirstOrDefault()

                             });

            return await PagedList<MRPDto>.CreateAsync(inventory, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<MRPDto>> GetAllItemForInventoryPaginationOrig(UserParams userParams, string search)
        {
            var EndDate = DateTime.Now;
            var StartDate = EndDate.AddDays(-30);

            var getPoSummary = _context.POSummary.Where(x => x.IsActive == true)
              .GroupBy(x => new
              {
                  x.ItemCode,

              }).Select(x => new PoSummaryInventory
              {
                  ItemCode = x.Key.ItemCode,
                  UnitPrice = x.Sum(x => x.UnitPrice),
                  Ordered = x.Sum(x => x.Ordered),
                  TotalPrice = x.Average(x => x.UnitPrice)

              });

            var getWarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .GroupBy(x => new
            {
                x.ItemCode,

            }).Select(x => new WarehouseInventory
            {
                ItemCode = x.Key.ItemCode,
                ActualGood = x.Sum(x => x.ActualGood)

            });

            var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                     .Where(x => x.IsPrepared == true)
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new MoveOrderInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                   });

            var getQCReceivingIn = _context.QC_Receiving.Where(x => x.ExpiryIsApprove == true)
                                                        .Where(x => x.IsActive == true)
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new MoveOrderInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       QuantityOrdered = x.Sum(x => x.Actual_Delivered)

                   });

            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                         .Where(x => x.TransactionType == "MiscellaneousReceipt")
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new ReceiptInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       Quantity = x.Sum(x => x.ActualGood)
                   });

            var getIssueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsTransact == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new IssueInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)
                });

            var getTransformation = _context.Transformation_Preparation.Where(x => x.IsActive == true)
                                                                      
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
             .GroupBy(x => new
             {
                 x.ItemCode,

             }).Select(x => new WarehouseInventory
             {
                 ItemCode = x.Key.ItemCode,
                 ActualGood = x.Sum(x => x.ActualGood)
             });

            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                    .Where(x => x.PreparedDate != null)
           .GroupBy(x => new
           {
               x.ItemCode,

           }).Select(x => new OrderingInventory
           {
               ItemCode = x.Key.ItemCode,
               QuantityOrdered = x.Sum(x => x.QuantityOrdered)
           });

            var getTransformationReserve = _context.Transformation_Request.Where(x => x.IsActive == true)
           .GroupBy(x => new
           {
               x.ItemCode,

           }).Select(x => new OrderingInventory
           {
               ItemCode = x.Key.ItemCode,
               QuantityOrdered = x.Sum(x => x.Quantity)
           });

            var getSOH = (from warehouse in getWarehouseStock
                          join preparation in getTransformation
                          on warehouse.ItemCode equals preparation.ItemCode
                          into leftJ1
                          from preparation in leftJ1.DefaultIfEmpty()

                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ2
                          from issue in leftJ2.DefaultIfEmpty()

                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ3
                          from moveorder in leftJ3.DefaultIfEmpty()

                          join receipt in getReceiptIn
                          on warehouse.ItemCode equals receipt.ItemCode
                          into leftJ4
                          from receipt in leftJ4.DefaultIfEmpty()


                          group new
                          {
                              warehouse,
                              preparation,
                              moveorder,
                              receipt,
                              issue
                          }

                          by new
                          {
                              warehouse.ItemCode

                          } into total

                          select new SOHInventory
                          {
                              ItemCode = total.Key.ItemCode,
                              SOH = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                    total.Sum(x => x.preparation.WeighingScale == null ? 0 : x.preparation.WeighingScale) -
                                    total.Sum(x => x.moveorder.QuantityOrdered == null ? 0 : x.moveorder.QuantityOrdered)

                          });


            var getReserve = (from warehouse in getWarehouseStock
                              join request in getTransformationReserve
                              on warehouse.ItemCode equals request.ItemCode
                              into leftJ1
                              from request in leftJ1.DefaultIfEmpty()

                              join ordering in getOrderingReserve
                              on warehouse.ItemCode equals ordering.ItemCode
                              into leftJ2
                              from ordering in leftJ2.DefaultIfEmpty()

                              group new
                              {
                                  warehouse,
                                  request,
                                  ordering
                              }
                              by new
                              {
                                  warehouse.ItemCode,

                              } into total

                              select new ReserveInventory
                              {

                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                           (total.Sum(x => x.request.QuantityOrdered == null ? 0 : x.request.QuantityOrdered) +
                                            total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered))
                              });


            var getSuggestedPo = (from posummary in getPoSummary
                                  join receive in getQCReceivingIn
                                  on posummary.ItemCode equals receive.ItemCode
                                  into leftJ
                                  from receive in leftJ.DefaultIfEmpty()

                                  group new
                                  {
                                      posummary,
                                      receive
                                  }
                                  by new
                                  {
                                      posummary.ItemCode

                                  } into total

                                  select new PoSummaryInventory
                                  {
                                      ItemCode = total.Key.ItemCode,
                                      Ordered = total.Sum(x => x.posummary.Ordered == null ? 0 : x.posummary.Ordered) -
                                              total.Sum(x => x.receive.QuantityOrdered == null ? 0 : x.receive.QuantityOrdered)
                                  });

            var getTransformOutPerMonth = _context.Transformation_Preparation.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                                             .Where(x => x.IsActive == true)
               
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getMoveOrderOutPerMonth = _context.MoveOrders.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                                           .Where(x => x.IsActive == true)
                                                                           .Where(x => x.IsPrepared == true)
              .GroupBy(x => new
              {
                  x.ItemCode,

              }).Select(x => new MoveOrderInventory
              {
                  ItemCode = x.Key.ItemCode,
                  QuantityOrdered = x.Sum(x => x.QuantityOrdered)
              });

            var getAverageIssuance = (from warehouse in getWarehouseStock
                                      join transformation in getTransformOutPerMonth
                                      on warehouse.ItemCode equals transformation.ItemCode
                                      into leftJ1
                                      from transformation in leftJ1.DefaultIfEmpty()

                                      join ordering in getMoveOrderOutPerMonth
                                      on warehouse.ItemCode equals ordering.ItemCode
                                      into leftJ2
                                      from ordering in leftJ2.DefaultIfEmpty()

                                      group new
                                      {
                                          warehouse,
                                          transformation,
                                          ordering
                                      }

                                      by new
                                      {

                                          warehouse.ItemCode

                                      } into total

                                      select new WarehouseInventory
                                      {

                                          ItemCode = total.Key.ItemCode,
                                          ActualGood = (total.Sum(x => x.transformation.WeighingScale == null ? 0 : x.transformation.WeighingScale) +
                                                       total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered)) / 30
                                      });


            var getReserveUsage = (from warehouse in getWarehouseStock
                                   join request in getTransformationReserve
                                   on warehouse.ItemCode equals request.ItemCode
                                   into leftJ1
                                   from request in leftJ1.DefaultIfEmpty()

                                   join ordering in getOrderingReserve
                                   on warehouse.ItemCode equals ordering.ItemCode
                                   into leftJ2
                                   from ordering in leftJ2.DefaultIfEmpty()

                                   group new
                                   {
                                       warehouse,
                                       request,
                                       ordering
                                   }
                                   by new
                                   {
                                       warehouse.ItemCode,

                                   } into total

                                   select new ReserveInventory
                                   {
                                       ItemCode = total.Key.ItemCode,
                                       Reserve = (total.Sum(x => x.request.QuantityOrdered == null ? 0 : x.request.QuantityOrdered) +
                                                 total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered))
                                   });


          var getTransformTo = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                       .Where(x => x.TransactionType == "Transformation")
          .GroupBy(x => new

          {
              x.ItemCode,

          }).Select(x => new ReceiptInventory
          {
              ItemCode = x.Key.ItemCode,
              Quantity = x.Sum(x => x.ActualGood)
          });

            //var getLastUsed = (from transform in _context.Transformation_Preparation
            //                   where transform.IsActive == true && transform.IsMixed == true
            //                   select new
            //                   {
            //                       ItemCode = transform.ItemCode,
            //                       PreparedDate = (DateTime?)transform.PreparedDate
            //                   }).Distinct()
            //                   .Union
            //                    (from moveorder in _context.MoveOrders
            //                     where moveorder.IsActive == true && moveorder.IsPrepared == true
            //                     select new
            //                     {
            //                         ItemCode = moveorder.ItemCode,
            //                         PreparedDate = moveorder.PreparedDate

            //                     });
            //.OrderByDescending(x => x.PreparedDate);


            //var x = getLastUsed.ToLookup(x => new
            //{ 
            //    x.PreparedDate,
            //    x.ItemCode
            //}).Distinct();

            // var xx = getLastUsed;

            var inventory = (from rawmaterial in _context.RawMaterials
                             join posummary in getPoSummary
                             on rawmaterial.ItemCode equals posummary.ItemCode
                             into leftJ1
                             from posummary in leftJ1.DefaultIfEmpty()

                             join warehouse in getWarehouseIn
                             on rawmaterial.ItemCode equals warehouse.ItemCode
                             into leftJ2
                             from warehouse in leftJ2.DefaultIfEmpty()

                             join moveorders in getMoveOrderOut
                             on rawmaterial.ItemCode equals moveorders.ItemCode
                             into leftJ3
                             from moveorders in leftJ3.DefaultIfEmpty()

                             join qcreceive in getQCReceivingIn
                             on rawmaterial.ItemCode equals qcreceive.ItemCode
                             into leftJ4
                             from qcreceive in leftJ4.DefaultIfEmpty()

                             join receiptin in getReceiptIn
                             on rawmaterial.ItemCode equals receiptin.ItemCode
                             into leftJ5
                             from receiptin in leftJ5.DefaultIfEmpty()

                             join issueout in getIssueOut
                             on rawmaterial.ItemCode equals issueout.ItemCode
                             into leftJ6
                             from issueout in leftJ6.DefaultIfEmpty()

                             join SOH in getSOH
                             on rawmaterial.ItemCode equals SOH.ItemCode
                             into leftJ7
                             from SOH in leftJ7.DefaultIfEmpty()

                             join Reserve in getReserve
                             on rawmaterial.ItemCode equals Reserve.ItemCode
                             into leftJ8
                             from Reserve in leftJ8.DefaultIfEmpty()

                             join suggestedpo in getSuggestedPo
                             on rawmaterial.ItemCode equals suggestedpo.ItemCode
                             into leftJ9
                             from suggestedpo in leftJ9.DefaultIfEmpty()

                             join averageissuance in getAverageIssuance
                             on rawmaterial.ItemCode equals averageissuance.ItemCode
                             into leftJ10
                             from averageissuance in leftJ10.DefaultIfEmpty()

                             join reserveusage in getReserveUsage
                             on rawmaterial.ItemCode equals reserveusage.ItemCode
                             into leftJ11
                             from reserveusage in leftJ11.DefaultIfEmpty()

                             join transformto in getTransformTo
                             on rawmaterial.ItemCode equals transformto.ItemCode
                             into leftJ12
                             from transformto in leftJ12.DefaultIfEmpty()

                             join transformfrom in getTransformation
                             on rawmaterial.ItemCode equals transformfrom.ItemCode
                             into leftJ13
                             from transformfrom in leftJ13.DefaultIfEmpty()

                             group new
                             {

                                 posummary,
                                 warehouse,
                                 moveorders,
                                 qcreceive,
                                 receiptin,
                                 issueout,
                                 SOH,
                                 Reserve,
                                 suggestedpo,
                                 averageissuance,
                                 reserveusage,
                                 transformto,
                                 transformfrom
                                 //   lastUsed
                             }
                             by new
                             {
                                 rawmaterial.ItemCode,
                                 rawmaterial.ItemDescription,
                                 rawmaterial.UOM.UOM_Code,
                                 rawmaterial.ItemCategory.ItemCategoryName,
                                 rawmaterial.BufferLevel,
                                 UnitPrice = posummary.UnitPrice != null ? posummary.UnitPrice : 0,
                                 SuggestedPo = suggestedpo.Ordered != null ? suggestedpo.Ordered : 0,
                                 WarehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                 ReceiptIn = receiptin.Quantity != null ? receiptin.Quantity : 0,
                                 MoveOrderOut = moveorders.QuantityOrdered != null ? moveorders.QuantityOrdered : 0,
                                 QcReceiving = qcreceive.QuantityOrdered != null ? qcreceive.QuantityOrdered : 0,
                                 IssueOut = issueout.Quantity != null ? issueout.Quantity : 0,
                                 TotalPrice = posummary.TotalPrice != null ? posummary.TotalPrice : 0,
                                 SOH = SOH.SOH != null ? SOH.SOH : 0,
                                 Reserve = Reserve.Reserve != null ? Reserve.Reserve : 0,
                                 AverageIssuance = averageissuance.ActualGood != null ? averageissuance.ActualGood : 0,
                                 ReserveUsage = reserveusage.Reserve != null ? reserveusage.Reserve : 0,
                                 TransformFrom = transformfrom.WeighingScale != null ? transformfrom.WeighingScale : 0,
                                 TransformTo = transformto.Quantity != null ? transformto.Quantity : 0
                                 //  LastUsed = lastUsed.PreparedDate != null ? lastUsed.PreparedDate : null

                             } into total

                             select new MRPDto
                             {
                                 ItemCode = total.Key.ItemCode,
                                 ItemDescription = total.Key.ItemDescription,
                                 Uom = total.Key.UOM_Code,
                                 ItemCategory = total.Key.ItemCategoryName,
                                 BufferLevel = total.Key.BufferLevel,
                                 Price = total.Key.UnitPrice,
                                 ReceiveIn = total.Key.WarehouseActualGood,
                                 MoveOrderOut = total.Key.MoveOrderOut,
                                 QCReceiving = total.Key.QcReceiving,
                                 ReceiptIn = total.Key.ReceiptIn,
                                 IssueOut = total.Key.IssueOut,
                                 TotalPrice = Math.Round(Convert.ToDecimal(total.Key.TotalPrice), 2),
                                 SOH = total.Key.SOH - total.Key.IssueOut,
                                 Reserve = total.Key.Reserve - total.Key.IssueOut,
                                 SuggestedPo = total.Key.SuggestedPo,
                                 AverageIssuance = Math.Round(Convert.ToDecimal(total.Key.AverageIssuance), 2),
                                 DaysLevel = Math.Round(Convert.ToDecimal(total.Key.Reserve / (total.Key.AverageIssuance != 0 ? total.Key.AverageIssuance : 1)), 2),
                                 ReserveUsage = total.Key.ReserveUsage,
                                 TransformFrom = total.Key.TransformFrom,
                                 TransformTo = total.Key.TransformTo
                                 //     LastUsed = total.Key.LastUsed.ToString()

                             }).Where(x => x.ItemDescription.ToLower()
                               .Contains(search.Trim().ToLower()));

            return await PagedList<MRPDto>.CreateAsync(inventory, userParams.PageNumber, userParams.PageSize);
        }
    }
}
